using System;

namespace Domain
{
    public class Comment
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid TicketId { get; set; }
        public Ticket Ticket { get; set; }
        public string AuthorId { get; set; }
        public AppUser Author { get; set; }
        public Guid? ParentCommentId { get; set; }
        public Comment ParentComment { get; set; }
        public ICollection<Comment> Replies { get; set; } = new List<Comment>();
        public ICollection<CommentAttachment> Attachments { get; set; } = new List<CommentAttachment>();
    }
}