using Application.Core;
using Application.Interfaces;
using Domain;
using MediatR;

namespace Application.Account
{
    public class GetCurrentUser
    {
        public class Query : IRequest<Result<AppUser>>
        {
            public string Email { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<AppUser>>
        {
            private readonly IAccountService _accountService;

            public Handler(IAccountService accountService)
            {
                _accountService = accountService;
            }

            public async Task<Result<AppUser>> Handle(Query request, CancellationToken cancellationToken)
            {
                var user = await _accountService.FindByEmailAsync(request.Email);
                if (user == null || user.IsDeleted)
                    return Result<AppUser>.Failure("User not found");
                return Result<AppUser>.Success(user);
            }
        }
    }
}
