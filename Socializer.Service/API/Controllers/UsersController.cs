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

        public UsersController(ILogger<UsersController> logger, IUserRegistrationManager userRegistrationManager)
        {
            _logger = logger;
            _userRegistrationManager = userRegistrationManager;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser()
        {
            var newUser = await _userRegistrationManager.RegisterUser();

            return Ok(newUser);
        }
    }
}
