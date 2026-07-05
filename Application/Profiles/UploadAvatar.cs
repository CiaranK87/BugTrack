using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using MediatR;
using Persistence;

namespace Application.Profiles
{
    public class UploadAvatar
    {
        public class Command : IRequest<Result<Unit>>
        {
            public FileUploadDto File { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;
            private readonly IFileService _fileService;

            public Handler(DataContext context, IUserAccessor userAccessor, IFileService fileService)
            {
                _context = context;
                _userAccessor = userAccessor;
                _fileService = fileService;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var userId = _userAccessor.GetUserId();
                var user = await _context.Users.FindAsync(userId);

                if (user == null) return Result<Unit>.Failure("User not found");

                if (request.File == null)
                    return Result<Unit>.Failure("No file provided");

                if (request.File.Length > 5 * 1024 * 1024)
                    return Result<Unit>.Failure("Avatar must be 5 MB or smaller");

                var oldBlobName = user.AvatarBlobName;
                user.AvatarBlobName = await _fileService.UploadAsync(request.File, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                if (!string.IsNullOrEmpty(oldBlobName))
                {
                    try { await _fileService.DeleteAsync(oldBlobName, cancellationToken); }
                    catch { /* orphaned blob is acceptable; do not block the upload */ }
                }

                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}
