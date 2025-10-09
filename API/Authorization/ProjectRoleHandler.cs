using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Persistence;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Authorization
{
    public class ProjectRoleHandler : AuthorizationHandler<ProjectRoleRequirement>
    {
        private readonly DataContext _context;
        private readonly IUserAccessor _userAccessor;
        private readonly ILogger<ProjectRoleHandler> _logger;

        public ProjectRoleHandler(DataContext context, IUserAccessor userAccessor, ILogger<ProjectRoleHandler> logger)
        {
            _context = context;
            _userAccessor = userAccessor;
            _logger = logger;
        }

        protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ProjectRoleRequirement requirement)
        {
            var globalRoleClaim = context.User.FindFirst("globalrole")?.Value;
            if (globalRoleClaim == "Admin")
            {
                context.Succeed(requirement);
                return;
            }
            
            if (requirement.AllowedRoles.Contains("Owner") && globalRoleClaim == "ProjectManager")
            {
                context.Succeed(requirement);
                return;
            }

            string projectIdString = null;

            if (context.Resource is Guid guidResource)
                projectIdString = guidResource.ToString();
            else if (context.Resource is string stringResource)
                projectIdString = stringResource;

            if (projectIdString == null && context.Resource is AuthorizationFilterContext authContext)
            {
                if (!authContext.HttpContext.Request.RouteValues.TryGetValue("projectId", out var pIdObj))
                    authContext.HttpContext.Request.RouteValues.TryGetValue("id", out pIdObj);
                projectIdString = pIdObj?.ToString();
            }

            if (!Guid.TryParse(projectIdString, out var projectGuid))
                return;

            var currentUserId = _userAccessor.GetUserId();
            if (string.IsNullOrEmpty(currentUserId))
                return;

            var participant = await _context.ProjectParticipants
                .AsNoTracking()
                .Where(pp => pp.AppUserId == currentUserId && pp.ProjectId == projectGuid)
                .Select(pp => new { pp.Role, pp.IsOwner })
                .FirstOrDefaultAsync();

            if (participant == null) return;

            if (participant.IsOwner && requirement.AllowedRoles.Contains("Owner"))
            {
                context.Succeed(requirement);
                return;
            }
            
            if (!string.IsNullOrWhiteSpace(participant.Role))
            {
                if (requirement.AllowedRoles.Contains(participant.Role))
                {
                    context.Succeed(requirement);
                    return;
                }

                if (IsRoleInHierarchy(participant.Role, requirement.AllowedRoles))
                {
                    context.Succeed(requirement);
                    return;
                }
            }
        }

        private bool IsRoleInHierarchy(string userRole, IEnumerable<string> allowedRoles)
        {
            var roleHierarchy = new Dictionary<string, int>
            {
                { "Owner", 4 },
                { "ProjectManager", 3 },
                { "Developer", 2 },
                { "User", 1 }
            };

            if (!roleHierarchy.TryGetValue(userRole, out var userRoleLevel))
                return false;

            foreach (var role in allowedRoles)
            {
                if (roleHierarchy.TryGetValue(role, out var allowedRoleLevel) &&
                    userRoleLevel >= allowedRoleLevel)
                {
                    return true;
                }
            }

            return false;
        }

    }
}
