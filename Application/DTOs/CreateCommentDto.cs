using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Application.DTOs
{
    public class CreateCommentDto
    {
        public string Content { get; set; }
        public List<IFormFile> Attachments { get; set; } = new List<IFormFile>();
        public Guid? ParentCommentId { get; set; }
    }
}