using Microsoft.AspNetCore.Http;

namespace Application.Interfaces
{
    public interface IFileService
    {
        Task<string> UploadAsync(IFormFile file, CancellationToken cancellationToken = default);
        Task<Stream> DownloadAsync(string blobName, CancellationToken cancellationToken = default);
        Task DeleteAsync(string blobName, CancellationToken cancellationToken = default);
    }
}
