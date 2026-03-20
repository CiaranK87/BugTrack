using System;
using Domain;

namespace Application.DTOs
{
    public class NotificationDto
    {
        public Guid Id { get; set; }
        public string RecipientId { get; set; }
        public NotificationType Type { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ReadAt { get; set; }
        public Guid? CommentId { get; set; }
        public Guid? TicketId { get; set; }
        public string? TicketTitle { get; set; }
        public string? AuthorDisplayName { get; set; }
        public string? AuthorUsername { get; set; }
    }
}
