using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Account
{
    public class ChangePassword
    {
        public class Command : IRequest<Result<Unit>>
        {
            public string UserId { get; set; }
            public ChangePasswordDto ChangePasswordDto { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly IAccountService _accountService;
            private readonly ILogger<Handler> _logger;

            public Handler(IAccountService accountService, ILogger<Handler> logger)
            {
                _accountService = accountService;
                _logger = logger;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _accountService.FindByIdAsync(request.UserId);
                if (user == null)
                    return Result<Unit>.Failure("User not found");

                var result = await _accountService.ChangePasswordAsync(
                    user,
                    request.ChangePasswordDto.CurrentPassword,
                    request.ChangePasswordDto.NewPassword);

                if (result.IsSuccess)
                    _logger.LogInformation("Password changed successfully for user: {Username}", user.UserName);
                else
                    _logger.LogError("Password change failed for user {Username}: {Error}", user.UserName, result.Error);

                return result;
            }
        }
    }
}
