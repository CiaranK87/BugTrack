using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API.Controllers
{
    public class ProjectsController : BaseApiController
    {
        private readonly DataContext _context;
        public ProjectsController(DataContext context)
        {
            _context = context;
                       
        }

        [HttpGet] // api/projects
        public async Task<ActionResult<List<Project>>> GetProjects()
        {
            return await _context.Projects.ToListAsync();
            
        }



        [HttpGet("{id}")] // api/project/:id

        public async Task<ActionResult<Project>> GetProject(Guid id)
        {
            return await _context.Projects.FindAsync(id);
        }
    }
}