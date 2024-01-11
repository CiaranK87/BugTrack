using Application.Core;
using Domain;
using MediatR;
using Persistence;

namespace Application.Projects
{
    public class Details
    {
        public class Query : IRequest<Result<Project>>
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<Project>>
        {
        private readonly DataContext _context;
            public Handler(DataContext context)
            {
            _context = context;
            }

            public async Task<Result<Project>> Handle(Query request, CancellationToken cancellationToken)
            {
                var project = await _context.Projects.FindAsync(request.Id);

                return Result<Project>.Success(project);
            }
        }
    }
}