using Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImagesController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IWebGalleryFileDownloader _webGalleryFileDownloader;

        public ImagesController(ILogger<UsersController> logger, IWebGalleryFileDownloader webGalleryFileDownloader)
        {
            _logger = logger;
            _webGalleryFileDownloader = webGalleryFileDownloader;
        }

        [HttpGet("profiles/{username}")]
        public IActionResult ProfilePicture(string username)
        {
            var fileBytes = new ProfilePictureService().GetProfilePictureFromDisk(username);

            return new FileContentResult(fileBytes, "image/jpeg");
        }

        [HttpGet("webgallery/{appPathBase64}")]
        public async Task<IActionResult> WebGalleryPicture(string appPathBase64)
        {
            var fileBytes = await _webGalleryFileDownloader.DownloadImageFromFileServer(appPathBase64, "testuser");

            return new FileContentResult(fileBytes, "image/jpeg");
        }
    }
}
