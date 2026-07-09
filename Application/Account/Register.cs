using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Account
{
    public class Register
    {
        public class Command : IRequest<Result<AppUser>>
        {
            public RegisterDto RegisterDto { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<AppUser>>
        {
            private readonly IAccountService _accountService;
            private readonly IConfiguration _config;
            private readonly ILogger<Handler> _logger;

            public Handler(IAccountService accountService, IConfiguration config, ILogger<Handler> logger)
            {
                _accountService = accountService;
                _config = config;
                _logger = logger;
            }

            public async Task<Result<AppUser>> Handle(Command request, CancellationToken cancellationToken)
            {
                var allowRegistration = _config.GetValue<bool>("SecuritySettings:AllowRegistration", true);
                if (!allowRegistration)
                {
                    _logger.LogWarning("Registration attempt when registration is disabled for email: {Email}", request.RegisterDto.Email);
                    return Result<AppUser>.Failure("User registration is currently disabled");
                }

                if (await _accountService.UserNameExistsAsync(request.RegisterDto.Username))
                    return Result<AppUser>.Failure("Username is already taken");

                if (await _accountService.EmailExistsAsync(request.RegisterDto.Email))
                    return Result<AppUser>.Failure("Email is already taken");

                var user = new AppUser
                {
                    DisplayName = request.RegisterDto.DisplayName,
                    Email = request.RegisterDto.Email,
                    UserName = request.RegisterDto.Username,
                };

                var result = await _accountService.CreateAsync(user, request.RegisterDto.Password);
                if (!result.IsSuccess) return result;

                _logger.LogInformation("New user registered: {Username} ({Email})", user.UserName, user.Email);
                return Result<AppUser>.Success(user);
            }
        }
    }
}
