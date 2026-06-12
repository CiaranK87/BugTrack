using System.Security.Claims;
using Application.DTOs;
using Application.Projects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class ProjectsController : BaseApiController
    {
        public ProjectsController(IAuthorizationService authorizationService)
            : base(authorizationService) {}



        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetProjects()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var globalRole = User.FindFirst("globalrole")?.Value;

            var result = await Mediator.Send(new List.Query { UserId = userId, GlobalRole = globalRole });

            return HandleResult(result);
        }


        [Authorize]
        [HttpGet("{projectId}")]
        public async Task<IActionResult> GetProject(Guid projectId)
        {
            var authorized = await _authorizationService.AuthorizeAsync(
                User, projectId, "ProjectAnyRole");

            if (!authorized.Succeeded)
                return Forbid();

            return HandleResult(await Mediator.Send(new Details.Query { Id = projectId }));
        }


        [Authorize(Policy = "CanCreateProjects")]
        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] CreateProjectDto projectDto) =>
            HandleResult(await Mediator.Send(new Create.Command { ProjectDto = projectDto }));


        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(Guid id, ProjectDto projectDto)
        {
            var authorized = await _authorizationService.AuthorizeAsync(
                User,
                id.ToString(),
                "ProjectOwnerOrManager");

            if (!authorized.Succeeded) return Forbid();

            return HandleResult(await Mediator.Send(new Edit.Command
            {
                Id = id,
                ProjectDto = projectDto
            }));
        }


        [Authorize]
        [HttpDelete("{projectId}")]
        public async Task<IActionResult> DeleteProject(Guid projectId)
        {
            var projectResult = await Mediator.Send(new Details.Query { Id = projectId });
            if (!projectResult.IsSuccess) return HandleResult(projectResult);

            var authorized = await _authorizationService.AuthorizeAsync(User, projectId.ToString(), "ProjectOwnerOrManager");
            if (!authorized.Succeeded) return Forbid();

            return HandleResult(await Mediator.Send(new Delete.Command { Id = projectId }));
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpDelete("{projectId}/admin-delete")]
        public async Task<IActionResult> AdminDeleteProject(Guid projectId) =>
            HandleResult(await Mediator.Send(new AdminDelete.Command { Id = projectId }));

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("admin/deleted")]
        public async Task<IActionResult> GetDeletedProjects()
        {
            return HandleResult(await Mediator.Send(new ListDeleted.Query()));
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPut("{projectId}/restore")]
        public async Task<IActionResult> RestoreProject(Guid projectId) =>
            HandleResult(await Mediator.Send(new Restore.Command { Id = projectId }));

        [Authorize]
        [HttpPost("{projectId}/cancel")]
        public async Task<IActionResult> ToggleCancel(Guid projectId) =>
            HandleResult(await Mediator.Send(new ToggleCancel.Command { Id = projectId }));


        [Authorize]
        [HttpGet("{projectId}/role")]
        public async Task<IActionResult> GetUserRole(Guid projectId) =>
            HandleResult(await Mediator.Send(new GetUserRole.Query { ProjectId = projectId }));




        [Authorize]
        [HttpPost("{projectId}/participants")]
        public async Task<IActionResult> AddProjectParticipant(Guid projectId, [FromBody] AddParticipantDto addParticipantDto)
        {
            var authorized = await _authorizationService.AuthorizeAsync(
                User, projectId, "ProjectOwnerOrManager");

            if (!authorized.Succeeded) 
                return Forbid("Only project owners can add participants");

            var command = new AddParticipant.Command 
            { 
                ProjectId = projectId,
                UserId = addParticipantDto.UserId,
                Role = addParticipantDto.Role
            };

            return HandleResult(await Mediator.Send(command));
        }


        [Authorize]
        [HttpPut("{projectId}/participants/{userId}")]
        public async Task<IActionResult> UpdateParticipantRole(Guid projectId, string userId, [FromBody] UpdateRoleDto updateRoleDto)
        {
            var authorized = await _authorizationService.AuthorizeAsync(
                User, projectId, "ProjectOwnerOrManager");

            if (!authorized.Succeeded) 
                return Forbid("Only project owners can update participant roles");

            var command = new UpdateParticipantRole.Command 
            { 
                ProjectId = projectId, 
                UserId = userId, 
                Role = updateRoleDto.Role 
            };
            
            return HandleResult(await Mediator.Send(command));
        }

        [Authorize]
        [HttpDelete("{projectId}/participants/{userId}")]
        public async Task<IActionResult> RemoveProjectParticipant(Guid projectId, string userId)
        {
            var authorized = await _authorizationService.AuthorizeAsync(
                User, projectId, "ProjectOwnerOrManager");

            if (!authorized.Succeeded) 
                return Forbid("Only project owners can remove participants");

            var command = new RemoveParticipant.Command { ProjectId = projectId, UserId = userId };
            return HandleResult(await Mediator.Send(command));
        }

        [Authorize]
        [HttpGet("{projectId}/participants")]
        public async Task<IActionResult> GetProjectParticipants(Guid projectId)
        {
            var authorized = await _authorizationService.AuthorizeAsync(
                User, projectId, "ProjectContributor");

            if (!authorized.Succeeded) 
                return Forbid();

            return HandleResult(await Mediator.Send(new ListParticipants.Query { ProjectId = projectId }));
        }

    }
}