using Application.DTOs;
using Application.Profiles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace API.Controllers
{

    [ApiController]
    public class ProfilesController(IAuthorizationService authorizationService) : BaseApiController(authorizationService)
    {
        [Authorize]
        [HttpGet("{username}")]
        public async Task<IActionResult> GetProfile(string username)
        {
            return HandleResult(await Mediator.Send(new GetProfile.Query { Username = username }));
        }

        [Authorize]
        [HttpGet("{username}/projects")]
        public async Task<IActionResult> GetUserProjects(string username)
        {
            return HandleResult(await Mediator.Send(new GetUserProjects.Query { Username = username }));
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto profileDto)
        {
            return HandleResult(await Mediator.Send(new UpdateProfile.Command { ProfileDto = profileDto }));
        }

        [AllowAnonymous]
        [HttpGet("{username}/avatar")]
        public async Task<IActionResult> GetAvatar(string username)
        {
            var (stream, contentType) = await Mediator.Send(new GetAvatar.Query { Username = username });
            if (stream == null) return NotFound();
            Response.Headers["Cache-Control"] = "no-cache, no-store";
            return File(stream, contentType);
        }

        [Authorize]
        [HttpPost("avatar")]
        public async Task<IActionResult> UploadAvatar([FromForm] IFormFile file)
        {
            return HandleResult(await Mediator.Send(new UploadAvatar.Command { File = file }));
        }

        [Authorize]
        [HttpDelete("avatar")]
        public async Task<IActionResult> DeleteAvatar()
        {
            return HandleResult(await Mediator.Send(new DeleteAvatar.Command()));
        }
    }
}