using Application.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Projects
{
    public class UpdateMemberRole
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Guid ProjectId { get; set; }
            public string UserId { get; set; }
            public string Role { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var project = await _context.Projects
                    .Include(p => p.Participants)
                    .FirstOrDefaultAsync(p => p.Id == request.ProjectId, cancellationToken);

                if (project == null)
                    return Result<Unit>.Failure("Project not found");

                var participant = project.Participants
                    .FirstOrDefault(p => p.AppUserId == request.UserId);

                if (participant == null)
                    return Result<Unit>.Failure("User is not a member of this project");

                // Check if we're removing the last owner
                var ownerCount = project.Participants.Count(p => p.IsOwner);
                if (participant.IsOwner && request.Role != "Owner" && ownerCount <= 1)
                    return Result<Unit>.Failure("Cannot remove owner role from the last owner");

                // Update the role
                participant.Role = request.Role;
                participant.IsOwner = request.Role == "Owner";

                var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                if (!result)
                    return Result<Unit>.Failure("Failed to update member role");

                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}