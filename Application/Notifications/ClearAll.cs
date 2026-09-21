using Application.Core;
using Application.Interfaces;
using MediatR;

namespace Application.Notifications
{
    public class ClearAll
    {
        public class Command : IRequest<Result<int>> { }

        public class Handler : IRequestHandler<Command, Result<int>>
        {
            private readonly INotificationService _notificationService;
            private readonly IUserAccessor _userAccessor;

            public Handler(INotificationService notificationService, IUserAccessor userAccessor)
            {
                _notificationService = notificationService;
                _userAccessor = userAccessor;
            }

            public async Task<Result<int>> Handle(Command request, CancellationToken cancellationToken)
            {
                var userId = _userAccessor.GetUserId();
                var count = await _notificationService.DeleteAllAsync(userId);
                return Result<int>.Success(count);
            }
        }
    }
}
