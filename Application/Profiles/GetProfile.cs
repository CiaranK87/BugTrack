using Application.Core;
using Application.DTOs;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles
{
    public class GetProfile
    {
        public class Query : IRequest<Result<ProfileDto>>
        {
            public string Username { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<ProfileDto>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<Result<ProfileDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(x => x.UserName == request.Username, cancellationToken);

                if (user == null) return null;

                var profile = _mapper.Map<ProfileDto>(user);
                return Result<ProfileDto>.Success(profile);
            }
        }
    }
}