using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Users
{
    public class Update
    {
        public class Command : IRequest<Result<UserDto>>
        {
            public string UserId { get; set; }
            public UpdateUserDto Dto { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<UserDto>>
        {
            private readonly DataContext _context;
            private readonly UserManager<AppUser> _userManager;
            private readonly IUserAccessor _userAccessor;

            public Handler(DataContext context, UserManager<AppUser> userManager, IUserAccessor userAccessor)
            {
                _context = context;
                _userManager = userManager;
                _userAccessor = userAccessor;
            }

            public async Task<Result<UserDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                var currentUserId = _userAccessor.GetUserId();
                var user = await _context.Users.FindAsync(request.UserId);

                if (user == null)
                    return null;

                if (user.GlobalRole == Roles.Global.Admin && user.Id != currentUserId)
                    return Result<UserDto>.Failure("Admins cannot modify other admins");

                var oldUsername = user.UserName;

                user.DisplayName = request.Dto.DisplayName ?? user.DisplayName;
                user.JobTitle = request.Dto.JobTitle;
                user.Bio = request.Dto.Bio;

                if (request.Dto.Email != null && request.Dto.Email != user.Email)
                {
                    var setEmailResult = await _userManager.SetEmailAsync(user, request.Dto.Email);
                    if (!setEmailResult.Succeeded)
                        return Result<UserDto>.Failure(string.Join(", ", setEmailResult.Errors.Select(e => e.Description)));
                }

                if (request.Dto.Username != null && request.Dto.Username != oldUsername)
                {
                    var setUsernameResult = await _userManager.SetUserNameAsync(user, request.Dto.Username);
                    if (!setUsernameResult.Succeeded)
                        return Result<UserDto>.Failure(string.Join(", ", setUsernameResult.Errors.Select(e => e.Description)));

                    var ticketsToUpdate = await _context.Tickets
                        .Where(t => t.Submitter == oldUsername || t.Assigned == oldUsername)
                        .ToListAsync(cancellationToken);

                    foreach (var ticket in ticketsToUpdate)
                    {
                        if (ticket.Submitter == oldUsername)
                            ticket.Submitter = user.UserName;
                        if (ticket.Assigned == oldUsername)
                            ticket.Assigned = user.UserName;
                    }
                }

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
