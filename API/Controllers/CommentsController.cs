using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Application.Comments;
using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using API.Hubs;
using MediatR;

namespace API.Controllers
{
    [ApiController]
    [Route("api/tickets/{ticketId}/comments")]
    [Authorize]
    public class CommentsController : BaseApiController
    {
        private readonly IMediator _mediator;
        private readonly IHubContext<TicketCommentHub> _hubContext;

        public CommentsController(IMediator mediator, IHubContext<TicketCommentHub> hubContext, IAuthorizationService authorizationService)
            : base(authorizationService)
        {
            _mediator = mediator;
            _hubContext = hubContext;
        }

        [HttpGet]
        public async Task<ActionResult<List<CommentDto>>> GetComments(Guid ticketId)
        {
            var result = await _mediator.Send(new List { TicketId = ticketId });
            return HandleResult(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CommentDto>> GetComment(Guid ticketId, Guid id)
        {
            var comment = await _mediator.Send(new Application.Comments.CommentDetailsQuery { TicketId = ticketId, Id = id });
            return HandleResult(comment);
        }

        [HttpPost]
        public async Task<ActionResult<CommentDto>> CreateComment(Guid ticketId, [FromForm] CreateCommentDto commentDto)
        {
            var result = await _mediator.Send(new Create.Command { TicketId = ticketId, Content = commentDto.Content, Attachments = commentDto.Attachments });
            
            if (result.IsSuccess)
            {
                // Notify all clients in the ticket group about the new comment
                await _hubContext.Clients.Group($"Ticket_{ticketId}").SendAsync("ReceiveComment", result.Value);
                return CreatedAtAction(nameof(GetComment), new { ticketId, id = result.Value.Id }, result.Value);
            }
            
            return BadRequest(result.Error);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CommentDto>> UpdateComment(Guid ticketId, Guid id, UpdateCommentDto commentDto)
        {
            var result = await _mediator.Send(new Update.Command { TicketId = ticketId, Id = id, Content = commentDto.Content });
            
            if (result.IsSuccess)
            {
                // Notify all clients in the ticket group about the updated comment
                await _hubContext.Clients.Group($"Ticket_{ticketId}").SendAsync("CommentUpdated", result.Value);
                return Ok(result.Value);
            }
            
            return BadRequest(result.Error);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(Guid ticketId, Guid id)
        {
            var result = await _mediator.Send(new Delete.Command { TicketId = ticketId, Id = id });
            
            if (result.IsSuccess)
            {
                // Notify all clients in the ticket group about the deleted comment
                await _hubContext.Clients.Group($"Ticket_{ticketId}").SendAsync("CommentDeleted", id);
                return NoContent();
            }
            
            return BadRequest(result.Error);
        }

        [HttpPost("{commentId}/attachments")]
        public async Task<ActionResult<CommentAttachmentDto>> AddAttachment(Guid ticketId, Guid commentId, [FromForm] IFormFile file)
        {
            var result = await _mediator.Send(new AddAttachment.Command { CommentId = commentId, File = file });
            
            if (result.IsSuccess)
            {
                // Notify all clients in the ticket group about the new attachment
                await _hubContext.Clients.Group($"Ticket_{ticketId}").SendAsync("AttachmentAdded", commentId, result.Value);
                return CreatedAtAction(nameof(GetAttachment), new { ticketId, commentId, id = result.Value.Id }, result.Value);
            }
            
            return BadRequest(result.Error);
        }

        [HttpGet("{commentId}/attachments/{attachmentId}/download")]
        public async Task<IActionResult> GetAttachment(Guid ticketId, Guid commentId, Guid attachmentId)
        {
            var attachmentResult = await _mediator.Send(new Application.Comments.Query { TicketId = ticketId, CommentId = commentId, AttachmentId = attachmentId });
            
            if (attachmentResult.Attachment != null && System.IO.File.Exists(attachmentResult.FilePath))
            {
                var fileBytes = await System.IO.File.ReadAllBytesAsync(attachmentResult.FilePath);
                return File(fileBytes, attachmentResult.Attachment.ContentType, attachmentResult.Attachment.OriginalFileName);
            }
            
            return NotFound();
        }

        [HttpDelete("{commentId}/attachments/{attachmentId}")]
        public async Task<IActionResult> DeleteAttachment(Guid ticketId, Guid commentId, Guid attachmentId)
        {
            var result = await _mediator.Send(new DeleteAttachment.Command { TicketId = ticketId, CommentId = commentId, AttachmentId = attachmentId });
            
            if (result.IsSuccess)
            {
                // Notify all clients in the ticket group about the deleted attachment
                await _hubContext.Clients.Group($"Ticket_{ticketId}").SendAsync("AttachmentDeleted", commentId, attachmentId);
                return NoContent();
            }
            
            return BadRequest(result.Error);
        }
    }
}