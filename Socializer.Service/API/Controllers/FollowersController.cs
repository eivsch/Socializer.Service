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
        public async Task<IActionResult> AddFollower([FromBody] AddFollowerRequest request)
        {
            await _userManager.FollowUser(request);

            return Created("followers", request);
        }
    }
}
