using Application.DTOs;
using Application.Profiles;
using Microsoft.AspNetCore.Mvc;


namespace API.Controllers
{

    [ApiController]
    public class ProfilesController : BaseApiController
    {
        [HttpGet("{username}")]
        public async Task<IActionResult> GetProfile(string username)
        {
            return HandleResult(await Mediator.Send(new GetProfile.Query { Username = username }));
        }

        [HttpGet("{username}/projects")]
        public async Task<IActionResult> GetUserProjects(string username)
        {
            return HandleResult(await Mediator.Send(new GetUserProjects.Query { Username = username }));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile(ProfileDto profileDto)
        {
            return HandleResult(await Mediator.Send(new UpdateProfile.Command { ProfileDto = profileDto }));
        }
    }
}