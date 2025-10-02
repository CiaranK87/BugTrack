using System.Security.Claims;
using Application.Interfaces;
using Application.Projects;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API.Controllers
{
    public class ProjectsController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly IUserAccessor _userAccessor;

        public ProjectsController(
            DataContext context,
            IUserAccessor userAccessor,
            IAuthorizationService authorizationService
        ) : base(authorizationService)
        {
            _context = context;
            _userAccessor = userAccessor;
        }


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetProjects()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await Mediator.Send(new List.Query { UserId = userId });

            return HandleResult(result);
        }


        [Authorize]
        [HttpGet("{projectId}")]
        public async Task<IActionResult> GetProject(Guid projectId)
        {
            var authorized = await _authorizationService.AuthorizeAsync(
                User, projectId, "ProjectContributor");

            if (!authorized.Succeeded)
                return Forbid();

            return HandleResult(await Mediator.Send(new Details.Query { Id = projectId }));
        }


        [Authorize(Policy = "CanCreateProjects")]
        [HttpPost]
        public async Task<IActionResult> CreateProject(Project project) =>
            HandleResult(await Mediator.Send(new Create.Command { Project = project }));


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

        [Authorize]
        [HttpPost("{projectId}/participate")]
        public async Task<IActionResult> AddParticipant(Guid projectId)
        {
            var projectResult = await Mediator.Send(new Details.Query { Id = projectId });
            if (!projectResult.IsSuccess) return HandleResult(projectResult);

            var authorized = await _authorizationService.AuthorizeAsync(User, projectId.ToString(), "ProjectOwnerOrManager");
            if (!authorized.Succeeded) return Forbid();

            return HandleResult(await Mediator.Send(new UpdateParticipants.Command { Id = projectId }));
        }

        [Authorize]
        [HttpGet("{projectId}/role")]
        public async Task<ActionResult<string>> GetUserRole(Guid projectId)
        {
            var userId = _userAccessor.GetUserId();
            var participant = await _context.ProjectParticipants
                .AsNoTracking()
                .FirstOrDefaultAsync(pp => pp.ProjectId == projectId && pp.AppUserId == userId);

            if (participant == null) return Forbid();

            if (participant.IsOwner) return "Owner";
            return participant.Role ?? "User";
        }


        // Add participant to project ************************************************************************************

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