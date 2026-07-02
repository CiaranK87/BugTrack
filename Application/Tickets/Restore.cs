using Application.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Tickets
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
                var ticket = await _context.Tickets
                    .Include(t => t.Project)
                    .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

                if (ticket == null) return Result<Unit>.NotFound();

                if (!ticket.IsDeleted)
                    return Result<Unit>.Failure("Ticket is not deleted");

                ticket.IsDeleted = false;
                ticket.DeletedDate = null;

                var result = await _context.SaveChangesAsync() > 0;

                if (!result) return Result<Unit>.Failure("Failed to restore ticket");

                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}
