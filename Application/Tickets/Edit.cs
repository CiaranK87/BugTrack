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

                var existingSubmitter = ticket.Submitter;

                _mapper.Map(request.Ticket, ticket);

                ticket.Submitter = existingSubmitter;
                ticket.Updated = DateTime.UtcNow;

                // If the ticket is assigned to someone, ensure they are a project participant
                if (!string.IsNullOrEmpty(ticket.Assigned))
                {
                    var assignedUser = await _context.Users
                        .FirstOrDefaultAsync(u => u.UserName == ticket.Assigned, cancellationToken);
                    
                    if (assignedUser != null)
                    {
                        var existingParticipant = await _context.ProjectParticipants
                            .FirstOrDefaultAsync(pp => pp.ProjectId == ticket.ProjectId && pp.AppUserId == assignedUser.Id, cancellationToken);
                        
                        // If the user is not already a participant, add them as a regular user
                        if (existingParticipant == null)
                        {
                            var participant = new ProjectParticipant
                            {
                                ProjectId = ticket.ProjectId,
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
                               : Result<Unit>.Failure("Failed to update ticket");
            }
        }
    }
}
