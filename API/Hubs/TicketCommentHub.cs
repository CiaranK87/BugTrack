using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace API.Hubs
{
    public class TicketCommentHub : Hub
    {
        public async Task JoinTicketGroup(string ticketId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Ticket_{ticketId}");
        }

        public async Task LeaveTicketGroup(string ticketId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Ticket_{ticketId}");
        }
    }
}