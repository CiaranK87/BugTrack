using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using MediatR;
using Persistence;

namespace Application.Users
{
    public class UpdateRole
    {
        public class Command : IRequest<Result<UserDto>>
        {
            public string UserId { get; set; }
            public string Role { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<UserDto>>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;

            public Handler(DataContext context, IUserAccessor userAccessor)
            {
                _context = context;
                _userAccessor = userAccessor;
            }

            public async Task<Result<UserDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                var currentUserId = _userAccessor.GetUserId();
                var user = await _context.Users.FindAsync(request.UserId);

                if (user == null)
                    return null;

                if (user.GlobalRole == "Admin" && user.Id != currentUserId)
                    return Result<UserDto>.Failure("Admins cannot modify other admins");

                var validRoles = new[] { "Admin", "ProjectManager", "Developer", "User" };
                if (!validRoles.Contains(request.Role))
                    return Result<UserDto>.Failure("Invalid role");

                if (user.Id == currentUserId && request.Role != "Admin")
                    return Result<UserDto>.Failure("Admins cannot demote themselves");

                user.GlobalRole = request.Role;
                await _context.SaveChangesAsync(cancellationToken);

                return Result<UserDto>.Success(new UserDto
                {
                    Id = user.Id,
                    Username = user.UserName,
                    DisplayName = user.DisplayName,
                    Email = user.Email,
                    GlobalRole = user.GlobalRole,
                    JobTitle = user.JobTitle,
                    Bio = user.Bio
                });
            }
        }
    }
}
