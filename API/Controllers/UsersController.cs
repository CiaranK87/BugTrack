using Application.DTOs;
using Application.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : BaseApiController
    {
        public UsersController(IMediator mediator, IAuthorizationService authorizationService)
            : base(mediator, authorizationService) {}


        [Authorize(Policy = "CanManageGlobalRoles")]
        [HttpGet("list")]
        public async Task<IActionResult> GetUsers() =>
            HandleResult(await Mediator.Send(new List.Query()));

        [Authorize]
        [HttpGet("search")]
        public async Task<IActionResult> SearchUsers([FromQuery] string query, [FromQuery] Guid? projectId) =>
            HandleResult(await Mediator.Send(new Search.Query { SearchQuery = query, ProjectId = projectId }));

        [Authorize(Policy = "CanManageGlobalRoles")]
        [HttpPut("{userId}/role")]
        public async Task<IActionResult> UpdateUserRole(string userId, [FromBody] UpdateRoleDto updateRoleDto) =>
            HandleResult(await Mediator.Send(new UpdateRole.Command { UserId = userId, Role = updateRoleDto.Role }));

        [Authorize(Policy = "CanManageGlobalRoles")]
        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(string userId, [FromBody] UpdateUserDto updateUserDto) =>
            HandleResult(await Mediator.Send(new Update.Command { UserId = userId, Dto = updateUserDto }));

        [Authorize(Policy = "CanManageGlobalRoles")]
        [HttpDelete("{userId}")]
        public async Task<IActionResult> SoftDeleteUser(string userId) =>
            HandleResult(await Mediator.Send(new SoftDelete.Command { UserId = userId }));
    }
}
