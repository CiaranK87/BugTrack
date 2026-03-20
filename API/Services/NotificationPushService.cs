using Application.DTOs;
using Application.Interfaces;
using API.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace API.Services
{
    public class NotificationPushService : INotificationPushService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationPushService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task PushNotificationAsync(string userId, NotificationDto notification)
        {
            await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", notification);
        }

        public async Task PushUnreadCountUpdateAsync(string userId, int unreadCount)
        {
            await _hubContext.Clients.User(userId).SendAsync("ReceiveUnreadCount", unreadCount);
        }
    }
}
