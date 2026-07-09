using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using Domain;
using MediatR;

namespace Application.Account
{
    public class Login
    {
        public class Command : IRequest<Result<AppUser>>
        {
            public LoginDto LoginDto { get; set; }
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
                var user = await _accountService.FindByEmailAsync(request.LoginDto.Email);
                if (user == null || user.IsDeleted)
                    return Result<AppUser>.Failure("Invalid credentials");

                var valid = await _accountService.CheckPasswordAsync(user, request.LoginDto.Password);
                if (!valid)
                    return Result<AppUser>.Failure("Invalid credentials");

                return Result<AppUser>.Success(user);
            }
        }
    }
}
