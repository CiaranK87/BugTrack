using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Comments
{
    public class Query : IRequest<(CommentAttachmentDto Attachment, string FilePath)>
    {
        public Guid TicketId { get; set; }
        public Guid CommentId { get; set; }
        public Guid AttachmentId { get; set; }
    }

    public class GetAttachmentHandler : IRequestHandler<Query, (CommentAttachmentDto Attachment, string FilePath)>
    {
        private readonly DataContext _context;

        public GetAttachmentHandler(DataContext context)
        {
            _context = context;
        }

        public async Task<(CommentAttachmentDto Attachment, string FilePath)> Handle(Query request, CancellationToken cancellationToken)
        {
            var attachment = await _context.CommentAttachments
                .Where(ca => ca.CommentId == request.CommentId && ca.Id == request.AttachmentId)
                .FirstOrDefaultAsync(cancellationToken);

            if (attachment == null)
            {
                return (null, null);
            }

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            var filePath = Path.Combine(uploadsFolder, attachment.FilePath);

            var attachmentDto = new CommentAttachmentDto
            {
                Id = attachment.Id,
                FileName = attachment.FileName,
                OriginalFileName = attachment.OriginalFileName,
                ContentType = attachment.ContentType,
                FileSize = attachment.FileSize,
                UploadedAt = attachment.UploadedAt,
                DownloadUrl = $"/api/tickets/{request.TicketId}/comments/{request.CommentId}/attachments/{attachment.Id}/download"
            };

            return (attachmentDto, filePath);
        }
    }
}