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
            public Guid Id { get; set; }
            public ProjectDto ProjectDto { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.ProjectDto).NotNull();
                RuleFor(x => x.ProjectDto.ProjectTitle).NotEmpty();
                RuleFor(x => x.ProjectDto.Description).MaximumLength(1000);
                RuleFor(x => x.ProjectDto.StartDate).NotEmpty();
            }
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
                var project = await _context.Projects
                    .Include(p => p.Participants)
                    .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

                if (project == null)
                    return Result<Unit>.Failure("Project not found");

                _mapper.Map(request.ProjectDto, project);

                var result = await _context.SaveChangesAsync(cancellationToken) > 0;
                if (!result)
                    return Result<Unit>.Failure("Failed to update project");

                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}
