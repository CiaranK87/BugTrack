using Application.Core;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Tickets
{
    public class Details
    {
      public class Query : IRequest<Result<Ticket>>
      {
        public Guid Id { get; set; }
      }

        public class Handler : IRequestHandler<Query, Result<Ticket>>
        {
        private readonly DataContext _context;
            public Handler(DataContext context)
            {
            _context = context;
            }

            public async Task<Result<Ticket>> Handle(Query request, CancellationToken cancellationToken)
            {
                var ticket = await _context.Tickets
                    .Include(t => t.Project)
                    .FirstOrDefaultAsync(t => t.Id == request.Id);

                return Result<Ticket>.Success(ticket);
            }
        }
    }
}