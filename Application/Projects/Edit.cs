using AutoMapper;
using Domain;
using MediatR;
using Persistence;

namespace Application.Projects
{
    public class Edit
    {
        public class Command : IRequest
        {
            public Project Project { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
            public Handler(DataContext context, IMapper mapper)
            {
            _mapper = mapper;
            _context = context;
            }

            public async Task Handle(Command request, CancellationToken cancellationToken)
            {
                var project = await _context.Projects.FindAsync(request.Project.Id);

                _mapper.Map(request.Project, project);

                await _context.SaveChangesAsync();


            }
        }
    }
}