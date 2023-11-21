using Application.Projects;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class ProjectsController : BaseApiController
    {

        [HttpGet] // api/projects
        public async Task<ActionResult<List<Project>>> GetProjects()
        {
            return await Mediator.Send(new List.Query());
            
        }



        [HttpGet("{id}")] // api/project/:id

        public async Task<ActionResult<Project>> GetProject(Guid id)
        {
            return await Mediator.Send(new Details.Query{Id = id});
        }

        [HttpPost]

        public async Task<IActionResult> CreateProject(Project project)
        {
            await Mediator.Send(new Create.Command {Project = project});
            return Ok();
        }


        [HttpPut("{id}")]

        public async Task<IActionResult> EditProject(Guid id, Project project)
        {
            project.Id = id;
            await Mediator.Send(new Edit.Command {Project = project});
            return Ok();
        }

        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteProject(Guid id)
        {
            await Mediator.Send(new Delete.Command {Id = id});
            return Ok();
        }
    }
}