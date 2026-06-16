using Application.Core;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Users
{
    public class Search
    {
        public class Query : IRequest<Result<List<UserSearchDto>>>
        {
            public string SearchQuery { get; set; }
            public Guid? ProjectId { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<UserSearchDto>>>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<Result<List<UserSearchDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                if (string.IsNullOrWhiteSpace(request.SearchQuery))
                    return Result<List<UserSearchDto>>.Failure("Query is required");

                var users = await _context.Users
                    .Where(u => !u.IsDeleted)
                    .Where(u =>
                        (u.DisplayName != null && u.DisplayName.ToLower().Contains(request.SearchQuery.ToLower())) ||
                        u.UserName.ToLower().Contains(request.SearchQuery.ToLower()))
                    .Take(20)
                    .ToListAsync(cancellationToken);

                var userIds = users.Select(u => u.Id).ToList();
                var participantUserIds = new HashSet<string>();

                if (request.ProjectId.HasValue)
                {
                    participantUserIds = (await _context.ProjectParticipants
                        .Where(pp => pp.ProjectId == request.ProjectId.Value && userIds.Contains(pp.AppUserId))
                        .Select(pp => pp.AppUserId)
                        .ToListAsync(cancellationToken)).ToHashSet();
                }

                var results = users.Select(u => new UserSearchDto
                {
                    Id = u.Id,
                    Name = u.DisplayName ?? u.UserName,
                    Username = u.UserName,
                    IsParticipant = !request.ProjectId.HasValue ||
                                    u.GlobalRole == Roles.Global.Admin ||
                                    participantUserIds.Contains(u.Id) ||
                                    _context.Projects.Any(p => p.Id == request.ProjectId.Value && p.ProjectOwner == u.UserName) ||
                                    _context.Tickets.Any(t => t.ProjectId == request.ProjectId.Value && (t.Assigned == u.UserName || t.Submitter == u.UserName))
                }).ToList();

                return Result<List<UserSearchDto>>.Success(results);
            }
        }
    }
}
