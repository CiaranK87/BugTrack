using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Projects
{
    public class List
    {
        public class Query : IRequest<Result<List<ProjectDto>>> {}

        public class Handler : IRequestHandler<Query, Result<List<ProjectDto>>>
        {
        private readonly DataContext _context;
        private readonly ILogger<List> _logger;
        private readonly IMapper _mapper;
            public Handler(DataContext context, IMapper mapper)
            {
            _mapper = mapper;
            _context = context;
            }
            public async Task<Result<List<ProjectDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var projects = await _context.Projects
                    .ProjectTo<ProjectDto>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);

                return Result<List<ProjectDto>>.Success(projects);
            }
        }
    }
}