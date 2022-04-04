using Logic;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;

        public PostsController(ILogger<UsersController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> AddRandomPost()
        {
            await new RandomUserPostManager().PostRandomTextFromRandomUser();

            return Ok();
        }

        // TODO: Move to users controller? users/dfsfs.abc/posts
        [HttpGet]
        public async Task<IActionResult> GetPosts(string username)
        {
            var posts = await new PostManager().GetPosts(username);

            if (posts == null)
                return NotFound();

            return Ok(posts);
        }

        [HttpGet("{postId}")]
        public async Task<IActionResult> GetPost(string postId)
        {
            if (postId.Length < 5)
                return BadRequest("Minimum the first 5 characters of the Post ID is required");

            var post = await new PostManager().GetPost(postId);
            if (post == null)
                return NoContent();

            return Ok(post);
        }

        [HttpGet("{postId}/picture")]
        public async Task<IActionResult> GetPictureForPost(string postId)
        {
            if (postId.Length < 5)
                return BadRequest("Minimum the first 5 characters of the Post ID is required");

            var pic = await new PostManager().GetPictureForPost(postId);

            return Ok(pic);
        }
    }
}
