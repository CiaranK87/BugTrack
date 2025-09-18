using Application.Core;
using Application.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Projects
{
    public class ListMembers
    {
        public class Query : IRequest<Result<List<ProjectMemberDto>>>
        {
            public Guid ProjectId { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<ProjectMemberDto>>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<Result<List<ProjectMemberDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var project = await _context.Projects
                    .Include(p => p.Participants)
                        .ThenInclude(p => p.AppUser)
                    .FirstOrDefaultAsync(p => p.Id == request.ProjectId, cancellationToken);

                if (project == null)
                    return Result<List<ProjectMemberDto>>.Failure("Project not found");

                var members = project.Participants
                    .Select(p => new ProjectMemberDto
                    {
                        UserId = p.AppUserId,
                        Username = p.AppUser.UserName,
                        DisplayName = p.AppUser.DisplayName,
                        Email = p.AppUser.Email,
                        Role = p.Role,
                        IsOwner = p.IsOwner
                    })
                    .ToList();

                return Result<List<ProjectMemberDto>>.Success(members);
            }
        }
    }
}