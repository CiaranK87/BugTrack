using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly DataContext _context;

    public UsersController(DataContext context)
    {
        _context = context;
    }

    [Authorize]
    [HttpGet("search")]
    public async Task<ActionResult<List<UserSearchDto>>> SearchUsers([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return BadRequest("Query is required");

        var users = await _context.Users
            .Where(u => 
                (u.DisplayName != null && u.DisplayName.Contains(query)) ||
                u.UserName.Contains(query)
            )
            .Select(u => new UserSearchDto
            {
                Id = u.Id,
                Name = u.DisplayName ?? u.UserName,
                Username = u.UserName
            })
            .Take(20)
            .ToListAsync();

        return Ok(users);
    }
}