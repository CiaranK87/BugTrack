using API.Helpers;
using Application.DTOs;
using Application.Profiles;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace API.Controllers
{

    [ApiController]
    public class ProfilesController(IMediator mediator, IAuthorizationService authorizationService) : BaseApiController(mediator, authorizationService)
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
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(ext))
                return BadRequest("Only image files are allowed (jpg, jpeg, png, gif, webp)");

            var upload = new FileUploadDto
            {
                Content = file.OpenReadStream(),
                FileName = file.FileName,
                ContentType = ContentTypeHelper.FromExtension(ext),
                Length = file.Length
            };
            return HandleResult(await Mediator.Send(new UploadAvatar.Command { File = upload }));
        }

        [Authorize]
        [HttpDelete("avatar")]
        public async Task<IActionResult> DeleteAvatar()
        {
            var result = await Mediator.Send(new DeleteAvatar.Command());
            if (!result.IsSuccess) return HandleResult(result);
            return NoContent();
        }
    }
}