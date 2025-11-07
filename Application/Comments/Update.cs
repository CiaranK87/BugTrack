using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Comments
{
    public class Update
    {
        public class Command : IRequest<Result<CommentDto>>
        {
            public Guid Id { get; set; }
            public string Content { get; set; }
            public Guid TicketId { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Id).NotEmpty().WithMessage("Comment ID is required");
                RuleFor(x => x.Content).NotEmpty().WithMessage("Comment content is required");
                RuleFor(x => x.TicketId).NotEmpty().WithMessage("Ticket ID is required");
            }
        }

        public class Handler : IRequestHandler<Command, Result<CommentDto>>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;

            public Handler(DataContext context, IUserAccessor userAccessor)
            {
                _context = context;
                _userAccessor = userAccessor;
            }

            public async Task<Result<CommentDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                var userId = _userAccessor.GetUserId();
                
                var comment = await _context.Comments
                    .Where(c => c.TicketId == request.TicketId && c.Id == request.Id)
                    .Include(c => c.Author)
                    .Include(c => c.Attachments)
                    .FirstOrDefaultAsync(cancellationToken);

                if (comment == null || comment.AuthorId != userId)
                    return Result<CommentDto>.Failure("Comment not found or access denied");

                comment.Content = request.Content;
                comment.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync(cancellationToken);

                return Result<CommentDto>.Success(new CommentDto
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
                });
            }
        }
    }
}