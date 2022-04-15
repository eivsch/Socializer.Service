using Logic;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IUserRegistrationManager _userRegistrationManager;
        private readonly IPostManager _postManager;

        public UsersController(ILogger<UsersController> logger, IUserRegistrationManager userRegistrationManager, IPostManager postManager)
        {
            _logger = logger;
            _userRegistrationManager = userRegistrationManager;
            _postManager = postManager;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser()
        {
            var newUser = await _userRegistrationManager.RegisterUser();
            if (newUser == null)
                return NoContent();

            return Ok(newUser);
        }

        [HttpGet("{username}/posts")]
        public async Task<IActionResult> GetPosts(string username)
        {
            var posts = await _postManager.GetPosts(username);

            if (posts == null)
                return NotFound();

            return Ok(posts);
        }
    }
}
