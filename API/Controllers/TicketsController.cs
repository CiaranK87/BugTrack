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
            return HandleResult(await Mediator.Send(new Details.Query{Id = id}));
        }


[HttpGet("project/{projectId}")]
public async Task<IActionResult> GetTicketsByProject(Guid projectId)
{
    var authorized = await _authorizationService.AuthorizeAsync(User, projectId.ToString(), "ProjectAnyRole");
    if (!authorized.Succeeded) return Forbid();

    return HandleResult(await Mediator.Send(new Application.Tickets.ListByProjectId.Query { ProjectId = projectId }));
}


        [Authorize(Policy = "ProjectOwnerOrManager")]
        [HttpPost("projects/{projectId}/tickets")]
        public async Task<IActionResult> CreateTicket(Guid projectId, Ticket ticket)
        {
            return HandleResult(await Mediator.Send(new Create.Command {Ticket = ticket}));
        }

        [Authorize(Policy = "ProjectContributor")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(Guid id, Ticket ticket)
        {
            ticket.Id = id;
            var result = await Mediator.Send(new Edit.Command { Ticket = ticket });
            if (!result.IsSuccess) return BadRequest(result.Error);
            return Ok();
        }


        [Authorize(Policy = "ProjectOwnerOrManager")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTicket(Guid id)
        {
            return HandleResult(await Mediator.Send(new Delete.Command {Id = id}));
        }

    }
}