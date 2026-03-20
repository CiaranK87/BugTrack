using Application.DTOs;
using Domain;

namespace Application.Interfaces
{
    public interface INotificationService
    {
        Task<Notification> CreateMentionNotificationAsync(string recipientId, Guid commentId, Guid ticketId, string authorDisplayName);
        Task<List<NotificationDto>> GetUserNotificationsAsync(string userId);
        Task<bool> MarkAsReadAsync(Guid notificationId, string userId);
        Task<bool> MarkAllAsReadAsync(string userId);
        Task<int> GetUnreadCountAsync(string userId);
        Task<bool> DeleteAsync(Guid notificationId, string userId);
        Task<int> DeleteAllAsync(string userId);
    }
}
