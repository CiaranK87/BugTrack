using Application.Core;
using AutoMapper;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Projects
{
    public class Edit
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
        private readonly IMapper _mapper;
            public Handler(DataContext context, IMapper mapper)
            {
            _mapper = mapper;
            _context = context;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var project = await _context.Projects
                    .Include(p => p.Participants)
                    .FirstOrDefaultAsync(p => p.Id == request.Project.Id);

                if (project == null) return Result<Unit>.Failure("Project not found");

                project.ProjectTitle = request.Project.ProjectTitle;
                project.Description = request.Project.Description;
                project.StartDate = request.Project.StartDate;
                project.IsCancelled = request.Project.IsCancelled;
                project.ProjectOwner = request.Project.ProjectOwner;

                var result = await _context.SaveChangesAsync() > 0;

                if (!result) return Result<Unit>.Failure("Failed to update project");

                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}