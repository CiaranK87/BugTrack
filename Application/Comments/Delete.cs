using Application.Core;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Comments
{
    public class Delete
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Guid Id { get; set; }
            public Guid TicketId { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;
            private readonly IFileService _fileService;

            public Handler(DataContext context, IUserAccessor userAccessor, IFileService fileService)
            {
                _context = context;
                _userAccessor = userAccessor;
                _fileService = fileService;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var userId = _userAccessor.GetUserId();
                
                var comment = await _context.Comments
                    .Include(c => c.Replies)
                    .Include(c => c.Attachments)
                    .Where(c => c.TicketId == request.TicketId && c.Id == request.Id)
                    .FirstOrDefaultAsync(cancellationToken);

                if (comment == null || comment.AuthorId != userId)
                    return Result<Unit>.Failure("Comment not found or access denied");

                // Check if comment has replies
                var hasReplies = comment.Replies != null && comment.Replies.Any();

                if (comment.Attachments.Any())
                {
                    foreach (var attachment in comment.Attachments)
                        await _fileService.DeleteAsync(attachment.FilePath, cancellationToken);

                    _context.CommentAttachments.RemoveRange(comment.Attachments);
                }

                if (hasReplies)
                {
                    comment.IsDeleted = true;
                    comment.DeletedAt = DateTime.UtcNow;
                    comment.Content = "[Deleted]";
                }
                else
                {
                    _context.Comments.Remove(comment);
                }

                await _context.SaveChangesAsync(cancellationToken);

                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}
