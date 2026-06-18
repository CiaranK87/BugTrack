using Microsoft.AspNetCore.Http;

namespace Application.Interfaces
{
    public interface IFileService
    {
        Task<string> UploadAsync(IFormFile file, CancellationToken cancellationToken = default);
        Task<(Stream Content, string ContentType)> DownloadAsync(string blobName, CancellationToken cancellationToken = default);
        Task DeleteAsync(string blobName, CancellationToken cancellationToken = default);
    }
}
