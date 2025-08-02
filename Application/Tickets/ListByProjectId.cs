using Application.Core;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Tickets
{
    public class ListByProjectId
    {
        public class Query : IRequest<Result<List<Ticket>>>
        {
            public Guid ProjectId { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<Ticket>>>
        {
        private readonly DataContext _context;
            public Handler(DataContext context)
            {
            _context = context;
            }

            public async Task<Result<List<Ticket>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var tickets = await _context.Tickets
                    .Where(t => t.ProjectId == request.ProjectId) // Add filtering
                    .ToListAsync(cancellationToken);
                
                return Result<List<Ticket>>.Success(tickets);
            }
        }

    }
}