using Infrastructure.WebGallery;
using Logic;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImagesController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IWebGalleryFileServerClient _webGalleryFileServerClient;
        private readonly IWebGalleryService _webGalleryService;
        private readonly IProfilePictureManager _profilePictureManager;

        public ImagesController(
            ILogger<UsersController> logger, 
            IWebGalleryFileServerClient webGalleryFileDownloader, 
            IWebGalleryService webGalleryService,
            IProfilePictureManager profilePictureManager)
        {
            _logger = logger;
            _webGalleryFileServerClient = webGalleryFileDownloader;
            _webGalleryService = webGalleryService;
            _profilePictureManager = profilePictureManager;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> WebGalleryPictureById(string id)
        {
            var pic = await _webGalleryService.GetPictureById(id);
            var appPathBytes = System.Text.Encoding.UTF8.GetBytes(pic.PictureAppPath);
            string appPathBase64 = Convert.ToBase64String(appPathBytes);
            
            var fileBytes = await _webGalleryFileServerClient.DownloadImageFromFileServer(appPathBase64);

            return new FileContentResult(fileBytes, "image/jpeg");
        }

        [HttpGet("webgallery/{appPathBase64}")]
        public async Task<IActionResult> WebGalleryPictureByAppPath(string appPathBase64)
        {
            var fileBytes = await _webGalleryFileServerClient.DownloadImageFromFileServer(appPathBase64);

            return new FileContentResult(fileBytes, "image/jpeg");
        }

        [HttpGet("profiles/{username}")]
        public async Task<IActionResult> ProfilePicture(string username)
        {
            var fileBytes = await _profilePictureManager.GetProfilePictureBytes(username);

            return new FileContentResult(fileBytes, "image/jpeg");
        }
    }
}
