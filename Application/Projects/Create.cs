using Application.Core;
using Application.Interfaces;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;

namespace Application.Projects
{
    public class Create
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Project Project { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Project).SetValidator(new ProjectValidator());

            }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
        private readonly DataContext _context;
        private readonly IUserAccessor _userAccessor;
            public Handler(DataContext context, IUserAccessor userAccessor)
            {
            _userAccessor = userAccessor;
            _context = context;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());

                if (user != null)
                {
                    request.Project.ProjectOwner = string.IsNullOrWhiteSpace(user.DisplayName) ? user.UserName : user.DisplayName;
                }


                var participant = new ProjectParticipant
                {
                    AppUser = user,
                    Project = request.Project,
                    IsOwner = true,
                    Role = "Owner"
                };

                request.Project.Participants.Add(participant);

                _context.ProjectParticipants.Add(participant);

                _context.Projects.Add(request.Project);

                var result = await _context.SaveChangesAsync() > 0;

                if(!result) return Result<Unit>.Failure("Failed to create project");

                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}