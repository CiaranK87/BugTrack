using Application.Core;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Comments
{
    public class DeleteAttachment
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Guid CommentId { get; set; }
            public Guid AttachmentId { get; set; }
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

                var attachment = await _context.CommentAttachments
                    .Where(ca => ca.CommentId == request.CommentId && ca.Id == request.AttachmentId)
                    .FirstOrDefaultAsync(cancellationToken);

                if (attachment == null || attachment.UploadedById != userId)
                    return Result<Unit>.Failure("Attachment not found or access denied");

                await _fileService.DeleteAsync(attachment.FilePath, cancellationToken);

                _context.CommentAttachments.Remove(attachment);
                await _context.SaveChangesAsync(cancellationToken);

                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}