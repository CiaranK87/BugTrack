using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class NotificationsController : BaseApiController
    {
        private readonly INotificationService _notificationService;
        private readonly IUserAccessor _userAccessor;

        public NotificationsController(
            INotificationService notificationService,
            IUserAccessor userAccessor,
            IAuthorizationService authorizationService) : base(authorizationService)
        {
            _notificationService = notificationService;
            _userAccessor = userAccessor;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<NotificationDto>>> GetNotifications()
        {
            var userId = _userAccessor.GetUserId();
            var notifications = await _notificationService.GetUserNotificationsAsync(userId);
            return Ok(notifications);
        }

        [HttpGet("unread-count")]
        [Authorize]
        public async Task<ActionResult<int>> GetUnreadCount()
        {
            var userId = _userAccessor.GetUserId();
            var count = await _notificationService.GetUnreadCountAsync(userId);
            return Ok(count);
        }

        [HttpPut("{id}/read")]
        [Authorize]
        public async Task<ActionResult> MarkAsRead(Guid id)
        {
            var userId = _userAccessor.GetUserId();
            var success = await _notificationService.MarkAsReadAsync(id, userId);
            
            if (!success)
                return NotFound();
            
            return Ok();
        }

        [HttpPut("read-all")]
        [Authorize]
        public async Task<ActionResult> MarkAllAsRead()
        {
            var userId = _userAccessor.GetUserId();
            await _notificationService.MarkAllAsReadAsync(userId);
            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteNotification(Guid id)
        {
            var userId = _userAccessor.GetUserId();
            var success = await _notificationService.DeleteAsync(id, userId);
            
            if (!success)
                return NotFound();
            
            return Ok();
        }

        [HttpDelete("clear-all")]
        [Authorize]
        public async Task<ActionResult> ClearAllNotifications()
        {
            var userId = _userAccessor.GetUserId();
            var count = await _notificationService.DeleteAllAsync(userId);
            return Ok(new { deletedCount = count });
        }
    }
}
