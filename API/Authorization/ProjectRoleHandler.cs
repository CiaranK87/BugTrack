using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;


namespace API.Authorization
{
    public class ProjectRoleHandler : AuthorizationHandler<ProjectRoleRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ProjectRoleRequirement requirement)
        {
        string projectId = null;

        // Case 1: From MVC filter
        if (context.Resource is AuthorizationFilterContext authContext)
        {
            if (!authContext.HttpContext.Request.RouteValues.TryGetValue("id", out var projectIdObj))
                return Task.CompletedTask;

            projectId = projectIdObj?.ToString();
        }
        // Case 2: From AuthorizeAsync (resource is string)
        else if (context.Resource is string resource)
        {
            projectId = resource;
        }

        if (string.IsNullOrEmpty(projectId))
            return Task.CompletedTask;

        var hasRole = context.User.Claims
            .Where(c => c.Type == "projectrole" && c.Value.StartsWith($"project:{projectId}="))
            .Select(c => c.Value.Split('=')[1])
            .Any(role => requirement.AllowedRoles.Contains(role));

        if (hasRole)
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
    }
}
