using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Application.DTOs;

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

    [Authorize(Policy = "CanManageGlobalRoles")]
    [HttpGet("list")]
    public async Task<ActionResult<List<UserDto>>> GetUsers()
    {
        var users = await _context.Users
            .Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.UserName,
                DisplayName = u.DisplayName,
                Email = u.Email,
                GlobalRole = u.GlobalRole ?? "User",
                JobTitle = u.JobTitle,
                Bio = u.Bio
            })
            .ToListAsync();

        return Ok(users);
    }

    [Authorize(Policy = "CanManageGlobalRoles")]
    [HttpPut("{userId}/role")]
    public async Task<ActionResult<UserDto>> UpdateUserRole(string userId, [FromBody] UpdateRoleDto updateRoleDto)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return NotFound("User not found");


        var validRoles = new[] { "Admin", "ProjectManager", "User" };
        if (!validRoles.Contains(updateRoleDto.Role))
            return BadRequest("Invalid role");

        user.GlobalRole = updateRoleDto.Role;
        await _context.SaveChangesAsync();

        return Ok(new UserDto
        {
            Id = user.Id,
            Username = user.UserName,
            DisplayName = user.DisplayName,
            Email = user.Email,
            GlobalRole = user.GlobalRole,
            JobTitle = user.JobTitle,
            Bio = user.Bio
        });
    }
}