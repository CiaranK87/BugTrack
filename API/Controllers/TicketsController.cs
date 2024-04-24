using Application.Tickets;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class TicketsController : BaseApiController
    {
        [HttpGet] //api/tickets
        public async Task<ActionResult<List<Ticket>>> GetTickets()
        {
            return await Mediator.Send(new List.Query());
        }

        [HttpGet("{id}")] //api/ticket/:id

        public async Task<ActionResult<Ticket>> GetTicket(Guid id)
        {
             return await Mediator.Send(new Details.Query{Id = id});
        }

        [HttpPost]

        public async Task<IActionResult> CreateTicket(Ticket ticket)
        {
            await Mediator.Send(new Create.Command {Ticket = ticket});
            return Ok();
        }

        [HttpPut("{id}")]

        public async Task<IActionResult> EditTicket(Guid id, Ticket ticket)
        {
            ticket.Id = id;

            await Mediator.Send(new Edit.Command {Ticket = ticket});

            return Ok();
        }

        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteTicket(Guid id)
        {
            await Mediator.Send(new Delete.Command {Id = id});
            return Ok();
        }

    }
}