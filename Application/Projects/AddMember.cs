using Application.Core;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Projects
{
    public class AddParticipant
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
                // Check if project exists
                var project = await _context.Projects
                    .Include(p => p.Participants)
                    .FirstOrDefaultAsync(p => p.Id == request.ProjectId, cancellationToken);

                if (project == null) 
                    return Result<Unit>.Failure("Project not found");

                // Check if user exists
                var user = await _context.Users.FindAsync(request.UserId);
                if (user == null) 
                    return Result<Unit>.Failure("User not found");

                // Check if user is already a participant
                if (project.Participants.Any(p => p.AppUserId == request.UserId))
                    return Result<Unit>.Failure("User is already a participant of this project");

                // Add the participant
                project.Participants.Add(new ProjectParticipant
                {
                    AppUserId = request.UserId,
                    ProjectId = request.ProjectId,
                    Role = request.Role,
                    IsOwner = request.Role == "Owner"
                });

                var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                if (!result) 
                    return Result<Unit>.Failure("Failed to add participant to project");

                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}