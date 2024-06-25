using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace S3BucketStructure
{
    public class S3Service
    {
        private readonly IAmazonS3 s3Client;
        private readonly string bucketName;

        public S3Service(string bucketName, RegionEndpoint region)
        {
            this.bucketName = bucketName;
            s3Client = new AmazonS3Client(region);
        }

        public async Task<List<string>> ListObjectsAsync()
        {
            var request = new ListObjectsV2Request
            {
                BucketName = bucketName
            };

            var objects = new List<string>();
            ListObjectsV2Response response;

            do
            {
                response = await s3Client.ListObjectsV2Async(request);

                foreach (S3Object entry in response.S3Objects)
                {
                    objects.Add(entry.Key);
                }

                request.ContinuationToken = response.NextContinuationToken;
            } while (response.IsTruncated);

            return objects;
        }

        public async Task DownloadFileAsync(string bucket, string key, string localPath)
        {
            var request = new GetObjectRequest
            {
                BucketName = bucket,
                Key = key
            };

            using (GetObjectResponse response = await s3Client.GetObjectAsync(request))
            using (Stream responseStream = response.ResponseStream)
            using (FileStream fileStream = new FileStream(localPath, FileMode.Create, FileAccess.Write))
            {
                await responseStream.CopyToAsync(fileStream);
            }
        }
    }
}