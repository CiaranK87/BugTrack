using Application.Core;
using MediatR;
using Persistence;

namespace Application.Projects
{
    public class Restore
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Guid Id { get; set; }
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
                var project = await _context.Projects.FindAsync(request.Id);

                if (project == null) return Result<Unit>.NotFound();

                if (!project.IsDeleted)
                    return Result<Unit>.Failure("Project is not deleted");

                project.IsDeleted = false;
                project.DeletedAt = null;

                var result = await _context.SaveChangesAsync() > 0;

                if (!result) return Result<Unit>.Failure("Failed to restore project");

                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}
