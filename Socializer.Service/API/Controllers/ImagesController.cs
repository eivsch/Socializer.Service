using Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImagesController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IFileServerClient _fileServerClient;

        public ImagesController(
            ILogger<UsersController> logger, 
            IFileServerClient fileServerClient)
        {
            _logger = logger;
            _fileServerClient = fileServerClient;
        }

        //[HttpGet("{id}")]
        //public async Task<IActionResult> WebGalleryPictureById(string id)
        //{
        //    var pic = await _webGalleryService.GetPictureById(id);
        //    var appPathBytes = System.Text.Encoding.UTF8.GetBytes(pic.PictureAppPath);
        //    string appPathBase64 = Convert.ToBase64String(appPathBytes);
            
        //    var fileBytes = await _webGalleryFileServerClient.DownloadImageFromFileServer(appPathBase64);

        //    return new FileContentResult(fileBytes, "image/jpeg");
        //}

        [HttpGet("{uri}")]
        public async Task<IActionResult> WebGalleryPictureByAppPath(string uri)
        {
            var fileBytes = await _fileServerClient.DownloadImage(uri);

            return new FileContentResult(fileBytes, "image/jpeg");
        }

        [HttpGet("profiles/{username}")]
        public async Task<IActionResult> ProfilePicture(string username)
        {
            var fileBytes = await _fileServerClient.DownloadUserProfilePicture(new DomainModel.Users.User("", username));

            return new FileContentResult(fileBytes, "image/jpeg");
        }
    }
}
