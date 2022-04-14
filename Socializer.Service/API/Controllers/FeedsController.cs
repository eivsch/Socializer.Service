using Logic;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FeedsController : ControllerBase
    {
        IPostManager _postManager;

        public FeedsController(IPostManager postManager)
        {
            _postManager = postManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetFeed([FromQuery] int size)
        {
            var posts = await _postManager.GetAll(size);

            return Ok(posts);
        }
    }
}
