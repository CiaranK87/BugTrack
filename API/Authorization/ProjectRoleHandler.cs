using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Persistence;
using API.Services;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Authorization
{
    public class ProjectRoleHandler : AuthorizationHandler<ProjectRoleRequirement>
    {
        private readonly DataContext _context;
        private readonly IUserAccessor _userAccessor;

        public ProjectRoleHandler(DataContext context, IUserAccessor userAccessor)
        {
            _context = context;
            _userAccessor = userAccessor;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            ProjectRoleRequirement requirement)
        {
            if (context.User.IsInRole("Admin"))
            {
                context.Succeed(requirement);
                return;
            }

            string projectId = null;

            if (context.Resource is AuthorizationFilterContext authContext)
            {
                if (!authContext.HttpContext.Request.RouteValues.TryGetValue("id", out var projectIdObj))
                    return;
                projectId = projectIdObj?.ToString();
            }
            else if (context.Resource is string resource)
            {
                projectId = resource;
            }

            if (string.IsNullOrEmpty(projectId) || !Guid.TryParse(projectId, out var projectGuid))
                return;

            var currentUserId = _userAccessor.GetUserId();
            if (string.IsNullOrEmpty(currentUserId))
                return;

            
            var hasRoleInClaim = context.User.Claims
                .Where(c => c.Type == "projectrole" && c.Value.StartsWith($"project:{projectId}="))
                .Select(c => c.Value.Split('=')[1])
                .Any(role => requirement.AllowedRoles.Contains(role));

            if (hasRoleInClaim)
            {
                context.Succeed(requirement);
                return;
            }

            
            var participant = await _context.ProjectParticipants
                .AsNoTracking()
                .Where(pp => pp.AppUserId == currentUserId && pp.ProjectId == projectGuid)
                .Select(pp => new { pp.Role, pp.IsOwner })
                .FirstOrDefaultAsync();

            if (participant == null)
                return;

            
            if (participant.IsOwner && requirement.AllowedRoles.Contains("Owner"))
            {
                context.Succeed(requirement);
                return;
            }

            
            if (!string.IsNullOrEmpty(participant.Role) && requirement.AllowedRoles.Contains(participant.Role))
            {
                context.Succeed(requirement);
            }
        }
    }
}