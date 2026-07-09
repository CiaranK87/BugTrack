using System.Security.Claims;
using Application.Account;
using Application.Core;
using Application.DTOs;
using API.Services;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : BaseApiController
    {
        private readonly TokenService _tokenService;

        public AccountController(IMediator mediator, IAuthorizationService authorizationService, TokenService tokenService)
            : base(mediator, authorizationService)
        {
            _tokenService = tokenService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login([FromBody] LoginDto loginDto)
        {
            var result = await Mediator.Send(new Login.Command { LoginDto = loginDto });
            if (!result.IsSuccess) return Unauthorized();
            return CreateUserObject(result.Value);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register([FromBody] RegisterDto registerDto)
        {
            var result = await Mediator.Send(new Register.Command { RegisterDto = registerDto });
            if (!result.IsSuccess)
            {
                ModelState.AddModelError("error", result.Error);
                return ValidationProblem();
            }
            return CreateUserObject(result.Value);
        }

        [Authorize(Policy = "CanManageGlobalRoles")]
        [HttpPost("admin/register")]
        public async Task<ActionResult<UserDto>> AdminRegister([FromBody] AdminRegisterDto registerDto)
        {
            var result = await Mediator.Send(new AdminRegister.Command { RegisterDto = registerDto });
            if (!result.IsSuccess)
            {
                ModelState.AddModelError("error", result.Error);
                return ValidationProblem();
            }
            return CreateUserObject(result.Value);
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var result = await Mediator.Send(new GetCurrentUser.Query { Email = email });
            if (!result.IsSuccess) return Unauthorized();
            return CreateUserObject(result.Value);
        }

        [Authorize]
        [HttpPost("changePassword")]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await Mediator.Send(new ChangePassword.Command
            {
                UserId = userId,
                ChangePasswordDto = changePasswordDto
            });
            if (!result.IsSuccess) return BadRequest(result.Error);
            return Ok("Password changed successfully");
        }

        private UserDto CreateUserObject(AppUser user)
        {
            return new UserDto
            {
                DisplayName = user.DisplayName,
                Token = _tokenService.CreateToken(user),
                Username = user.UserName,
                GlobalRole = user.GlobalRole ?? Roles.Global.User,
                Image = user.AvatarBlobName != null ? AvatarUrl.For(user.UserName) : null
            };
        }
    }
}
