using Application.DTOs;
using Application.Interfaces;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Storage
{
    public class AzureBlobFileService : IFileService
    {
        private readonly BlobContainerClient _containerClient;

        public AzureBlobFileService(IConfiguration config)
        {
            var connectionString = config["AzureStorageConnectionString"];
            var containerName = config["AzureStorageContainerName"] ?? "attachments";
            _containerClient = new BlobContainerClient(connectionString, containerName);
        }

        public async Task<string> UploadAsync(FileUploadDto file, CancellationToken cancellationToken = default)
        {
            await _containerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
            var blobName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var blobClient = _containerClient.GetBlobClient(blobName);
            await blobClient.UploadAsync(file.Content, new BlobHttpHeaders { ContentType = file.ContentType }, cancellationToken: cancellationToken);
            return blobName;
        }

        public async Task<(Stream Content, string ContentType)> DownloadAsync(string blobName, CancellationToken cancellationToken = default)
        {
            var blobClient = _containerClient.GetBlobClient(blobName);
            try
            {
                var response = await blobClient.DownloadStreamingAsync(cancellationToken: cancellationToken);
                var contentType = response.Value.Details?.ContentType;
                if (string.IsNullOrWhiteSpace(contentType))
                    contentType = FallbackContentType(blobName);
                return (response.Value.Content, contentType);
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return (null, null);
            }
        }

        private static string FallbackContentType(string blobName) =>
            Path.GetExtension(blobName)?.ToLowerInvariant() switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png"            => "image/png",
                ".gif"            => "image/gif",
                ".webp"           => "image/webp",
                _                 => "application/octet-stream"
            };

        public async Task DeleteAsync(string blobName, CancellationToken cancellationToken = default)
        {
            var blobClient = _containerClient.GetBlobClient(blobName);
            await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
        }
    }
}
