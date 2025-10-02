using Application.Core;
using AutoMapper;
using Domain;
using MediatR;
using Persistence;

namespace Application.Tickets
{
    public class Edit
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Ticket Ticket { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var ticket = await _context.Tickets.FindAsync(request.Ticket.Id);
                if (ticket == null) return null;

                // Preserve immutable fields
                var existingSubmitter = ticket.Submitter;

                _mapper.Map(request.Ticket, ticket);

                // Restore preserved fields
                ticket.Submitter = existingSubmitter;
                ticket.Updated = DateTime.UtcNow;

                var success = await _context.SaveChangesAsync() > 0;
                return success ? Result<Unit>.Success(Unit.Value)
                               : Result<Unit>.Failure("Failed to update ticket");
            }
        }
    }
}
