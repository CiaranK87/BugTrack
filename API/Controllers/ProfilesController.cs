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
        public async Task<IActionResult> UpdateProfile(ProfileDto profileDto)
        {
            return HandleResult(await Mediator.Send(new UpdateProfile.Command { ProfileDto = profileDto }));
        }
    }
}