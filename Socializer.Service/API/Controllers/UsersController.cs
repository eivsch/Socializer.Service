using Logic;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;

        public UsersController(ILogger<UsersController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser()
        {
            var newUser = await new UserRegistrationManager().RegisterUser();

            return Ok(newUser);
        }

        [HttpGet("{userId}/feed")]
        public async Task<IActionResult> GetFeed([FromRoute] string userId, [FromQuery] int size, [FromQuery] string lastReadPostId)
        {

        }
    }
}
