using System;
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
    public class CommentDetailsQuery : IRequest<Result<CommentDto>>
    {
        public Guid TicketId { get; set; }
        public Guid Id { get; set; }
    }

    public class CommentDetailsHandler : IRequestHandler<CommentDetailsQuery, Result<CommentDto>>
    {
        private readonly DataContext _context;

        public CommentDetailsHandler(DataContext context)
        {
            _context = context;
        }

        public async Task<Result<CommentDto>> Handle(CommentDetailsQuery request, CancellationToken cancellationToken)
        {
            var comment = await _context.Comments
                .Where(c => c.TicketId == request.TicketId && c.Id == request.Id)
                .Include(c => c.Author)
                .Include(c => c.Attachments)
                .FirstOrDefaultAsync(cancellationToken);

            if (comment == null)
                return Result<CommentDto>.Failure("Comment not found");

            var commentDto = new CommentDto
            {
                Id = comment.Id,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt,
                TicketId = comment.TicketId,
                AuthorId = comment.AuthorId,
                AuthorUsername = comment.Author?.UserName,
                AuthorDisplayName = comment.Author?.DisplayName,
                Attachments = comment.Attachments?.Select(a => new CommentAttachmentDto
                {
                    Id = a.Id,
                    FileName = a.FileName,
                    OriginalFileName = a.OriginalFileName,
                    ContentType = a.ContentType,
                    FileSize = a.FileSize,
                    UploadedAt = a.UploadedAt,
                    DownloadUrl = $"/api/comments/attachments/{a.Id}/download"
                }).ToList() ?? new List<CommentAttachmentDto>()
            };

            return Result<CommentDto>.Success(commentDto);
        }
    }
}