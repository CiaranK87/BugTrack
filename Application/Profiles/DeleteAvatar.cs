using Application.Core;
using Application.Interfaces;
using MediatR;
using Persistence;

namespace Application.Profiles
{
    public class DeleteAvatar
    {
        public class Command : IRequest<Result<Unit>>
        {
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
                if (string.IsNullOrEmpty(user.AvatarBlobName))
                    return Result<Unit>.Failure("No avatar to delete");

                var blobName = user.AvatarBlobName;
                user.AvatarBlobName = null;
                await _context.SaveChangesAsync(cancellationToken);

                try { await _fileService.DeleteAsync(blobName, cancellationToken); }
                catch { /* orphaned blob is acceptable; do not block the delete */ }

                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}
