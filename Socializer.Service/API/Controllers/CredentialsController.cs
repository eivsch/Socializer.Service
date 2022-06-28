using Logic.Managers;
using Logic.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CredentialsController : ControllerBase
    {
        ICredentialsManager _credentialsManager;

        public CredentialsController(ICredentialsManager credentialsManager)
        {
            _credentialsManager = credentialsManager;
        }

        [HttpPost("validation")]
        public async Task<IActionResult> ValidateCredentials(ValidateCredentialsRequest request)
        {
            var response = await _credentialsManager.ValidateCredentials(request);

            return Ok(response);
        }

        //[HttpPost("tokens/validation")]
        //public async Task<IActionResult> ValidateCredentials(ValidateCredentialsRequest request)
        //{
        //    var response = _credentialsManager.ValidateCredentials(request);

        //    return Ok(response);
        //}
    }
}
