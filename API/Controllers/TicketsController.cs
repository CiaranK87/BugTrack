using Application.Core;
using Application.Tickets;
using Application.DTOs;
using AutoMapper;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public TicketsController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TicketDto>> GetTicket(Guid id)
        {
            var result = await _mediator.Send(new Details.Query { Id = id });
            if (result.Value == null) return NotFound();

            return Ok(_mapper.Map<TicketDto>(result.Value));
            
        }

        [HttpGet]
        public async Task<ActionResult<List<TicketDto>>> GetTickets()
        {
            var result = await _mediator.Send(new Application.Tickets.List.Query());
            if (!result.IsSuccess) return BadRequest(result.Error);
            
            var dtoList = result.Value.Select(t => _mapper.Map<TicketDto>(t)).ToList();
            return Ok(dtoList);
        }

        [HttpGet("project/{projectId}")]
        public async Task<ActionResult<List<TicketDto>>> GetTicketsByProject(Guid projectId)
        {
            var result = await _mediator.Send(new ListByProjectId.Query { ProjectId = projectId });
            var dtoList = result.Value.Select(t => _mapper.Map<TicketDto>(t)).ToList();
            return Ok(dtoList);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTicket([FromBody] TicketDto ticketDto)
        {
            var ticket = _mapper.Map<Ticket>(ticketDto);
            return HandleResult(await _mediator.Send(new Create.Command { Ticket = ticket }));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditTicket(Guid id, [FromBody] EditTicketDto editDto)
        {
            var ticket = _mapper.Map<Ticket>(editDto);
            ticket.Id = id;
            return HandleResult(await _mediator.Send(new Edit.Command { Ticket = ticket }));
        }

        private IActionResult HandleResult<T>(Result<T> result)
        {
            if (result == null) return NotFound();
            if (result.IsSuccess && result.Value != null) return Ok(result.Value);
            if (result.IsSuccess) return NoContent();
            return BadRequest(result.Error);
        }
    }
}