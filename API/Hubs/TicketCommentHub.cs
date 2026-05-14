using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using API.Authorization;
using System;

namespace API.Hubs
{
    [Authorize]
    public class TicketCommentHub : Hub
    {
        private readonly IAuthorizationService _authorizationService;

        public TicketCommentHub(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        public async Task JoinTicketGroup(string ticketId)
        {
            if (!Guid.TryParse(ticketId, out var ticketIdGuid))
                throw new HubException("Invalid ticket ID format.");

            var authResult = await _authorizationService.AuthorizeAsync(Context.User, ticketIdGuid, new TicketOperationRequirement(TicketOperation.Read));
            
            if (!authResult.Succeeded)
                throw new HubException("You do not have permission to access this ticket's comments.");

            await Groups.AddToGroupAsync(Context.ConnectionId, $"Ticket_{ticketId}");
        }

        public async Task LeaveTicketGroup(string ticketId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Ticket_{ticketId}");
        }
    }
}