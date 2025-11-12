using Application.Core;
using Domain;
using MediatR;
using Persistence;
using Microsoft.EntityFrameworkCore;

namespace Application.Tickets
{
    public class ListDeleted
    {
        public class Query : IRequest<Result<List<Ticket>>> {}

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
                    .Where(t => t.IsDeleted)
                    .Include(t => t.Project)
                    .ToListAsync(cancellationToken);

                return Result<List<Ticket>>.Success(tickets);
            }
        }
    }
}