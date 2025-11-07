using System;

namespace Domain
{
    public class CommentAttachment
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string OriginalFileName { get; set; }
        public string ContentType { get; set; }
        public long FileSize { get; set; }
        public string FilePath { get; set; }
        public DateTime UploadedAt { get; set; }
        public Guid CommentId { get; set; }
        public Comment Comment { get; set; }
        public string UploadedById { get; set; }
        public AppUser UploadedBy { get; set; }
    }
}