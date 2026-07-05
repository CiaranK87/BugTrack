using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Comments
{
    public class GetAttachment
    {
        public class Query : IRequest<(Stream FileStream, string ContentType, string OriginalFileName)>
        {
            public Guid TicketId { get; set; }
            public Guid CommentId { get; set; }
            public Guid AttachmentId { get; set; }
        }

        public class Handler : IRequestHandler<Query, (Stream FileStream, string ContentType, string OriginalFileName)>
        {
            private readonly DataContext _context;
            private readonly IFileService _fileService;

            public Handler(DataContext context, IFileService fileService)
            {
                _context = context;
                _fileService = fileService;
            }

            public async Task<(Stream FileStream, string ContentType, string OriginalFileName)> Handle(Query request, CancellationToken cancellationToken)
            {
                var attachment = await _context.CommentAttachments
                    .Include(ca => ca.Comment)
                    .Where(ca => ca.Comment.TicketId == request.TicketId
                              && ca.CommentId == request.CommentId
                              && ca.Id == request.AttachmentId)
                    .FirstOrDefaultAsync(cancellationToken);

                if (attachment == null)
                    return (null, null, null);

                var (stream, _) = await _fileService.DownloadAsync(attachment.FilePath, cancellationToken);
                if (stream == null) return (null, null, null);
                return (stream, attachment.ContentType, attachment.OriginalFileName);
            }
        }
    }
}
