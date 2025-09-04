using Microsoft.AspNetCore.Authorization;

namespace API.Authorization
{
    public class ProjectRoleRequirement : IAuthorizationRequirement
        {
            public IEnumerable<string> AllowedRoles { get; }

            public ProjectRoleRequirement(params string[] allowedRoles)
            {
                AllowedRoles = allowedRoles;
            }
        }

}
