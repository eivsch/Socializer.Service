using Logic;
using Logic.Models;
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
        public async Task<IActionResult> RegisterUser(RegisterUserRequest request)
        {
            var newUser = await _userRegistrationManager.RegisterUser(request);
            if (newUser == null)
                throw new Exception("User was not correctly registered");

            return Created("users", newUser);
        }

        [HttpPost("random")]
        public async Task<IActionResult> RegisterRandomizedUser()
        {
            var newUser = await _userRegistrationManager.GenerateRandomUserAndRegister();
            if (newUser == null)
                return NoContent();

            return Created("users/random", newUser);
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
