using Application.Core;
using AutoMapper;
using Domain;
using FluentValidation;
using MediatR;
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
                var project = await _context.Projects.FindAsync(request.Project.Id);

                if(project == null) return null;

                _mapper.Map(request.Project, project);

                var result = await _context.SaveChangesAsync() > 0;

                if(!result) return Result<Unit>.Failure("Failed to update project");

                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}