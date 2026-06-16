using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Infrastructure.Security
{
    public class IsOwnerRequirement : IAuthorizationRequirement
    {
    }

    public class IsOwnerRequirementHandler : AuthorizationHandler<IsOwnerRequirement>
    {
        private readonly DataContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public IsOwnerRequirementHandler(DataContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsOwnerRequirement requirement)
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Task.CompletedTask;

            Guid projectId;
            if (context.Resource is Guid guidResource)
            {
                projectId = guidResource;
            }
            else
            {
                var routeValues = _httpContextAccessor.HttpContext?.Request.RouteValues;
                var idString = routeValues?.GetValueOrDefault("projectId")?.ToString()
                            ?? routeValues?.GetValueOrDefault("id")?.ToString();
                if (!Guid.TryParse(idString, out projectId))
                    return Task.CompletedTask;
            }

            var participant = _dbContext.ProjectParticipants
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.AppUserId == userId && x.ProjectId == projectId)
                .Result;

            if (participant == null) return Task.CompletedTask;
            if (participant.IsOwner) context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}