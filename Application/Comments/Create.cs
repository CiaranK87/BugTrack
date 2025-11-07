using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Persistence;

namespace Application.Comments
{
    public class Create
    {
        public class Command : IRequest<Result<CommentDto>>
        {
            public string Content { get; set; }
            public Guid TicketId { get; set; }
            public List<IFormFile> Attachments { get; set; } = new List<IFormFile>();
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Content).NotEmpty().WithMessage("Comment content is required");
                RuleFor(x => x.TicketId).NotEmpty().WithMessage("Ticket ID is required");
            }
        }

        public class Handler : IRequestHandler<Command, Result<CommentDto>>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;
            private readonly IConfiguration _config;

            public Handler(DataContext context, IUserAccessor userAccessor, IConfiguration config)
            {
                _context = context;
                _userAccessor = userAccessor;
                _config = config;
            }

            public async Task<Result<CommentDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                var userId = _userAccessor.GetUserId();
                
                var comment = new Comment
                {
                    Id = Guid.NewGuid(),
                    Content = request.Content,
                    TicketId = request.TicketId,
                    AuthorId = userId,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Comments.Add(comment);
                
                // Handle attachments if any
                if (request.Attachments != null && request.Attachments.Count > 0)
                {
                    foreach (var file in request.Attachments)
                    {
                        var attachment = await CreateAttachmentFromFile(file, comment.Id, userId);
                        if (attachment != null)
                        {
                            _context.CommentAttachments.Add(attachment);
                        }
                    }
                }

                var success = await _context.SaveChangesAsync() > 0;
                 
                if (!success)
                    return Result<CommentDto>.Failure("Failed to create comment");
                
                // Reload the comment with all related data
                await _context.Entry(comment)
                    .Collection(c => c.Attachments)
                    .Query()
                    .Include(a => a.UploadedBy)
                    .LoadAsync();
                
                // Load the Author data
                await _context.Entry(comment)
                    .Reference(c => c.Author)
                    .LoadAsync();
                
                return Result<CommentDto>.Success(MapToCommentDto(comment));
            }

            private async Task<CommentAttachment> CreateAttachmentFromFile(IFormFile file, Guid commentId, string userId)
            {
                if (file == null || file.Length == 0)
                    return null;

                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return new CommentAttachment
                {
                    Id = Guid.NewGuid(),
                    FileName = uniqueFileName,
                    OriginalFileName = file.FileName,
                    ContentType = file.ContentType,
                    FileSize = file.Length,
                    FilePath = uniqueFileName,
                    UploadedAt = DateTime.UtcNow,
                    CommentId = commentId,
                    UploadedById = userId
                };
            }

            private static CommentDto MapToCommentDto(Comment comment)
            {
                return new CommentDto
                {
                    Id = comment.Id,
                    Content = comment.Content,
                    CreatedAt = comment.CreatedAt,
                    UpdatedAt = comment.UpdatedAt,
                    TicketId = comment.TicketId,
                    AuthorId = comment.AuthorId,
                    AuthorUsername = comment.Author?.UserName,
                    AuthorDisplayName = comment.Author?.DisplayName,
                    Attachments = comment.Attachments?.Select(MapToCommentAttachmentDto).ToList() ?? new List<CommentAttachmentDto>()
                };
            }

            private static CommentAttachmentDto MapToCommentAttachmentDto(CommentAttachment attachment)
            {
                return new CommentAttachmentDto
                {
                    Id = attachment.Id,
                    FileName = attachment.FileName,
                    OriginalFileName = attachment.OriginalFileName,
                    ContentType = attachment.ContentType,
                    FileSize = attachment.FileSize,
                    UploadedAt = attachment.UploadedAt,
                    DownloadUrl = $"/api/comments/attachments/{attachment.Id}/download"
                };
            }
        }
    }
}