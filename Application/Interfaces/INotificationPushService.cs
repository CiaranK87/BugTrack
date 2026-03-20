using Application.DTOs;

namespace Application.Interfaces
{
    public interface INotificationPushService
    {
        Task PushNotificationAsync(string userId, NotificationDto notification);
        Task PushUnreadCountUpdateAsync(string userId, int unreadCount);
    }
}
