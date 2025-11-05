using Application.Core;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Tickets
{
    public class Create
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Ticket Ticket { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Ticket).SetValidator(new TicketValidator());
            }
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
                request.Ticket.CreatedAt = DateTime.UtcNow;
                request.Ticket.Updated = DateTime.UtcNow;
                _context.Tickets.Add(request.Ticket);

                if (!string.IsNullOrEmpty(request.Ticket.Assigned))
                {
                    var assignedUser = await _context.Users
                        .FirstOrDefaultAsync(u => u.UserName == request.Ticket.Assigned, cancellationToken);
                    
                    if (assignedUser != null)
                    {
                        var existingParticipant = await _context.ProjectParticipants
                            .FirstOrDefaultAsync(pp => pp.ProjectId == request.Ticket.ProjectId && pp.AppUserId == assignedUser.Id, cancellationToken);
                        
                        if (existingParticipant == null)
                        {
                            var participant = new ProjectParticipant
                            {
                                ProjectId = request.Ticket.ProjectId,
                                AppUserId = assignedUser.Id,
                                IsOwner = false,
                                Role = "User"
                            };
                            _context.ProjectParticipants.Add(participant);
                        }
                    }
                }

                var success = await _context.SaveChangesAsync() > 0;
                return success ? Result<Unit>.Success(Unit.Value)
                               : Result<Unit>.Failure("Failed to create ticket");
            }
        }
    }
}
