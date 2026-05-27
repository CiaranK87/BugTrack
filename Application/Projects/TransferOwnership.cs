using Application.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Projects
{
    public class TransferOwnership
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Guid ProjectId { get; set; }
            public string NewOwnerId { get; set; }
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
                var currentOwner = await _context.ProjectParticipants
                    .FirstOrDefaultAsync(pp => pp.ProjectId == request.ProjectId && pp.IsOwner, cancellationToken);

                if (currentOwner == null)
                    return Result<Unit>.Failure("Project owner not found");

                var newOwner = await _context.ProjectParticipants
                    .FirstOrDefaultAsync(pp => pp.ProjectId == request.ProjectId && pp.AppUserId == request.NewOwnerId, cancellationToken);

                if (newOwner == null)
                    return Result<Unit>.Failure("New owner must be a project participant");

                currentOwner.IsOwner = false;
                currentOwner.Role = "ProjectManager";

                newOwner.IsOwner = true;
                newOwner.Role = null;

                await _context.SaveChangesAsync(cancellationToken);

                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}
