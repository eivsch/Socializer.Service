using System.Text;
using Infrastructure.WebGallery;

namespace Logic
{
    public interface IProfilePictureManager
    {
        Task<string> SaveProfilePictureForUser(string username, Stream profilePicture);
        Task<byte[]> GetProfilePictureForUser(string username);
    }

    public class ProfilePictureManager : IProfilePictureManager
    {
        private const string RootPath = "socializer_profilepics";

        private IWebGalleryFileServerClient _webGalleryFileDownloader;

        public ProfilePictureManager(IWebGalleryFileServerClient webGalleryFileServerClient)
        {
            _webGalleryFileDownloader = webGalleryFileServerClient;
        }

        string ResolveAppPath(string username) => Path.Combine(RootPath, username + ".jpg");

        public async Task<string> SaveProfilePictureForUser(string username, Stream profilePicture)
        {
            string filename = username + ".jpg";
            await _webGalleryFileDownloader.UploadImageToFileServer(RootPath, filename, profilePicture);
            
            return ResolveAppPath(username);
        }

        public async Task<byte[]> GetProfilePictureForUser(string username)
        {
            string path = ResolveAppPath(username);
            var appPathBytes = Encoding.UTF8.GetBytes(path);
            string appPathBase64 = Convert.ToBase64String(appPathBytes);

            return await _webGalleryFileDownloader.DownloadImageFromFileServer(appPathBase64);
        }
    }
}
