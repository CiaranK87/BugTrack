using Application.Projects;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{

    public class ProjectsController(IAuthorizationService authorizationService) : BaseApiController(authorizationService)
    {

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetProjects()
        {
            return HandleResult(await Mediator.Send(new List.Query()));
            
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetProject(Guid id)
        {
            var authorized = await _authorizationService.AuthorizeAsync(User, id.ToString(), "ProjectAnyRole");
            if (!authorized.Succeeded)
                return Forbid();

            return HandleResult(await Mediator.Send(new Details.Query { Id = id }));
}


        [Authorize(Policy = "CanCreateProjects")]
        [HttpPost]
        public async Task<IActionResult> CreateProject(Project project)
        {
            return HandleResult(await Mediator.Send(new Create.Command {Project = project}));
        }



        [Authorize]    
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(Project project)
        {
            var projectResult = await Mediator.Send(new Details.Query { Id = project.Id });
            if (!projectResult.IsSuccess) return HandleResult(projectResult);

            var projectId = projectResult.Value?.Id.ToString();
            if (string.IsNullOrEmpty(projectId)) return NotFound();

            var authorized = await _authorizationService.AuthorizeAsync(
                User,
                projectId,
                "ProjectOwnerOrManager"
            );

            if (!authorized.Succeeded) return Forbid();

            return HandleResult(await Mediator.Send(new Edit.Command { Project = project }));
        }


        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(Guid id)
        {
            var projectResult = await Mediator.Send(new Details.Query { Id = id });
            if (!projectResult.IsSuccess) return HandleResult(projectResult);

            var projectId = projectResult.Value?.Id.ToString();
            if (string.IsNullOrEmpty(projectId)) return NotFound();

            var authorized = await _authorizationService.AuthorizeAsync(
                User,
                projectId,
                "ProjectOwnerOrManager"
            );

            if (!authorized.Succeeded) return Forbid();
            
            return HandleResult(await Mediator.Send(new Delete.Command { Id = id }));
        }

        [Authorize]
        [HttpPost("{id}/participate")]
        public async Task<IActionResult> AddParticipant(Guid id)
        {
            var projectResult = await Mediator.Send(new Details.Query { Id = id });
            if (!projectResult.IsSuccess) return HandleResult(projectResult);

            var projectId = projectResult.Value?.Id.ToString();
            if (string.IsNullOrEmpty(projectId)) return NotFound();

            var authorized = await _authorizationService.AuthorizeAsync(User, projectId, "ProjectOwnerOrManager");
            if (!authorized.Succeeded) return Forbid();

            return HandleResult(await Mediator.Send(new UpdateParticipants.Command { Id = id }));
        }


    }
}