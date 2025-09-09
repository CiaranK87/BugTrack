using Application.Core;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Projects
{
    public class Details
    {
        public class Query : IRequest<Result<ProjectDto>>
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<ProjectDto>>
            {
                private readonly DataContext _context;
                private readonly IMapper _mapper;

                public Handler(DataContext context, IMapper mapper)
                {
                    _context = context;
                    _mapper = mapper;
                }

                public async Task<Result<ProjectDto>> Handle(Query request, CancellationToken cancellationToken)
                {
                    var project = await _context.Projects
                        .Include(p => p.Tickets)
                        .Include(p => p.Participants)
                            .ThenInclude(pp => pp.AppUser)
                        .FirstOrDefaultAsync(x => x.Id == request.Id);

                    if (project == null) return null;

                    return Result<ProjectDto>.Success(_mapper.Map<ProjectDto>(project));
                }
}
    }
}