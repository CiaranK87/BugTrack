using Application.Core;
using MediatR;
using Persistence;

namespace Application.Projects
{
    public class AdminDelete
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

                if (project == null) return null;

                if (!project.IsDeleted)
                    return Result<Unit>.Failure("Project must be soft deleted first");

                _context.Projects.Remove(project);
                var result = await _context.SaveChangesAsync() > 0;

                if (!result) return Result<Unit>.Failure("Failed to permanently delete project");

                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}
