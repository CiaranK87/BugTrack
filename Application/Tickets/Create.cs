using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Tickets
{
    public class Create
    {
        public class Command : IRequest<Result<Guid>>
        {
            public CreateTicketDto TicketDto { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.TicketDto.Title).NotEmpty();
                RuleFor(x => x.TicketDto.Description).NotEmpty();
                RuleFor(x => x.TicketDto.Assigned).NotEmpty();
                RuleFor(x => x.TicketDto.Priority).NotEmpty();
                RuleFor(x => x.TicketDto.Severity).NotEmpty();
                RuleFor(x => x.TicketDto.StartDate).NotEmpty();
                RuleFor(x => x.TicketDto.ProjectId).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Command, Result<Guid>>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;

            public Handler(DataContext context, IUserAccessor userAccessor)
            {
                _context = context;
                _userAccessor = userAccessor;
            }

            public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername(), cancellationToken);

                if (user == null) return Result<Guid>.Failure("User not found");

                var ticket = new Ticket
                {
                    Title = request.TicketDto.Title,
                    Description = request.TicketDto.Description,
                    Assigned = request.TicketDto.Assigned,
                    Priority = request.TicketDto.Priority,
                    Severity = request.TicketDto.Severity,
                    StartDate = request.TicketDto.StartDate ?? DateTime.UtcNow,
                    EndDate = request.TicketDto.EndDate,
                    ProjectId = request.TicketDto.ProjectId,
                    Submitter = user.UserName,
                    Status = "Open",
                    IsDeleted = false,
                    DeletedAt = null,
                    ClosedDate = null,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Tickets.Add(ticket);

                if (!string.IsNullOrEmpty(ticket.Assigned))
                {
                    var assignedUser = await _context.Users
                        .FirstOrDefaultAsync(u => u.UserName == ticket.Assigned, cancellationToken);

                    if (assignedUser != null)
                    {
                        var existingParticipant = await _context.ProjectParticipants
                            .FirstOrDefaultAsync(pp => pp.ProjectId == ticket.ProjectId && pp.AppUserId == assignedUser.Id, cancellationToken);

                        if (existingParticipant == null)
                        {
                            var participant = new ProjectParticipant
                            {
                                ProjectId = ticket.ProjectId,
                                AppUserId = assignedUser.Id,
                                    Role = Roles.Project.User
                            };
                            _context.ProjectParticipants.Add(participant);
                        }
                    }
                }

                var success = await _context.SaveChangesAsync(cancellationToken) > 0;
                return success ? Result<Guid>.Success(ticket.Id)
                               : Result<Guid>.Failure("Failed to create ticket");
            }
        }
    }
}
