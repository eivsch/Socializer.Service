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
        private readonly IWebGalleryService _webGalleryService;

        public ImagesController(ILogger<UsersController> logger, IWebGalleryFileDownloader webGalleryFileDownloader, IWebGalleryService webGalleryService)
        {
            _logger = logger;
            _webGalleryFileDownloader = webGalleryFileDownloader;
            _webGalleryService = webGalleryService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> WebGalleryPictureById(string id)
        {
            var pic = await _webGalleryService.GetPictureById(id);
            var appPathBytes = System.Text.Encoding.UTF8.GetBytes(pic.PictureAppPath);
            string appPathBase64 = Convert.ToBase64String(appPathBytes);
            
            var fileBytes = await _webGalleryFileDownloader.DownloadImageFromFileServer(appPathBase64, "testuser");

            return new FileContentResult(fileBytes, "image/jpeg");
        }

        [HttpGet("webgallery/{appPathBase64}")]
        public async Task<IActionResult> WebGalleryPictureByAppPath(string appPathBase64)
        {
            var fileBytes = await _webGalleryFileDownloader.DownloadImageFromFileServer(appPathBase64, "testuser");

            return new FileContentResult(fileBytes, "image/jpeg");
        }

        [HttpGet("profiles/{username}")]
        public IActionResult ProfilePicture(string username)
        {
            var fileBytes = new ProfilePictureService().GetProfilePictureFromDisk(username);

            return new FileContentResult(fileBytes, "image/jpeg");
        }
    }
}
