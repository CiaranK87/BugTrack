using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using MediatR;

namespace Application.Notifications
{
    public class List
    {
        public class Query : IRequest<Result<List<NotificationDto>>> { }

        public class Handler : IRequestHandler<Query, Result<List<NotificationDto>>>
        {
            private readonly INotificationService _notificationService;
            private readonly IUserAccessor _userAccessor;

            public Handler(INotificationService notificationService, IUserAccessor userAccessor)
            {
                _notificationService = notificationService;
                _userAccessor = userAccessor;
            }

            public async Task<Result<List<NotificationDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var userId = _userAccessor.GetUserId();
                var notifications = await _notificationService.GetUserNotificationsAsync(userId);
                return Result<List<NotificationDto>>.Success(notifications);
            }
        }
    }
}
