using System;
using System.Collections.Generic;
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
    public class List : IRequest<Result<List<CommentDto>>>
    {
        public Guid TicketId { get; set; }
    }

    public class Handler : IRequestHandler<List, Result<List<CommentDto>>>
    {
        private readonly DataContext _context;

        public Handler(DataContext context)
        {
            _context = context;
        }

        public async Task<Result<List<CommentDto>>> Handle(List request, CancellationToken cancellationToken)
        {
            var comments = await _context.Comments
                .Where(c => c.TicketId == request.TicketId)
                .Include(c => c.Author)
                .Include(c => c.Attachments)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync(cancellationToken);

            // Build parent-child relationships
            var rootComments = comments.Where(c => c.ParentCommentId == null).ToList();
            var allReplies = comments.Where(c => c.ParentCommentId != null).ToList();

            var commentDtos = rootComments.Select(rootComment => new CommentDto
            {
                Id = rootComment.Id,
                Content = rootComment.Content,
                CreatedAt = rootComment.CreatedAt,
                UpdatedAt = rootComment.UpdatedAt,
                TicketId = rootComment.TicketId,
                AuthorId = rootComment.AuthorId,
                AuthorUsername = rootComment.Author?.UserName,
                AuthorDisplayName = rootComment.Author?.DisplayName,
                ParentCommentId = rootComment.ParentCommentId,
                Replies = allReplies.Where(r => r.ParentCommentId == rootComment.Id)
                    .Select(reply => new CommentDto
                    {
                        Id = reply.Id,
                        Content = reply.Content,
                        CreatedAt = reply.CreatedAt,
                        UpdatedAt = reply.UpdatedAt,
                        TicketId = reply.TicketId,
                        AuthorId = reply.AuthorId,
                        AuthorUsername = reply.Author?.UserName,
                        AuthorDisplayName = reply.Author?.DisplayName,
                        ParentCommentId = reply.ParentCommentId,
                        Attachments = reply.Attachments?.Select(a => new CommentAttachmentDto
                        {
                            Id = a.Id,
                            FileName = a.FileName,
                            OriginalFileName = a.OriginalFileName,
                            ContentType = a.ContentType,
                            FileSize = a.FileSize,
                            UploadedAt = a.UploadedAt,
                            DownloadUrl = $"/api/tickets/{reply.TicketId}/comments/{reply.Id}/attachments/{a.Id}/download"
                        }).ToList() ?? new List<CommentAttachmentDto>()
                    }).ToList(),
                Attachments = rootComment.Attachments?.Select(a => new CommentAttachmentDto
                {
                    Id = a.Id,
                    FileName = a.FileName,
                    OriginalFileName = a.OriginalFileName,
                    ContentType = a.ContentType,
                    FileSize = a.FileSize,
                    UploadedAt = a.UploadedAt,
                    DownloadUrl = $"/api/tickets/{rootComment.TicketId}/comments/{rootComment.Id}/attachments/{a.Id}/download"
                }).ToList() ?? new List<CommentAttachmentDto>()
            }).ToList();

            return Result<List<CommentDto>>.Success(commentDtos);
        }
    }
}