using Application.Core;
using Application.Interfaces;
using Domain;
using MediatR;
using Persistence;

namespace Application.Users
{
    public class SoftDelete
    {
        public class Command : IRequest<Result<Unit>>
        {
            public string UserId { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;

            public Handler(DataContext context, IUserAccessor userAccessor)
            {
                _context = context;
                _userAccessor = userAccessor;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var currentUserId = _userAccessor.GetUserId();
                var user = await _context.Users.FindAsync(request.UserId);

                if (user == null)
                    return null;

                if (user.GlobalRole == Roles.Global.Admin && user.Id != currentUserId)
                    return Result<Unit>.Failure("Admins cannot delete other admins");

                if (user.Id == currentUserId)
                    return Result<Unit>.Failure("Admins cannot delete themselves");

                user.IsDeleted = true;
                user.DeletedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync(cancellationToken);

                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}
