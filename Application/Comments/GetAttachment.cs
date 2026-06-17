using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Comments
{
    public class GetAttachmentQuery : IRequest<(Stream FileStream, string ContentType, string OriginalFileName)>
    {
        public Guid TicketId { get; set; }
        public Guid CommentId { get; set; }
        public Guid AttachmentId { get; set; }
    }

    public class GetAttachmentHandler : IRequestHandler<GetAttachmentQuery, (Stream FileStream, string ContentType, string OriginalFileName)>
    {
        private readonly DataContext _context;
        private readonly IFileService _fileService;

        public GetAttachmentHandler(DataContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        public async Task<(Stream FileStream, string ContentType, string OriginalFileName)> Handle(GetAttachmentQuery request, CancellationToken cancellationToken)
        {
            var attachment = await _context.CommentAttachments
                .Include(ca => ca.Comment)
                .Where(ca => ca.Comment.TicketId == request.TicketId
                          && ca.CommentId == request.CommentId
                          && ca.Id == request.AttachmentId)
                .FirstOrDefaultAsync(cancellationToken);

            if (attachment == null)
                return (null, null, null);

            var stream = await _fileService.DownloadAsync(attachment.FilePath, cancellationToken);
            return (stream, attachment.ContentType, attachment.OriginalFileName);
        }
    }
}
