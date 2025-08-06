using Application.Core;
using Application.Projects;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles
{
    public class GetUserProjects
    {
        public class Query : IRequest<Result<List<ProjectDto>>>
        {
            public string Username { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<ProjectDto>>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<Result<List<ProjectDto>>> Handle(Query request, CancellationToken cancellationToken)
{
                var projects = await _context.Projects
                    .Include(p => p.Tickets)
                    .Where(p => p.ProjectOwner == request.Username || 
                            p.Participants.Any(pa => pa.AppUser.UserName == request.Username))
                    .ToListAsync(cancellationToken);

                var projectDtos = _mapper.Map<List<ProjectDto>>(projects);
                return Result<List<ProjectDto>>.Success(projectDtos);
            }
        }
    }
}