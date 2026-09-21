using Application.Core;
using Application.Interfaces;
using MediatR;

namespace Application.Notifications
{
    public class Delete
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly INotificationService _notificationService;
            private readonly IUserAccessor _userAccessor;

            public Handler(INotificationService notificationService, IUserAccessor userAccessor)
            {
                _notificationService = notificationService;
                _userAccessor = userAccessor;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var userId = _userAccessor.GetUserId();
                var success = await _notificationService.DeleteAsync(request.Id, userId);

                if (!success) return Result<Unit>.NotFound();

                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}
