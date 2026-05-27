using Application.Core;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Projects
{
    public class GetUserRole
    {
        public class Query : IRequest<Result<string>>
        {
            public Guid ProjectId { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<string>>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;

            public Handler(DataContext context, IUserAccessor userAccessor)
            {
                _context = context;
                _userAccessor = userAccessor;
            }

            public async Task<Result<string>> Handle(Query request, CancellationToken cancellationToken)
            {
                var globalRole = _userAccessor.GetGlobalRole();
                if (globalRole == "Admin") return Result<string>.Success("Admin");

                var userId = _userAccessor.GetUserId();

                var participant = await _context.ProjectParticipants
                    .AsNoTracking()
                    .FirstOrDefaultAsync(pp => pp.ProjectId == request.ProjectId && pp.AppUserId == userId, cancellationToken);

                if (participant == null) return Result<string>.Success("User");

                if (participant.IsOwner) return Result<string>.Success("Owner");

                return Result<string>.Success(participant.Role ?? "User");
            }
        }
    }
}
