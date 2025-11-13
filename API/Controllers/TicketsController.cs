using Application.Core;
using Application.Tickets;
using Application.Interfaces;
using AutoMapper;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IUserAccessor _userAccessor;
        private readonly IAuthorizationService _authorizationService;

        public TicketsController(IMediator mediator, IMapper mapper, DataContext context, IUserAccessor userAccessor, IAuthorizationService authorizationService)
        {
            _mediator = mediator;
            _mapper = mapper;
            _context = context;
            _userAccessor = userAccessor;
            _authorizationService = authorizationService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TicketDto>> GetTicket(Guid id)
        {
            var globalRoleClaim = User.FindFirst("globalrole")?.Value;
            var isGlobalAdmin = globalRoleClaim == "Admin";
            
            var ticketResult = await _mediator.Send(new Details.Query { Id = id });
            if (ticketResult.Value == null) return NotFound();
            
            var ticket = ticketResult.Value;
            
            if (!isGlobalAdmin)
            {
                var currentUserId = _userAccessor.GetUserId();
                var currentUser = await _context.Users
                    .AsNoTracking()
                    .Where(u => u.Id == currentUserId)
                    .Select(u => u.UserName)
                    .FirstOrDefaultAsync();
                
                var isSubmitter = ticket.Submitter == currentUser;
                
                if (isSubmitter)
                {
                    return Ok(_mapper.Map<TicketDto>(ticket));
                }
                
                var projectOwner = await _context.ProjectParticipants
                    .AsNoTracking()
                    .Where(pp => pp.AppUserId == currentUserId && pp.ProjectId == ticket.ProjectId && pp.IsOwner)
                    .FirstOrDefaultAsync();
                
                var isProjectOwner = projectOwner != null;
                
                if (isProjectOwner)
                {
                    return Ok(_mapper.Map<TicketDto>(ticket));
                }
                
                var authorized = await HttpContext.RequestServices
                    .GetService<IAuthorizationService>()
                    .AuthorizeAsync(User, ticket.ProjectId, "ProjectAnyRole");

                if (!authorized.Succeeded)
                    return Forbid();
            }

            return Ok(_mapper.Map<TicketDto>(ticket));
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<TicketDto>>> GetTickets()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var globalRole = User.FindFirst("globalrole")?.Value;
            
            var result = await _mediator.Send(new Application.Tickets.List.Query { UserId = userId, GlobalRole = globalRole });
            if (!result.IsSuccess) return BadRequest(result.Error);
            
            var dtoList = result.Value.Select(t => _mapper.Map<TicketDto>(t)).ToList();
            return Ok(dtoList);
        }

        [HttpGet("project/{projectId}")]
        public async Task<ActionResult<List<TicketDto>>> GetTicketsByProject(Guid projectId)
        {
            var globalRoleClaim = User.FindFirst("globalrole")?.Value;
            var isGlobalAdmin = globalRoleClaim == "Admin";
            
            if (!isGlobalAdmin)
            {
                var authorized = await HttpContext.RequestServices
                    .GetService<IAuthorizationService>()
                    .AuthorizeAsync(User, projectId, "ProjectAnyRole");

                if (!authorized.Succeeded)
                    return Forbid();
            }

            var result = await _mediator.Send(new ListByProjectId.Query { ProjectId = projectId });
            var dtoList = result.Value.Select(t => _mapper.Map<TicketDto>(t)).ToList();
            return Ok(dtoList);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTicket([FromBody] TicketDto ticketDto)
        {
            // Check authorization using the ProjectId from the ticket
            var authorized = await _authorizationService.AuthorizeAsync(User, ticketDto.ProjectId, "ProjectContributor");

            if (!authorized.Succeeded)
                return Forbid();

            var ticket = _mapper.Map<Ticket>(ticketDto);
            return HandleResult(await _mediator.Send(new Create.Command { Ticket = ticket }));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditTicket(Guid id, [FromBody] EditTicketDto editDto)
        {
            var ticketResult = await _mediator.Send(new Details.Query { Id = id });
            if (ticketResult.Value == null)
            {
                return NotFound();
            }
            
            var ticket = ticketResult.Value;
            
            var globalRoleClaim = User.FindFirst("globalrole")?.Value;
            var isGlobalAdmin = globalRoleClaim == "Admin";
            
            if (!isGlobalAdmin)
            {
                var currentUserId = _userAccessor.GetUserId();
                var currentUser = await _context.Users
                    .AsNoTracking()
                    .Where(u => u.Id == currentUserId)
                    .Select(u => u.UserName)
                    .FirstOrDefaultAsync();
                
                var isSubmitter = ticket.Submitter == currentUser;
                var isAssigned = ticket.Assigned == currentUser;
                
                if (!isSubmitter && !isAssigned)
                {
                    var authorized = await HttpContext.RequestServices
                        .GetService<IAuthorizationService>()
                        .AuthorizeAsync(User, ticket, "CanEditTicket");

                    if (!authorized.Succeeded)
                    {
                        return Forbid();
                    }
                }
            }

            return HandleResult(await _mediator.Send(new Edit.Command { EditDto = editDto, Id = id }));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTicket(Guid id)
        {
            var ticketResult = await _mediator.Send(new Details.Query { Id = id });
            if (ticketResult.Value == null)
            {
                return NotFound();
            }
            
            var ticket = ticketResult.Value;
            
            var globalRoleClaim = User.FindFirst("globalrole")?.Value;
            var isGlobalAdmin = globalRoleClaim == "Admin";
            
            if (!isGlobalAdmin)
            {
                var currentUserId = _userAccessor.GetUserId();
                var currentUser = await _context.Users
                    .AsNoTracking()
                    .Where(u => u.Id == currentUserId)
                    .Select(u => u.UserName)
                    .FirstOrDefaultAsync();
                
                var isSubmitter = ticket.Submitter == currentUser;
                
                if (!isSubmitter)
                {
                    var authorized = await HttpContext.RequestServices
                        .GetService<IAuthorizationService>()
                        .AuthorizeAsync(User, ticket, "CanDeleteTicket");

                    if (!authorized.Succeeded)
                    {
                        return Forbid();
                    }
                }
            }

            return HandleResult(await _mediator.Send(new Delete.Command { Id = id }));
        }

        [HttpGet("admin/deleted")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<ActionResult<List<TicketDto>>> GetDeletedTickets()
        {
            var result = await _mediator.Send(new ListDeleted.Query());
            var dtoList = result.Value.Select(t => _mapper.Map<TicketDto>(t)).ToList();
            return Ok(dtoList);
        }

        [HttpDelete("{id}/admin-delete")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> AdminDeleteTicket(Guid id)
        {
            var ticketResult = await _mediator.Send(new Details.Query { Id = id });
            if (ticketResult.Value == null)
            {
                return NotFound();
            }
            
            var ticket = ticketResult.Value;
            
            if (!ticket.IsDeleted)
            {
                return BadRequest("Ticket must be soft deleted first");
            }

            // Hard delete the ticket
            _context.Tickets.Remove(ticket);
            var result = await _context.SaveChangesAsync() > 0;
            
            if (!result) return BadRequest("Failed to permanently delete ticket");
            
            return NoContent();
        }

        [HttpPut("{id}/restore")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> RestoreTicket(Guid id)
        {
            var ticket = await _context.Tickets
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == id);
                
            if (ticket == null)
            {
                return NotFound();
            }
            
            if (!ticket.IsDeleted)
            {
                return BadRequest("Ticket is not deleted");
            }

            // Restore the ticket
            ticket.IsDeleted = false;
            ticket.DeletedDate = null;
            
            var result = await _context.SaveChangesAsync() > 0;
            
            if (!result) return BadRequest("Failed to restore ticket");
            
            return NoContent();
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