using Application.Core;
using MediatR;
using Persistence;

namespace Application.Tickets
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
                var ticket = await _context.Tickets.FindAsync(request.Id);

                if (ticket == null) return null;

                if (!ticket.IsDeleted)
                    return Result<Unit>.Failure("Ticket must be soft deleted first");

                _context.Tickets.Remove(ticket);
                var result = await _context.SaveChangesAsync() > 0;

                if (!result) return Result<Unit>.Failure("Failed to permanently delete ticket");

                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}
