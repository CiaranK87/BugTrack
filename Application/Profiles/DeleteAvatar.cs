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

                if (user == null) return null;
                if (string.IsNullOrEmpty(user.AvatarBlobName))
                    return Result<Unit>.Failure("No avatar to delete");

                await _fileService.DeleteAsync(user.AvatarBlobName, cancellationToken);
                user.AvatarBlobName = null;

                await _context.SaveChangesAsync(cancellationToken);
                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}
