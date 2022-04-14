using System.Text;
using Infrastructure.ThirdPartyServices;
using Infrastructure.WebGallery;

namespace Logic
{
    public interface IProfilePictureManager
    {
        Task<string> GenerateProfilePictureForUser(string username);
        Task<byte[]> GetProfilePictureBytes(string username);
    }

    public class ProfilePictureManager : IProfilePictureManager
    {
        private const string RootPath = "socializer_profilepics";

        private IThisPersonDoesNotExistClient _thisPersonDoesNotExistClient;
        private IWebGalleryFileServerClient _webGalleryFileDownloader;

        public ProfilePictureManager(IThisPersonDoesNotExistClient thisPersonDoesNotExistClient, IWebGalleryFileServerClient webGalleryFileServerClient)
        {
            _thisPersonDoesNotExistClient = thisPersonDoesNotExistClient;
            _webGalleryFileDownloader = webGalleryFileServerClient;
        }

        string ResolveAppPath(string username) => Path.Combine(RootPath, username + ".jpg");

        public async Task<string> GenerateProfilePictureForUser(string username)
        {
            Stream profilePicStream = await _thisPersonDoesNotExistClient.DownloadRandomGeneratedPicture();
            string filename = username + ".jpg";
            await _webGalleryFileDownloader.UploadFileToFileServer(RootPath, filename, profilePicStream);
            
            return ResolveAppPath(username);
        }

        public async Task<byte[]> GetProfilePictureBytes(string username)
        {
            string path = ResolveAppPath(username);
            var appPathBytes = Encoding.UTF8.GetBytes(path);
            string appPathBase64 = Convert.ToBase64String(appPathBytes);

            return await _webGalleryFileDownloader.DownloadImageFromFileServer(appPathBase64);
        }
    }
}
