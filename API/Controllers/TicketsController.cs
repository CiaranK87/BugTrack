using Application.Tickets;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class TicketsController(IAuthorizationService authorizationService) : BaseApiController(authorizationService)
    {
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetTickets()
        {
            return HandleResult(await Mediator.Send(new List.Query()));
        }


        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTicket(Guid id)
        {            
            var ticketResult = await Mediator.Send(new Details.Query{Id = id});
            if (!ticketResult.IsSuccess) return HandleResult(ticketResult);

            var projectId = ticketResult.Value?.ProjectId.ToString();
            if (string.IsNullOrEmpty(projectId)) return NotFound();

            var authorized = await _authorizationService.AuthorizeAsync(User, projectId, "ProjectAnyRole");
            if (!authorized.Succeeded) return Forbid();

            return Ok(ticketResult.Value);
        }

        [Authorize]
        [HttpGet("project/{projectId}")]
        public async Task<IActionResult> GetTicketsByProject(Guid projectId)
        {
            var authorized = await _authorizationService.AuthorizeAsync(User, projectId.ToString(), "ProjectAnyRole");
            if (!authorized.Succeeded) return Forbid();

            return HandleResult(await Mediator.Send(new Application.Tickets.ListByProjectId.Query { ProjectId = projectId }));
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateTicket(Ticket ticket)
        {
            var projectId = ticket.ProjectId.ToString();
            var authorized = await _authorizationService.AuthorizeAsync(User, projectId, "ProjectContributor");
            
            if (!authorized.Succeeded)
                return Forbid();

            return HandleResult(await Mediator.Send(new Create.Command { Ticket = ticket }));
        }


        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(Guid id, Ticket ticket)
        {
            ticket.Id = id;

            
            var ticketResult = await Mediator.Send(new Details.Query { Id = id });
            if (!ticketResult.IsSuccess) return HandleResult(ticketResult);

            var projectId = ticketResult.Value?.ProjectId.ToString();
            if (string.IsNullOrEmpty(projectId)) return NotFound();

            
            var authorized = await _authorizationService.AuthorizeAsync(User, projectId, "ProjectContributor");
            if (!authorized.Succeeded) return Forbid();

            
            var result = await Mediator.Send(new Edit.Command { Ticket = ticket });
            return HandleResult(result);
        }


        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTicket(Guid id)
        {
            var ticketResult = await Mediator.Send(new Details.Query { Id = id });
            if (!ticketResult.IsSuccess) return HandleResult(ticketResult);

            var projectId = ticketResult.Value?.ProjectId.ToString();
            if (string.IsNullOrEmpty(projectId)) return NotFound();

            var authorized = await _authorizationService.AuthorizeAsync(
                User, 
                projectId, 
                "ProjectOwnerOrManager"
            );

            if (!authorized.Succeeded) return Forbid();

            return HandleResult(await Mediator.Send(new Delete.Command { Id = id }));
        }

    }
}