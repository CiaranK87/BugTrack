using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
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
    public class AddAttachment
    {
        public class Command : IRequest<Result<CommentAttachmentDto>>
        {
            public Guid CommentId { get; set; }
            public IFormFile File { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.CommentId).NotEmpty().WithMessage("Comment ID is required");
                RuleFor(x => x.File).NotNull().WithMessage("File is required");
            }
        }

        public class Handler : IRequestHandler<Command, Result<CommentAttachmentDto>>
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

            public async Task<Result<CommentAttachmentDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                var userId = _userAccessor.GetUserId();
                
                var attachment = await CreateAttachmentFromFile(request.File, request.CommentId, userId);
                
                if (attachment == null)
                    return Result<CommentAttachmentDto>.Failure("Failed to upload attachment");

                _context.CommentAttachments.Add(attachment);
                await _context.SaveChangesAsync(cancellationToken);

                // Reload to get all related data
                await _context.Entry(attachment)
                    .Reference(a => a.UploadedBy)
                    .LoadAsync();

                return Result<CommentAttachmentDto>.Success(MapToCommentAttachmentDto(attachment));
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