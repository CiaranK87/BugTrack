using Application.Notifications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class NotificationsController : BaseApiController
    {
        public NotificationsController(IMediator mediator, IAuthorizationService authorizationService)
            : base(mediator, authorizationService) { }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetNotifications() =>
            HandleResult(await Mediator.Send(new List.Query()));

        [HttpGet("unread-count")]
        [Authorize]
        public async Task<IActionResult> GetUnreadCount() =>
            HandleResult(await Mediator.Send(new GetUnreadCount.Query()));

        [HttpPut("{id}/read")]
        [Authorize]
        public async Task<IActionResult> MarkAsRead(Guid id) =>
            HandleResult(await Mediator.Send(new MarkAsRead.Command { Id = id }));

        [HttpPut("read-all")]
        [Authorize]
        public async Task<IActionResult> MarkAllAsRead() =>
            HandleResult(await Mediator.Send(new MarkAllAsRead.Command()));

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteNotification(Guid id)
        {
            var result = await Mediator.Send(new Delete.Command { Id = id });
            if (!result.IsSuccess) return HandleResult(result);
            return NoContent();
        }

        [HttpDelete("clear-all")]
        [Authorize]
        public async Task<IActionResult> ClearAllNotifications()
        {
            var result = await Mediator.Send(new ClearAll.Command());
            if (!result.IsSuccess) return HandleResult(result);
            return Ok(new { deletedCount = result.Value });
        }
    }
}
