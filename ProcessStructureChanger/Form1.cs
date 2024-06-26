using S3BucketStructure;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProcessStructureChanger
{
    public partial class Form1 : Form
    {
        private S3Service s3Service;
        private string bucketName;

        public Form1()
        {
            InitializeComponent();
            updateBucketButton.Click += UpdateBucketButton_Click;
            downloadButton.Click += async (s, e) => await DownloadSelectedNodeAsync();
            selectFolderButton.Click += SelectFolderButton_Click;
            treeView1.AfterSelect += TreeView1_AfterSelect;
            folderPathTextBox.Enabled = false;
            downloadButton.Enabled = false;
        }

        private async void UpdateBucketButton_Click(object sender, EventArgs e)
        {
            bucketName = bucketNameTextBox.Text;
            if (string.IsNullOrEmpty(bucketName))
            {
                MessageBox.Show(this, "Por favor, insira um nome de bucket válido.", "Nome de Bucket Inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            s3Service = new S3Service(bucketName, Amazon.RegionEndpoint.SAEast1);
            await DisplayBucketStructureAsync();
        }

        private async Task DisplayBucketStructureAsync()
        {
            treeView1.Nodes.Clear();

            var rootNode = new TreeNode(bucketName);
            var objects = await s3Service.ListObjectsAsync();

            foreach (var obj in objects)
            {
                AddNode(rootNode, obj);
            }

            treeView1.Nodes.Add(rootNode);
            CollapseAllNodes(treeView1);
            folderPathTextBox.Enabled = true;
        }

        private void AddNode(TreeNode rootNode, string path)
        {
            var parts = path.Split('/');
            var currentNode = rootNode;

            foreach (var part in parts)
            {
                if (string.IsNullOrEmpty(part)) continue;

                var existingNode = FindNode(currentNode, part);
                if (existingNode == null)
                {
                    var newNode = new TreeNode(part);
                    currentNode.Nodes.Add(newNode);
                    currentNode = newNode;
                }
                else
                {
                    currentNode = existingNode;
                }
            }
        }

        private TreeNode FindNode(TreeNode node, string text)
        {
            foreach (TreeNode child in node.Nodes)
            {
                if (child.Text == text)
                {
                    return child;
                }
            }
            return null;
        }

        private void CollapseAllNodes(System.Windows.Forms.TreeView treeView)
        {
            foreach (TreeNode node in treeView.Nodes)
            {
                CollapseNode(node);
            }
        }

        private void CollapseNode(TreeNode node)
        {
            node.Collapse();
            foreach (TreeNode childNode in node.Nodes)
            {
                CollapseNode(childNode);
            }
        }

        private async Task DownloadSelectedNodeAsync()
        {
            var selectedNode = treeView1.SelectedNode;
            if (selectedNode == null)
            {
                MessageBox.Show(this, "Por favor, selecione um nó.", "Nenhum Nó Selecionado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(folderPathTextBox.Text) || !Directory.Exists(folderPathTextBox.Text))
            {
                MessageBox.Show(this, "Por favor, selecione um diretório de download válido.", "Diretório Inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string localDirectory = folderPathTextBox.Text;
            string selectedPath = GetFullPath(selectedNode);

            // Remove o nome do bucket do caminho selecionado
            if (selectedPath.StartsWith(bucketName))
            {
                selectedPath = selectedPath.Substring(bucketName.Length).TrimStart(Path.DirectorySeparatorChar).Replace("\\", "/");
            }

            var objects = await s3Service.ListObjectsAsync();
            var filesToDownload = objects.FindAll(obj => obj.StartsWith(selectedPath));

            progressBar1.Maximum = filesToDownload.Count;
            progressBar1.Value = 0;
            listBox1.Items.Clear();

            foreach (var obj in filesToDownload)
            {
                // Inclui o nome da pasta selecionada no caminho local
                string localFilePath = Path.Combine(localDirectory, obj.Replace('/', Path.DirectorySeparatorChar));
                Directory.CreateDirectory(Path.GetDirectoryName(localFilePath));
                await s3Service.DownloadFileAsync(bucketName, obj, localFilePath);

                // Atualiza a barra de progresso e a lista de arquivos
                progressBar1.Value++;
                listBox1.Items.Add($"Baixando {obj} para {localFilePath}");
            }
            progressBar1.Maximum = 0;
            progressBar1.Value = 0;
            MessageBox.Show(this, $"Arquivos baixados para {localDirectory}", "Download Completo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private bool IsLastNode(TreeNode node)
        {
            if (node == null || node.Parent == null)
            {
                // Se o nó é nulo ou não tem pai, ele pode ser o último nó no nível raiz
                return node.TreeView.Nodes[node.TreeView.Nodes.Count - 1] == node;
            }
            else
            {
                // Caso contrário, verifique se é o último nó do seu pai
                TreeNode parentNode = node.Parent;
                return parentNode.Nodes[parentNode.Nodes.Count - 1] == node;
            }
        }

        private string GetFullPath(TreeNode node)
        {
            return node.Parent == null ? node.Text : Path.Combine(GetFullPath(node.Parent), node.Text);
        }

        private void SelectFolderButton_Click(object sender, EventArgs e)
        {
            using (var folderBrowserDialog = new FolderBrowserDialog())
            {
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    folderPathTextBox.Text = folderBrowserDialog.SelectedPath;
                }
            }
        }

        private void TreeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            downloadButton.Enabled = treeView1.SelectedNode != null;
        }
    }
}