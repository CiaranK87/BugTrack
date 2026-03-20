using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using Domain;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly DataContext _context;

        public NotificationService(DataContext context)
        {
            _context = context;
        }

        public async Task<Notification> CreateMentionNotificationAsync(string recipientId, Guid commentId, Guid ticketId, string authorDisplayName)
        {
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                RecipientId = recipientId,
                Type = NotificationType.Mention,
                Message = $"{authorDisplayName} mentioned you in a comment",
                CreatedAt = DateTime.UtcNow,
                CommentId = commentId,
                TicketId = ticketId
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            return notification;
        }

        public async Task<List<NotificationDto>> GetUserNotificationsAsync(string userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.RecipientId == userId)
                .Include(n => n.Comment)
                    .ThenInclude(c => c.Author)
                .Include(n => n.Ticket)
                .OrderByDescending(n => n.CreatedAt)
                .Take(5)
                .Select(n => new NotificationDto
                {
                    Id = n.Id,
                    RecipientId = n.RecipientId,
                    Type = n.Type,
                    Message = n.Message,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt,
                    ReadAt = n.ReadAt,
                    CommentId = n.CommentId,
                    TicketId = n.TicketId,
                    TicketTitle = n.Ticket != null ? n.Ticket.Title : null,
                    AuthorDisplayName = n.Comment != null && n.Comment.Author != null ? n.Comment.Author.DisplayName : null,
                    AuthorUsername = n.Comment != null && n.Comment.Author != null ? n.Comment.Author.UserName : null
                })
                .ToListAsync();

            return notifications;
        }

        public async Task<bool> MarkAsReadAsync(Guid notificationId, string userId)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.RecipientId == userId);

            if (notification == null)
                return false;

            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkAllAsReadAsync(string userId)
        {
            var unreadNotifications = await _context.Notifications
                .Where(n => n.RecipientId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetUnreadCountAsync(string userId)
        {
            return await _context.Notifications
                .CountAsync(n => n.RecipientId == userId && !n.IsRead);
        }

        public async Task<bool> DeleteAsync(Guid notificationId, string userId)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.RecipientId == userId);

            if (notification == null)
                return false;

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> DeleteAllAsync(string userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.RecipientId == userId)
                .ToListAsync();

            _context.Notifications.RemoveRange(notifications);
            var deleted = await _context.SaveChangesAsync();
            return notifications.Count;
        }
    }
}
