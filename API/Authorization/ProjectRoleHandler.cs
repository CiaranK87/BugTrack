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

            string? projectIdString = null;

            // Use resource directly if it's a Guid or string
            if (context.Resource is Guid guidResource)
                projectIdString = guidResource.ToString();
            else if (context.Resource is string stringResource)
                projectIdString = stringResource;

            // Fallback: extract from route
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

            // Check participant role
            var participant = await _context.ProjectParticipants
                .AsNoTracking()
                .Where(pp => pp.AppUserId == currentUserId && pp.ProjectId == projectGuid)
                .Select(pp => new { pp.Role, pp.IsOwner })
                .FirstOrDefaultAsync();

            if (participant == null) return;

            // Owner shortcut
            if (participant.IsOwner && requirement.AllowedRoles.Contains("Owner"))
            {
                context.Succeed(requirement);
                return;
            }

            // Match role string
            if (!string.IsNullOrWhiteSpace(participant.Role) &&
                requirement.AllowedRoles.Contains(participant.Role))
            {
                context.Succeed(requirement);
            }
        }

    }
}
