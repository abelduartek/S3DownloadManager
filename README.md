# 🌐 S3 File Downloader

Bem-vindo ao **S3 File Downloader**! Este projeto é uma aplicação Windows Forms que permite aos usuários baixar arquivos de um bucket Amazon S3 selecionado. A interface do usuário é simples e intuitiva, facilitando a navegação e o download de arquivos.

## 📋 Funcionalidades

- 📂 **Exploração de Buckets S3:** Insira o nome do bucket S3 e explore a estrutura de diretórios.
- 📥 **Download de Arquivos:** Baixe arquivos e diretórios selecionados para um caminho local especificado.
- 🚀 **Progresso do Download:** Acompanhe o progresso dos downloads com uma barra de progresso.
- 📑 **Lista de Arquivos:** Veja os arquivos que estão sendo baixados em tempo real.

## 🛠️ Instalação

1. Clone o repositório:

git clone https://github.com/seu-usuario/s3-file-downloader.git

2. Abra a solução no Visual Studio:
   
cd s3-file-downloader
start S3FileDownloader.sln

3. Restaure os pacotes NuGet necessários:
   
dotnet restore

4. Compile e execute o projeto:

dotnet run

💻 Uso
Insira o nome do bucket S3:

Digite o nome do bucket S3 no campo "Bucket Name" e clique em "Update Bucket".
Exploração do Bucket:

A estrutura do bucket será exibida no TreeView.
Selecione um diretório de download:

Clique em "Select Folder" e escolha o diretório onde deseja baixar os arquivos.
Download dos arquivos:

Selecione o nó desejado no TreeView.
Clique em "Download Selected Node" para iniciar o download dos arquivos.

🎨 Interface do Usuário

![image](https://github.com/abelduartek/S3DownloadManager/assets/50053954/f243a885-2312-48e2-b2d4-6ec32e812594)


🚧 Requisitos
.NET Framework 4.7.2 ou superior
Visual Studio 2019 ou superior
🤝 Contribuições
Contribuições são bem-vindas! Sinta-se à vontade para abrir issues e pull requests.

📜 Licença
Este projeto está licenciado sob a Licença MIT. Veja o arquivo LICENSE para mais detalhes.

📞 Contato
Nome: Abel Francisco Duarte
Email: abelduartek@gmail.com

Feito com ❤️ por @abelduartek

