using System;
using System.Threading;
using System.Threading.Tasks;
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

            public Handler(DataContext context, IUserAccessor userAccessor)
            {
                _context = context;
                _userAccessor = userAccessor;
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

                if (comment.Replies.Any())
                    return Result<Unit>.Failure("Cannot delete a comment that has replies");

                if (comment.Attachments.Any())
                {
                    _context.CommentAttachments.RemoveRange(comment.Attachments);
                }

                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync(cancellationToken);

                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}