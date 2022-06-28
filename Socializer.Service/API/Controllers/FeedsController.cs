using Logic;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FeedsController : ControllerBase
    {
        IFeedService _feedService;

        public FeedsController(IFeedService feedService)
        {
            _feedService = feedService;
        }

        [HttpGet]
        public async Task<IActionResult> GetFeed([FromQuery] int size, [FromHeader] string socializerToken)
        {
            var posts = await _feedService.GetFeed(size, socializerToken);

            return Ok(posts);
        }
    }
}
