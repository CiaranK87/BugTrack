using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
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
            _mapper = mapper;
            _context = context;
            }

            public async Task<Result<ProjectDto>> Handle(Query request, CancellationToken cancellationToken)
            {

                var project = await _context.Projects
                .Include(p => p.Tickets)
                .Include(p => p.Participants)
                    .ThenInclude(p => p.AppUser)
                .FirstOrDefaultAsync(x => x.Id == request.Id);

                return Result<ProjectDto>.Success(_mapper.Map<ProjectDto>(project));
            }
        }
    }
}