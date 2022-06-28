using Logic.Managers;
using Logic.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FollowersController : ControllerBase
    {
        IUserManager _userManager;

        public FollowersController(IUserManager userManager)
        {
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> AddFollower([FromHeader] string socializerToken, [FromBody] AddFollowerRequest request)
        {
            if (request.UserToFollowId.Length < 5)
                return BadRequest("Minimum the first 5 characters of the UserId to follow is required");

            string token = socializerToken.Trim();
            request.CurrentUserToken = token;

            await _userManager.FollowUser(request);

            return Ok();
        }
    }
}
