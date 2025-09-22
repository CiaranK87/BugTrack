using Application.Core;
using Application.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Projects
{
    public class ListParticipants
    {
        public class Query : IRequest<Result<List<ProjectParticipantDto>>>
        {
            public Guid ProjectId { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<ProjectParticipantDto>>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<Result<List<ProjectParticipantDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var project = await _context.Projects
                    .Include(p => p.Participants)
                        .ThenInclude(p => p.AppUser)
                    .FirstOrDefaultAsync(p => p.Id == request.ProjectId, cancellationToken);

                if (project == null)
                    return Result<List<ProjectParticipantDto>>.Failure("Project not found");

                var participants = project.Participants
                    .Select(p => new ProjectParticipantDto
                    {
                        UserId = p.AppUserId,
                        Username = p.AppUser.UserName,
                        DisplayName = p.AppUser.DisplayName,
                        Email = p.AppUser.Email,
                        Role = p.Role,
                        IsOwner = p.IsOwner
                    })
                    .ToList();

                return Result<List<ProjectParticipantDto>>.Success(participants);
            }
        }
    }
}