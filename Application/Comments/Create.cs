using Application.Core;
using Application.DTOs;
using Application.Helpers;
using Application.Interfaces;
using Application.Services;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Comments
{
    public class Create
    {
        public class Command : IRequest<Result<CommentDto>>
        {
            public string Content { get; set; }
            public Guid TicketId { get; set; }
            public Guid? ParentCommentId { get; set; }
            public List<FileUploadDto> Attachments { get; set; } = new List<FileUploadDto>();
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
            private readonly INotificationService _notificationService;
            private readonly INotificationPushService _notificationPushService;
            private readonly ILogger<Handler> _logger;
            private readonly IFileService _fileService;

            private const long MaxFileSizeBytes = 10 * 1024 * 1024;

            public Handler(DataContext context, IUserAccessor userAccessor, INotificationService notificationService, INotificationPushService notificationPushService, ILogger<Handler> logger, IFileService fileService)
            {
                _context = context;
                _userAccessor = userAccessor;
                _notificationService = notificationService;
                _notificationPushService = notificationPushService;
                _logger = logger;
                _fileService = fileService;
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
                    ParentCommentId = request.ParentCommentId,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Comments.Add(comment);

                if (request.Attachments != null && request.Attachments.Count > 0)
                {
                    if (_userAccessor.GetGlobalRole() == Roles.Global.Guest)
                        return Result<CommentDto>.Failure("File uploads are not available for Guest users.");

                    foreach (var file in request.Attachments)
                    {
                        if (file.Length > MaxFileSizeBytes)
                            return Result<CommentDto>.Failure($"'{file.FileName}' exceeds the 10 MB size limit.");
                    }

                    foreach (var file in request.Attachments)
                    {
                        var attachment = await CreateAttachmentFromFile(file, comment.Id, userId, cancellationToken);
                        if (attachment != null)
                            _context.CommentAttachments.Add(attachment);
                    }
                }

                var success = await _context.SaveChangesAsync() > 0;

                if (!success)
                    return Result<CommentDto>.Failure("Failed to create comment");

                await ProcessMentionsAsync(comment, userId);

                await _context.Entry(comment)
                    .Collection(c => c.Attachments)
                    .Query()
                    .Include(a => a.UploadedBy)
                    .LoadAsync();

                await _context.Entry(comment)
                    .Reference(c => c.Author)
                    .LoadAsync();

                return Result<CommentDto>.Success(MapToCommentDto(comment));
            }

            private async Task ProcessMentionsAsync(Comment comment, string authorId)
            {
                var mentions = MentionHelper.ExtractMentions(comment.Content);

                if (mentions.Count == 0)
                    return;

                var author = await _context.Users.FindAsync(authorId);
                var authorDisplayName = author?.DisplayName ?? author?.UserName ?? "Someone";

                var ticket = await _context.Tickets.FindAsync(comment.TicketId);

                var lowerMentions = mentions.Select(m => m.ToLower()).ToList();

                var mentionedUsers = await _context.Users
                    .Where(u => u.Id != authorId && !u.IsDeleted)
                    .Where(u => lowerMentions.Contains(u.UserName.Replace(" ", "").ToLower()))
                    .Where(u => u.GlobalRole == Roles.Global.Admin ||
                               _context.ProjectParticipants.Any(pp => pp.AppUserId == u.Id && pp.ProjectId == ticket.ProjectId) ||
                               _context.Projects.Any(p => p.Id == ticket.ProjectId && p.ProjectOwner == u.UserName) ||
                               _context.Tickets.Any(t => t.ProjectId == ticket.ProjectId && (t.Assigned == u.UserName || t.Submitter == u.UserName)))
                    .ToListAsync();

                foreach (var user in mentionedUsers)
                {
                    try
                    {
                        var notification = await _notificationService.CreateMentionNotificationAsync(
                            user.Id,
                            comment.Id,
                            comment.TicketId,
                            authorDisplayName
                        );

                        var notificationDto = new NotificationDto
                        {
                            Id = notification.Id,
                            RecipientId = notification.RecipientId,
                            Type = notification.Type,
                            Message = notification.Message,
                            IsRead = notification.IsRead,
                            CreatedAt = notification.CreatedAt,
                            ReadAt = notification.ReadAt,
                            CommentId = notification.CommentId,
                            TicketId = notification.TicketId,
                            TicketTitle = ticket?.Title,
                            AuthorDisplayName = authorDisplayName,
                            AuthorUsername = author?.UserName
                        };

                        await _notificationPushService.PushNotificationAsync(user.Id, notificationDto);

                        var unreadCount = await _notificationService.GetUnreadCountAsync(user.Id);
                        await _notificationPushService.PushUnreadCountUpdateAsync(user.Id, unreadCount);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing mention notification for user {UserId}", user.Id);
                    }
                }
            }

            private async Task<CommentAttachment> CreateAttachmentFromFile(FileUploadDto file, Guid commentId, string userId, CancellationToken cancellationToken)
            {
                if (file == null || file.Length == 0)
                    return null;

                var blobName = await _fileService.UploadAsync(file, cancellationToken);

                return new CommentAttachment
                {
                    Id = Guid.NewGuid(),
                    FileName = blobName,
                    OriginalFileName = file.FileName,
                    ContentType = file.ContentType,
                    FileSize = file.Length,
                    FilePath = blobName,
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
                    AuthorImage = comment.Author?.AvatarBlobName != null ? AvatarUrl.For(comment.Author.UserName) : null,
                    ParentCommentId = comment.ParentCommentId,
                    Attachments = comment.Attachments?.Select(a => MapToCommentAttachmentDto(a, comment.TicketId, comment.Id)).ToList() ?? new List<CommentAttachmentDto>()
                };
            }

            private static CommentAttachmentDto MapToCommentAttachmentDto(CommentAttachment attachment, Guid ticketId, Guid commentId)
            {
                return new CommentAttachmentDto
                {
                    Id = attachment.Id,
                    FileName = attachment.FileName,
                    OriginalFileName = attachment.OriginalFileName,
                    ContentType = attachment.ContentType,
                    FileSize = attachment.FileSize,
                    UploadedAt = attachment.UploadedAt,
                    DownloadUrl = $"/api/tickets/{ticketId}/comments/{commentId}/attachments/{attachment.Id}/download"
                };
            }
        }
    }
}
