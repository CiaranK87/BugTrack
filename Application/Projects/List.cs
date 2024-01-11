using Application.Core;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Projects
{
    public class List
    {
        public class Query : IRequest<Result<List<Project>>> {}

        public class Handler : IRequestHandler<Query, Result<List<Project>>>
        {
        private readonly DataContext _context;
        private readonly ILogger<List> _logger;
            public Handler(DataContext context)
            {
            _context = context;
            }
            public async Task<Result<List<Project>>> Handle(Query request, CancellationToken cancellationToken)
            {
                return Result<List<Project>>.Success(await _context.Projects.ToListAsync());
            }
        }
    }
}