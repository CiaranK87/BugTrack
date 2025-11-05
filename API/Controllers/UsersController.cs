using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Application.DTOs;
using System.Security.Claims;

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
                (u.DisplayName != null && u.DisplayName.ToLower().Contains(query.ToLower())) ||
                u.UserName.ToLower().Contains(query.ToLower())
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
            .Where(u => !u.IsDeleted)
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
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return NotFound("User not found");

        if (user.GlobalRole == "Admin" && user.Id != currentUserId)
            return Forbid("Admins cannot modify other admins");

        var validRoles = new[] { "Admin", "ProjectManager", "Developer", "User" };
        if (!validRoles.Contains(updateRoleDto.Role))
            return BadRequest("Invalid role");

        if (user.Id == currentUserId && updateRoleDto.Role != "Admin")
            return BadRequest("Admins cannot demote themselves");

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

    [Authorize(Policy = "CanManageGlobalRoles")]
    [HttpPut("{userId}")]
    public async Task<ActionResult<UserDto>> UpdateUser(string userId, [FromBody] UpdateUserDto updateUserDto)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return NotFound("User not found");

        if (user.GlobalRole == "Admin" && user.Id != currentUserId)
            return Forbid("Admins cannot modify other admins");

        var oldUsername = user.UserName;

        user.DisplayName = updateUserDto.DisplayName ?? user.DisplayName;
        user.Email = updateUserDto.Email ?? user.Email;
        user.UserName = updateUserDto.Username ?? user.UserName;
        user.JobTitle = updateUserDto.JobTitle;
        user.Bio = updateUserDto.Bio;

        if (oldUsername != user.UserName)
        {
            var ticketsToUpdate = await _context.Tickets
                .Where(t => t.Submitter == oldUsername || t.Assigned == oldUsername)
                .ToListAsync();

            foreach (var ticket in ticketsToUpdate)
            {
                if (ticket.Submitter == oldUsername)
                    ticket.Submitter = user.UserName;
                
                if (ticket.Assigned == oldUsername)
                    ticket.Assigned = user.UserName;
            }
        }

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

    [Authorize(Policy = "CanManageGlobalRoles")]
    [HttpDelete("{userId}")]
    public async Task<ActionResult> SoftDeleteUser(string userId)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return NotFound("User not found");

        if (user.GlobalRole == "Admin" && user.Id != currentUserId)
            return Forbid("Admins cannot delete other admins");

        if (user.Id == currentUserId)
            return BadRequest("Admins cannot delete themselves");

        user.IsDeleted = true;
        user.DeletedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok();
    }
}