using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;

namespace AuthenticationServices.Services
{
    public class BlobServices : IBlobServices
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;

        public BlobServices(BlobServiceClient blobServiceClient, IConfiguration configuration)
        {
            _blobServiceClient = blobServiceClient;
            _containerName = configuration.GetSection("AzureBlobStorage:ContainerName").Value;
        }

        public async Task<string> UploadBlobAsync(IFormFile file)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(Guid.NewGuid().ToString() + "_" + file.FileName);

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, true);
            }

            return blobClient.Uri.ToString();
        }

        public async Task DeleteBlobAsync(string blobUrl)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobName = Path.GetFileName(new Uri(blobUrl).LocalPath);
            var blobClient = containerClient.GetBlobClient(blobName);

            await blobClient.DeleteIfExistsAsync();
        }

        public async Task<string> UploadBlobWithContentTypeAsync(IFormFile file, string contentType)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(Guid.NewGuid().ToString() + "_" + file.FileName);
            var blobHttpHeader = new BlobHttpHeaders { ContentType = contentType };

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, new BlobUploadOptions
                {
                    HttpHeaders = blobHttpHeader
                });
            }

            return blobClient.Uri.ToString();
        }
    }
}
