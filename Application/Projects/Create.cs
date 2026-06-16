using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Projects
{
    public class Create
    {
        public class Command : IRequest<Result<Guid>>
        {
            public CreateProjectDto ProjectDto { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.ProjectDto.ProjectTitle).NotEmpty();
                RuleFor(x => x.ProjectDto.Description).NotEmpty();
                RuleFor(x => x.ProjectDto.StartDate).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Command, Result<Guid>>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;

            public Handler(DataContext context, IUserAccessor userAccessor)
            {
                _userAccessor = userAccessor;
                _context = context;
            }

            public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());

                var project = new Project
                {
                    ProjectTitle = request.ProjectDto.ProjectTitle,
                    Description = request.ProjectDto.Description,
                    StartDate = request.ProjectDto.StartDate,
                    ProjectOwner = user != null && !string.IsNullOrWhiteSpace(user.DisplayName)
                        ? user.DisplayName
                        : user?.UserName ?? string.Empty,
                    IsDeleted = false,
                    IsCancelled = false,
                    DeletedDate = null
                };

                var participant = new ProjectParticipant
                {
                    AppUser = user,
                    Project = project,
                    IsOwner = true,
                    Role = Roles.Project.Owner
                };

                project.Participants.Add(participant);

                _context.ProjectParticipants.Add(participant);
                _context.Projects.Add(project);

                var result = await _context.SaveChangesAsync() > 0;

                if (!result) return Result<Guid>.Failure("Failed to create project");

                return Result<Guid>.Success(project.Id);
            }
        }
    }
}