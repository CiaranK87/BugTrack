using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Tickets
{
    public class List
    {
        public class Query : IRequest<Result<List<TicketDto>>> {}

        public class Handler : IRequestHandler<Query, Result<List<TicketDto>>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            private readonly IUserAccessor _userAccessor;

            public Handler(DataContext context, IMapper mapper, IUserAccessor userAccessor)
            {
                _context = context;
                _mapper = mapper;
                _userAccessor = userAccessor;
            }

            public async Task<Result<List<TicketDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var userId = _userAccessor.GetUserId();
                var globalRole = _userAccessor.GetGlobalRole();

                List<Domain.Ticket> tickets;

                if (globalRole == Roles.Global.Admin)
                {
                    tickets = await _context.Tickets
                        .Include(x => x.Project)
                        .ToListAsync(cancellationToken);
                }
                else
                {
                    tickets = await _context.Tickets
                        .Include(x => x.Project)
                        .Where(t => t.Project.Participants.Any(p => p.AppUserId == userId))
                        .ToListAsync(cancellationToken);
                }

                return Result<List<TicketDto>>.Success(_mapper.Map<List<TicketDto>>(tickets));
            }
        }
    }
}