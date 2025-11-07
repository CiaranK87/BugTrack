using System;

namespace Application.DTOs
{
    public class CommentDto
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid TicketId { get; set; }
        public string AuthorId { get; set; }
        public string AuthorUsername { get; set; }
        public string AuthorDisplayName { get; set; }
        public List<CommentAttachmentDto> Attachments { get; set; } = new List<CommentAttachmentDto>();
    }
}