using API.Authorization;
using Application.DTOs;
using Application.Tickets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TicketsController : BaseApiController
    {
        public TicketsController(IAuthorizationService authorizationService)
            : base(authorizationService) {}

        [HttpGet("{id}")]
        public async Task<ActionResult<TicketDto>> GetTicket(Guid id)
        {
            var result = await Mediator.Send(new Details.Query { Id = id });
            if (result.Value == null) return NotFound();

            var authResult = await _authorizationService.AuthorizeAsync(
                User, id, new TicketOperationRequirement(TicketOperation.Read));
            if (!authResult.Succeeded) return Forbid();

            return HandleResult(result);
        }

        [HttpGet]
        public async Task<ActionResult<List<TicketDto>>> GetTickets() =>
            HandleResult(await Mediator.Send(new List.Query()));

        [HttpGet("project/{projectId}")]
        public async Task<ActionResult<List<TicketDto>>> GetTicketsByProject(Guid projectId)
        {
            var authResult = await _authorizationService.AuthorizeAsync(User, projectId, "ProjectAnyRole");
            if (!authResult.Succeeded) return Forbid();

            return HandleResult(await Mediator.Send(new ListByProjectId.Query { ProjectId = projectId }));
        }

        [HttpPost]
        public async Task<IActionResult> CreateTicket([FromBody] CreateTicketDto ticketDto)
        {
            var authResult = await _authorizationService.AuthorizeAsync(User, ticketDto.ProjectId, "ProjectContributor");
            if (!authResult.Succeeded) return Forbid();

            return HandleResult(await Mediator.Send(new Create.Command { TicketDto = ticketDto }));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditTicket(Guid id, [FromBody] EditTicketDto editDto)
        {
            var result = await Mediator.Send(new Details.Query { Id = id });
            if (result.Value == null) return NotFound();

            var authResult = await _authorizationService.AuthorizeAsync(
                User, id, new TicketOperationRequirement(TicketOperation.Edit));
            if (!authResult.Succeeded) return Forbid();

            return HandleResult(await Mediator.Send(new Edit.Command { EditDto = editDto, Id = id }));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTicket(Guid id)
        {
            var result = await Mediator.Send(new Details.Query { Id = id });
            if (result.Value == null) return NotFound();

            var authResult = await _authorizationService.AuthorizeAsync(
                User, id, new TicketOperationRequirement(TicketOperation.Delete));
            if (!authResult.Succeeded) return Forbid();

            return HandleResult(await Mediator.Send(new Delete.Command { Id = id }));
        }

        [HttpGet("admin/deleted")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<ActionResult<List<TicketDto>>> GetDeletedTickets() =>
            HandleResult(await Mediator.Send(new ListDeleted.Query()));

        [HttpDelete("{id}/admin-delete")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> AdminDeleteTicket(Guid id) =>
            HandleResult(await Mediator.Send(new AdminDelete.Command { Id = id }));

        [HttpPut("{id}/restore")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> RestoreTicket(Guid id) =>
            HandleResult(await Mediator.Send(new Restore.Command { Id = id }));
    }
}
