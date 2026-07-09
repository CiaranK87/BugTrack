using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using Domain;
using MediatR;

namespace Application.Account
{
    public class AdminRegister
    {
        public class Command : IRequest<Result<AppUser>>
        {
            public AdminRegisterDto RegisterDto { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<AppUser>>
        {
            private readonly IAccountService _accountService;

            public Handler(IAccountService accountService)
            {
                _accountService = accountService;
            }

            public async Task<Result<AppUser>> Handle(Command request, CancellationToken cancellationToken)
            {
                if (await _accountService.UserNameExistsAsync(request.RegisterDto.Username))
                    return Result<AppUser>.Failure("Username is already taken");

                if (await _accountService.EmailExistsAsync(request.RegisterDto.Email))
                    return Result<AppUser>.Failure("Email is already taken");

                var validRoles = new HashSet<string>
                {
                    Roles.Global.Admin, Roles.Global.ProjectManager,
                    Roles.Global.Developer, Roles.Global.User, Roles.Global.Guest
                };
                var roleToSet = request.RegisterDto.Role ?? Roles.Global.User;
                if (!validRoles.Contains(roleToSet))
                    return Result<AppUser>.Failure("Invalid role specified");

                var user = new AppUser
                {
                    DisplayName = request.RegisterDto.DisplayName,
                    Email = request.RegisterDto.Email,
                    UserName = request.RegisterDto.Username,
                    JobTitle = request.RegisterDto.JobTitle
                };

                return await _accountService.CreateWithRoleAsync(user, request.RegisterDto.Password, roleToSet);
            }
        }
    }
}
