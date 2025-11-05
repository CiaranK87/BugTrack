using Application.Core;
using AutoMapper;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Tickets
{
    public class Edit
    {
        public class Command : IRequest<Result<Unit>>
        {
            public EditTicketDto EditDto { get; set; }
            public Guid Id { get; set; }
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
                var ticket = await _context.Tickets.FindAsync(request.Id);
                if (ticket == null) return null;

                var existingSubmitter = ticket.Submitter;

                ticket.Title = request.EditDto.Title;
                ticket.Description = request.EditDto.Description;
                ticket.Assigned = request.EditDto.Assigned;
                ticket.Priority = request.EditDto.Priority;
                ticket.Severity = request.EditDto.Severity;
                
                var previousStatus = ticket.Status;
                ticket.Status = request.EditDto.Status;
                
                if (previousStatus != "Closed" && ticket.Status == "Closed")
                {
                    ticket.ClosedDate = DateTime.UtcNow;
                }
                else if (ticket.Status != "Closed")
                {
                    ticket.ClosedDate = null;
                }
                
                ticket.StartDate = request.EditDto.StartDate ?? DateTime.UtcNow;
                ticket.EndDate = request.EditDto.EndDate ?? DateTime.UtcNow;
                
                ticket.Submitter = existingSubmitter;
                ticket.Updated = DateTime.UtcNow;

                _context.Entry(ticket).State = EntityState.Modified;

                var success = await _context.SaveChangesAsync() > 0;
                return success ? Result<Unit>.Success(Unit.Value)
                               : Result<Unit>.Failure("Failed to update ticket");
            }
        }
    }
}
