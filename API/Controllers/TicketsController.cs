using API.Authorization;
using Application.DTOs;
using Application.Tickets;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TicketsController : BaseApiController
    {
        public TicketsController(IMediator mediator, IAuthorizationService authorizationService)
            : base(mediator, authorizationService) {}

        [HttpGet("{id}")]
        public async Task<ActionResult<TicketDto>> GetTicket(Guid id)
        {
            var authResult = await _authorizationService.AuthorizeAsync(
                User, id, new TicketOperationRequirement(TicketOperation.Read));
            if (!authResult.Succeeded) return Forbid();

            return HandleResult(await Mediator.Send(new Details.Query { Id = id }));
        }

        [HttpGet]
        public async Task<ActionResult<List<TicketDto>>> GetTickets() =>
            HandleResult(await Mediator.Send(new List.Query()));

        [HttpGet("project/{projectId}")]
        public async Task<ActionResult<List<TicketDto>>> GetTicketsByProject(Guid projectId)
        {
            var authResult = await _authorizationService.AuthorizeAsync(User, projectId, "ProjectContributor");
            if (!authResult.Succeeded) return Forbid();

            return HandleResult(await Mediator.Send(new ListByProjectId.Query { ProjectId = projectId }));
        }

        [HttpPost]
        public async Task<IActionResult> CreateTicket([FromBody] CreateTicketDto ticketDto)
        {
            var authResult = await _authorizationService.AuthorizeAsync(User, ticketDto.ProjectId, "ProjectContributor");
            if (!authResult.Succeeded) return Forbid();

            var result = await Mediator.Send(new Create.Command { TicketDto = ticketDto });
            if (!result.IsSuccess) return BadRequest(result.Error);
            return CreatedAtAction(nameof(GetTicket), new { id = result.Value }, result.Value);
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

            var deleteResult = await Mediator.Send(new Delete.Command { Id = id });
            if (!deleteResult.IsSuccess) return HandleResult(deleteResult);
            return NoContent();
        }

        [HttpGet("admin/deleted")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<ActionResult<List<TicketDto>>> GetDeletedTickets() =>
            HandleResult(await Mediator.Send(new ListDeleted.Query()));

        [HttpDelete("{id}/admin-delete")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> AdminDeleteTicket(Guid id)
        {
            var result = await Mediator.Send(new AdminDelete.Command { Id = id });
            if (!result.IsSuccess) return HandleResult(result);
            return NoContent();
        }

        [HttpPut("{id}/restore")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> RestoreTicket(Guid id) =>
            HandleResult(await Mediator.Send(new Restore.Command { Id = id }));
    }
}
