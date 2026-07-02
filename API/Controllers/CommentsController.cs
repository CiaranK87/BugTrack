using API.Authorization;
using API.Helpers;
using API.Hubs;
using Application.Comments;
using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace API.Controllers
{
    [ApiController]
    [Route("api/tickets/{ticketId}/comments")]
    [Authorize]
    public class CommentsController : BaseApiController
    {
        private readonly IHubContext<TicketCommentHub> _hubContext;

        public CommentsController(IMediator mediator, IHubContext<TicketCommentHub> hubContext, IAuthorizationService authorizationService)
            : base(mediator, authorizationService)
        {
            _hubContext = hubContext;
        }

        [HttpGet]
        public async Task<ActionResult<List<CommentDto>>> GetComments(Guid ticketId)
        {
            var authResult = await _authorizationService.AuthorizeAsync(User, ticketId, new TicketOperationRequirement(TicketOperation.Read));
            if (!authResult.Succeeded) return Forbid();

            var result = await Mediator.Send(new CommentList.Query { TicketId = ticketId });
            return HandleResult(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CommentDto>> GetComment(Guid ticketId, Guid id)
        {
            var authResult = await _authorizationService.AuthorizeAsync(User, ticketId, new TicketOperationRequirement(TicketOperation.Read));
            if (!authResult.Succeeded) return Forbid();

            var comment = await Mediator.Send(new CommentDetails.Query { TicketId = ticketId, Id = id });
            return HandleResult(comment);
        }

        [HttpPost]
        public async Task<ActionResult<CommentDto>> CreateComment(Guid ticketId, [FromForm] CreateCommentDto commentDto)
        {
            var authResult = await _authorizationService.AuthorizeAsync(
                User, ticketId, new TicketOperationRequirement(TicketOperation.Read));
            if (!authResult.Succeeded) return Forbid();

            if (commentDto.Attachments != null)
            {
                var invalid = commentDto.Attachments
                    .FirstOrDefault(f => !ContentTypeHelper.IsAllowed(Path.GetExtension(f.FileName)));
                if (invalid != null)
                    return BadRequest($"File type '{Path.GetExtension(invalid.FileName)}' is not allowed.");
            }

            var attachments = commentDto.Attachments?.Select(f => new FileUploadDto
            {
                Content = f.OpenReadStream(),
                FileName = f.FileName,
                ContentType = ContentTypeHelper.FromExtension(Path.GetExtension(f.FileName)),
                Length = f.Length
            }).ToList() ?? new();

            var result = await Mediator.Send(new Create.Command {
                TicketId = ticketId,
                Content = commentDto.Content,
                Attachments = attachments,
                ParentCommentId = commentDto.ParentCommentId
            });

            if (result.IsSuccess)
            {
                await _hubContext.Clients.Group($"Ticket_{ticketId}").SendAsync("ReceiveComment", result.Value);
                return CreatedAtAction(nameof(GetComment), new { ticketId, id = result.Value.Id }, result.Value);
            }

            return BadRequest(result.Error);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CommentDto>> UpdateComment(Guid ticketId, Guid id, UpdateCommentDto commentDto)
        {
            var authResult = await _authorizationService.AuthorizeAsync(
                User, ticketId, new TicketOperationRequirement(TicketOperation.Read));
            if (!authResult.Succeeded) return Forbid();

            var result = await Mediator.Send(new Update.Command { TicketId = ticketId, Id = id, Content = commentDto.Content });

            if (result.IsNotFound || (result.IsSuccess && result.Value == null))
                return NotFound();

            if (result.IsSuccess)
            {
                await _hubContext.Clients.Group($"Ticket_{ticketId}").SendAsync("CommentUpdated", result.Value);
                return Ok(result.Value);
            }

            return BadRequest(result.Error);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(Guid ticketId, Guid id)
        {
            var authResult = await _authorizationService.AuthorizeAsync(
                User, ticketId, new TicketOperationRequirement(TicketOperation.Read));
            if (!authResult.Succeeded) return Forbid();

            var result = await Mediator.Send(new Delete.Command { TicketId = ticketId, Id = id });

            if (result.IsNotFound)
                return NotFound();

            if (result.IsSuccess)
            {
                await _hubContext.Clients.Group($"Ticket_{ticketId}").SendAsync("CommentDeleted", id);
                return NoContent();
            }

            return BadRequest(result.Error);
        }

        [HttpGet("{commentId}/attachments/{attachmentId}/download")]
        public async Task<IActionResult> GetAttachment(Guid ticketId, Guid commentId, Guid attachmentId)
        {
            var authResult = await _authorizationService.AuthorizeAsync(User, ticketId, new TicketOperationRequirement(TicketOperation.Read));
            if (!authResult.Succeeded) return Forbid();

            var (stream, contentType, originalFileName) = await Mediator.Send(new GetAttachment.Query { TicketId = ticketId, CommentId = commentId, AttachmentId = attachmentId });

            if (stream == null) return NotFound();

            return File(stream, contentType, originalFileName);
        }

        [HttpDelete("{commentId}/attachments/{attachmentId}")]
        public async Task<IActionResult> DeleteAttachment(Guid ticketId, Guid commentId, Guid attachmentId)
        {
            var authResult = await _authorizationService.AuthorizeAsync(User, ticketId, new TicketOperationRequirement(TicketOperation.Read));
            if (!authResult.Succeeded) return Forbid();

            var result = await Mediator.Send(new DeleteAttachment.Command { TicketId = ticketId, CommentId = commentId, AttachmentId = attachmentId });

            if (result.IsSuccess)
            {
                await _hubContext.Clients.Group($"Ticket_{ticketId}").SendAsync("AttachmentDeleted", commentId, attachmentId);
                return NoContent();
            }

            return BadRequest(result.Error);
        }
    }
}
