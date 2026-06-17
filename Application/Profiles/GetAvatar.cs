using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles
{
    public class GetAvatar
    {
        public class Query : IRequest<(Stream FileStream, string ContentType)>
        {
            public string Username { get; set; }
        }

        public class Handler : IRequestHandler<Query, (Stream FileStream, string ContentType)>
        {
            private readonly DataContext _context;
            private readonly IFileService _fileService;

            public Handler(DataContext context, IFileService fileService)
            {
                _context = context;
                _fileService = fileService;
            }

            public async Task<(Stream FileStream, string ContentType)> Handle(Query request, CancellationToken cancellationToken)
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserName == request.Username, cancellationToken);

                if (user == null || string.IsNullOrEmpty(user.AvatarBlobName))
                    return (null, null);

                var stream = await _fileService.DownloadAsync(user.AvatarBlobName, cancellationToken);
                return (stream, GetContentType(user.AvatarBlobName));
            }

            private static string GetContentType(string blobName) =>
                Path.GetExtension(blobName)?.ToLower() switch
                {
                    ".jpg" or ".jpeg" => "image/jpeg",
                    ".png" => "image/png",
                    ".gif" => "image/gif",
                    ".webp" => "image/webp",
                    _ => "image/octet-stream"
                };
        }
    }
}
