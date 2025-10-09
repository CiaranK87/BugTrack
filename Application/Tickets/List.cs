using Application.Core;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Tickets
{
    public class List
    {
        public class Query : IRequest<Result<List<Ticket>>>
        {
            public string UserId { get; set; }
            public string GlobalRole { get; set; }
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
                if (request.GlobalRole == "Admin")
                {
                    return Result<List<Ticket>>.Success(await _context.Tickets.ToListAsync(cancellationToken));
                }
                else
                {
                    var tickets = await _context.Tickets
                        .Where(t => t.Project.Participants.Any(p => p.AppUserId == request.UserId))
                        .ToListAsync(cancellationToken);
                    
                    return Result<List<Ticket>>.Success(tickets);
                }
            }
        }

    }
}