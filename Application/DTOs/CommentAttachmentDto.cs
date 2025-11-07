using System;

namespace Application.DTOs
{
    public class CommentAttachmentDto
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string OriginalFileName { get; set; }
        public string ContentType { get; set; }
        public long FileSize { get; set; }
        public DateTime UploadedAt { get; set; }
        public string DownloadUrl { get; set; }
    }
}