using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Persistence;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Authorization
{
    public class TicketAuthorizationHandler : AuthorizationHandler<TicketOperationRequirement>
    {
        private readonly DataContext _context;
        private readonly IUserAccessor _userAccessor;

        public TicketAuthorizationHandler(DataContext context, IUserAccessor userAccessor)
        {
            _context = context;
            _userAccessor = userAccessor;
        }

        protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        TicketOperationRequirement requirement)
        {
            var globalRoleClaim = context.User.FindFirst("globalrole")?.Value;
            if (globalRoleClaim == "Admin")
            {
                context.Succeed(requirement);
                return;
            }

            if (context.Resource is Domain.Ticket ticket)
            {
                await CheckProjectAccessAsync(context, requirement, ticket.ProjectId, ticket.Submitter, ticket.Assigned);
                return;
            }

            string ticketIdString = null;
            string projectIdString = null;

            if (context.Resource is AuthorizationFilterContext authContext)
            {
                authContext.HttpContext.Request.RouteValues.TryGetValue("id", out var ticketIdObj);
                ticketIdString = ticketIdObj?.ToString();
                
                authContext.HttpContext.Request.RouteValues.TryGetValue("projectId", out var projectIdObj);
                if (projectIdObj == null)
                {
                    authContext.HttpContext.Request.Query.TryGetValue("projectId", out var projectIdQuery);
                    projectIdString = projectIdQuery.FirstOrDefault();
                }
                else
                {
                    projectIdString = projectIdObj?.ToString();
                }
            }

            if (string.IsNullOrEmpty(ticketIdString) && !string.IsNullOrEmpty(projectIdString))
            {
                if (Guid.TryParse(projectIdString, out var projectGuid))
                {
                    await CheckProjectAccessAsync(context, requirement, projectGuid);
                    return;
                }
            }

            if (!string.IsNullOrEmpty(ticketIdString) && Guid.TryParse(ticketIdString, out var ticketGuid))
            {
                var ticketFromDb = await _context.Tickets
                    .AsNoTracking()
                    .Where(t => t.Id == ticketGuid)
                    .Select(t => new { t.ProjectId, t.Submitter, t.Assigned })
                    .FirstOrDefaultAsync();

                if (ticketFromDb != null)
                {
                    await CheckProjectAccessAsync(context, requirement, ticketFromDb.ProjectId, ticketFromDb.Submitter, ticketFromDb.Assigned);
                }
            }
        }

        private async Task CheckProjectAccessAsync(
            AuthorizationHandlerContext context,
            TicketOperationRequirement requirement,
            Guid projectId,
            string ticketSubmitter = null,
            string ticketAssigned = null)
        {
            var currentUserId = _userAccessor.GetUserId();
            if (string.IsNullOrEmpty(currentUserId))
                return;

            var participant = await _context.ProjectParticipants
                .AsNoTracking()
                .Where(pp => pp.AppUserId == currentUserId && pp.ProjectId == projectId)
                .Select(pp => new { pp.Role, pp.IsOwner })
                .FirstOrDefaultAsync();

            if (participant == null)
                return;

            var role = participant.Role;
            if (role == "Contributor")
                role = "User";

            switch (requirement.Operation)
            {
                case TicketOperation.Read:
                    if (participant.IsOwner ||
                        role == "Owner" ||
                        role == "ProjectManager" ||
                        role == "Developer" ||
                        role == "User" ||
                        role == "Guest")
                    {
                        context.Succeed(requirement);
                    }
                    break;

                case TicketOperation.Create:
                    if (participant.IsOwner ||
                        role == "Owner" ||
                        role == "ProjectManager" ||
                        role == "Developer" ||
                        role == "User" ||
                        role == "Guest")
                    {
                        context.Succeed(requirement);
                    }
                    break;

                case TicketOperation.Edit:
                    if (participant.IsOwner ||
                        role == "Owner" ||
                        role == "ProjectManager" ||
                        role == "Developer" ||
                        (!string.IsNullOrEmpty(ticketSubmitter) && ticketSubmitter == currentUserId) ||
                        (!string.IsNullOrEmpty(ticketAssigned) && ticketAssigned == currentUserId))
                    {
                        context.Succeed(requirement);
                    }
                    break;

                case TicketOperation.Close:
                    if (participant.IsOwner ||
                        role == "Owner" ||
                        role == "ProjectManager" ||
                        role == "Developer")
                    {
                        context.Succeed(requirement);
                    }
                    break;

                case TicketOperation.Delete:
                    // Delete is admin-only; the admin bypass at the top of HandleRequirementAsync
                    // handles this — no project-role path grants delete.
                    break;
            }
        }
    }

    public class TicketOperationRequirement : IAuthorizationRequirement
    {
        public TicketOperation Operation { get; }

        public TicketOperationRequirement(TicketOperation operation)
        {
            Operation = operation;
        }
    }

    public enum TicketOperation
    {
        Read,
        Create,
        Edit,
        Close,
        Delete
    }
}