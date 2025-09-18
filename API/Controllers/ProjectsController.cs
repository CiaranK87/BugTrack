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
            Console.WriteLine($"Getting projects for user: {userId}");

            var result = await Mediator.Send(new List.Query { UserId = userId });
            Console.WriteLine($"Query returned {result.Value?.Count ?? 0} projects");

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


        // Add member to project ************************************************************************************

        [Authorize]
        [HttpPost("{projectId}/members")]
        public async Task<IActionResult> AddProjectMember(Guid projectId, [FromBody] AddMemberDto addMemberDto)
        {
            // Check if user can manage this project
            var authorized = await _authorizationService.AuthorizeAsync(
                User, projectId, "ProjectOwnerOrManager");

            if (!authorized.Succeeded) 
                return Forbid("Only project owners can add members");

            var command = new AddMember.Command 
            { 
                ProjectId = projectId,
                UserId = addMemberDto.UserId,
                Role = addMemberDto.Role
            };

            return HandleResult(await Mediator.Send(command));
        }

        [Authorize]
        [HttpPut("{projectId}/members/{userId}")]
        public async Task<IActionResult> UpdateMemberRole(Guid projectId, string userId, [FromBody] UpdateRoleDto updateRoleDto)
        {
            var authorized = await _authorizationService.AuthorizeAsync(
                User, projectId, "ProjectOwnerOrManager");

            if (!authorized.Succeeded) 
                return Forbid("Only project owners can update member roles");

            var command = new UpdateMemberRole.Command 
            { 
                ProjectId = projectId, 
                UserId = userId, 
                Role = updateRoleDto.Role 
            };
            
            return HandleResult(await Mediator.Send(command));
        }

        [Authorize]
        [HttpDelete("{projectId}/members/{userId}")]
        public async Task<IActionResult> RemoveProjectMember(Guid projectId, string userId)
        {
            var authorized = await _authorizationService.AuthorizeAsync(
                User, projectId, "ProjectOwnerOrManager");

            if (!authorized.Succeeded) 
                return Forbid("Only project owners can remove members");

            var command = new RemoveMember.Command { ProjectId = projectId, UserId = userId };
            return HandleResult(await Mediator.Send(command));
        }

        [Authorize]
        [HttpGet("{projectId}/members")]
        public async Task<IActionResult> GetProjectMembers(Guid projectId)
        {
            // Any project member can view the member list
            var authorized = await _authorizationService.AuthorizeAsync(
                User, projectId, "ProjectContributor");

            if (!authorized.Succeeded) 
                return Forbid();

            return HandleResult(await Mediator.Send(new ListMembers.Query { ProjectId = projectId }));
        }
    }
}