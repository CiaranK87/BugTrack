using Application.Core;
using Application.DTOs;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Tickets
{
    public class Details
    {
        public class Query : IRequest<Result<TicketDto>>
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<TicketDto>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<Result<TicketDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var ticket = await _context.Tickets
                    .Include(t => t.Project)
                    .FirstOrDefaultAsync(t => t.Id == request.Id && !t.IsDeleted);

                return Result<TicketDto>.Success(_mapper.Map<TicketDto>(ticket));
            }
        }
    }
}