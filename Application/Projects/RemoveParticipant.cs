using Application.Core;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Projects
{
    public class RemoveParticipant
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Guid ProjectId { get; set; }
            public string UserId { get; set; }
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
                    return Result<Unit>.Failure("User is not a participant of this project");

                // Prevent removing the last owner
                var ownerCount = project.Participants.Count(p => p.IsOwner);
                if (participant.IsOwner && ownerCount <= 1)
                    return Result<Unit>.Failure("Cannot remove the last owner from the project");

                project.Participants.Remove(participant);

                var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                if (!result)
                    return Result<Unit>.Failure("Failed to remove participant from project");

                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}