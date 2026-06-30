using Application.Core;
using Application.DTOs;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Tickets
{
    public class ListByProjectId
    {
        public class Query : IRequest<Result<List<TicketDto>>>
        {
            public Guid ProjectId { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<TicketDto>>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<Result<List<TicketDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var tickets = await _context.Tickets
                    .Include(t => t.Project)
                    .Where(t => t.ProjectId == request.ProjectId && !t.IsDeleted)
                    .ToListAsync(cancellationToken);

                return Result<List<TicketDto>>.Success(_mapper.Map<List<TicketDto>>(tickets));
            }
        }
    }
}