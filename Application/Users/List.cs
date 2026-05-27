using Application.Core;
using Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Users
{
    public class List
    {
        public class Query : IRequest<Result<List<UserDto>>> { }

        public class Handler : IRequestHandler<Query, Result<List<UserDto>>>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<Result<List<UserDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var users = await _context.Users
                    .Where(u => !u.IsDeleted)
                    .Select(u => new UserDto
                    {
                        Id = u.Id,
                        Username = u.UserName,
                        DisplayName = u.DisplayName,
                        Email = u.Email,
                        GlobalRole = u.GlobalRole ?? "User",
                        JobTitle = u.JobTitle,
                        Bio = u.Bio
                    })
                    .ToListAsync(cancellationToken);

                return Result<List<UserDto>>.Success(users);
            }
        }
    }
}
