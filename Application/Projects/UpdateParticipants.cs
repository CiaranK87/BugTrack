using Application.Core;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Projects
{
    public class UpdateParticipants
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
        private readonly DataContext _context;
        private readonly IUserAccessor _userAccessor;

            public Handler(DataContext context, IUserAccessor userAccessor)
            {
            _userAccessor = userAccessor;
            _context = context;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var project = await _context.Projects
                    .Include(p => p.Participants).ThenInclude(u => u.AppUser)
                    .FirstOrDefaultAsync(x => x.Id == request.Id);

                if(project == null) return null;

                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());

                if(user == null) return null;

                var OwnerUsername = project.Participants.FirstOrDefault(x => x.IsOwner)?.AppUser?.UserName;

                var participants = project.Participants.FirstOrDefault(x => x.AppUser.UserName == user.UserName);

                if(participants != null && OwnerUsername == user.UserName)
                    project.IsCancelled = !project.IsCancelled;

                if(participants != null && OwnerUsername != user.UserName)
                    project.Participants.Remove(participants);

                if(participants == null )
                {
                    participants = new ProjectParticipant
                    {
                        AppUser = user,
                        Project = project,
                        IsOwner = false,
                    };

                    project.Participants.Add(participants);
                }

                var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                return result ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure("Problem updating participant");
            }
        }
    }
}