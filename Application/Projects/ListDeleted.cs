using Application.Core;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System.Linq;

namespace Application.Projects
{
    public class ListDeleted
    {
        public class Query : IRequest<Result<List<ProjectDto>>>
        {
        }

        public class Handler : IRequestHandler<Query, Result<List<ProjectDto>>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
            }

            public async Task<Result<List<ProjectDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var projects = await _context.Projects
                    .Include(p => p.Tickets)
                    .Where(p => p.IsDeleted)
                    .ToListAsync(cancellationToken);

                var projectDtos = _mapper.Map<List<ProjectDto>>(projects);
                return Result<List<ProjectDto>>.Success(projectDtos);
            }
        }
    }
}