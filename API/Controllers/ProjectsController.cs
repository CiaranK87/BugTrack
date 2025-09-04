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

        [Authorize(Policy = "ProjectOwnerOrManager")]    
        [HttpPut("{id}")]

        public async Task<IActionResult> EditProject(Guid id, Project project)
        {
            project.Id = id;
            return HandleResult(await Mediator.Send(new Edit.Command {Project = project}));
        }


        [Authorize(Policy = "ProjectOwnerOrManager")]
        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteProject(Guid id)
        {
            return HandleResult (await Mediator.Send(new Delete.Command {Id = id}));
            
        }


        [Authorize(Policy = "ProjectOwnerOrManager")]
        [HttpPost("{id}/participate")]
        public async Task<IActionResult> AddParticipant(Guid id)
        {
            return HandleResult(await Mediator.Send(new UpdateParticipants.Command {Id = id})); 
        }


    }
}