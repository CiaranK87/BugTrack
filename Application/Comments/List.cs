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

            var commentDtos = comments.Select(c => new CommentDto
            {
                Id = c.Id,
                Content = c.Content,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                TicketId = c.TicketId,
                AuthorId = c.AuthorId,
                AuthorUsername = c.Author?.UserName,
                AuthorDisplayName = c.Author?.DisplayName,
                Attachments = c.Attachments?.Select(a => new CommentAttachmentDto
                {
                    Id = a.Id,
                    FileName = a.FileName,
                    OriginalFileName = a.OriginalFileName,
                    ContentType = a.ContentType,
                    FileSize = a.FileSize,
                    UploadedAt = a.UploadedAt,
                    DownloadUrl = $"/api/tickets/{c.TicketId}/comments/{c.Id}/attachments/{a.Id}/download"
                }).ToList() ?? new List<CommentAttachmentDto>()
            }).ToList();

            return Result<List<CommentDto>>.Success(commentDtos);
        }
    }
}