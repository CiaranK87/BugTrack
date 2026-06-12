using Application.Core;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Projects
{
    public class ToggleCancel
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;

            public Handler(DataContext context, IUserAccessor userAccessor)
            {
                _context = context;
                _userAccessor = userAccessor;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var userId = _userAccessor.GetUserId();

                var isOwner = await _context.ProjectParticipants
                    .AnyAsync(pp => pp.ProjectId == request.Id && pp.AppUserId == userId && pp.IsOwner, cancellationToken);

                if (!isOwner) return Result<Unit>.Failure("Forbidden");

                var project = await _context.Projects.FindAsync(request.Id);

                if (project == null) return null;

                project.IsCancelled = !project.IsCancelled;

                var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                return result ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure("Problem updating project");
            }
        }
    }
}
