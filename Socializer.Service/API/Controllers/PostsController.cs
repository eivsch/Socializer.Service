using Logic.Managers;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IRandomUserPostManager _randomUserPostManager;
        private readonly IPostManager _postManager;

        public PostsController(ILogger<UsersController> logger, IRandomUserPostManager randomUserPostManager, IPostManager postManager)
        {
            _logger = logger;
            _randomUserPostManager = randomUserPostManager;
            _postManager = postManager;
        }

        [HttpPost]
        public async Task<IActionResult> AddRandomPost()
        {
            var post = await _randomUserPostManager.PostRandomTextFromRandomUser();
            if (post == null)
                return NoContent();

            return Ok(post);
        }

        [HttpGet("{postId}")]
        public async Task<IActionResult> GetPost(string postId)
        {
            if (postId.Length < 5)
                return BadRequest("Minimum the first 5 characters of the Post ID is required");

            var post = await _postManager.GetPost(postId);
            if (post == null)
                return NoContent();

            return Ok(post);
        }

        [HttpGet("{postId}/picture")]
        public async Task<IActionResult> GetPictureForPost(string postId)
        {
            if (postId.Length < 5)
                return BadRequest("Minimum the first 5 characters of the Post ID is required");

            var pic = await _postManager.GetPictureForPost(postId);

            return Ok(pic);
        }
    }
}
