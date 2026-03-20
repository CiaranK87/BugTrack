using System;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class Notification
    {
        public Guid Id { get; set; }
        
        [Required]
        public string RecipientId { get; set; }
        public AppUser Recipient { get; set; }
        public NotificationType Type { get; set; }
        
        [Required]
        public string Message { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ReadAt { get; set; }
        
        // Optional relationships for context
        public Guid? CommentId { get; set; }
        public Comment Comment { get; set; }
        
        public Guid? TicketId { get; set; }
        public Ticket Ticket { get; set; }
    }

    public enum NotificationType
    {
        Mention,
        CommentReply,
        TicketAssigned,
        TicketStatusChanged
    }
}
