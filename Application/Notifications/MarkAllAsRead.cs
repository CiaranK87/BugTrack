using Application.Core;
using Application.Interfaces;
using MediatR;

namespace Application.Notifications
{
    public class MarkAllAsRead
    {
        public class Command : IRequest<Result<Unit>> { }

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
                await _notificationService.MarkAllAsReadAsync(userId);
                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}
