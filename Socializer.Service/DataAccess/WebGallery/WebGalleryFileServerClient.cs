using System.Text;
using DomainModel.Posts;
using DomainModel.Users;

namespace Infrastructure.WebGallery
{
    public class WebGalleryFileServerClient : IFileServerClient
    {
        private const string ProfilePicturesSubPath = "socializer_profilepics";

        private string _fileServerUrl;
        private string _webGalleryApiUser;
        private HttpClient _client;

        public WebGalleryFileServerClient(IWebGalleryOptions webGalleryOptions, IHttpClientFactory clientFactory)
        {
            _fileServerUrl = webGalleryOptions.FileServerEndpoint;
            _webGalleryApiUser = webGalleryOptions.User;

            _client = clientFactory.CreateClient("HttpClientWithSSLUntrusted");
            _client.DefaultRequestHeaders.Add("Gallery-User", _webGalleryApiUser);
        }

        string ResolveProfilePictureAppPath(User user) => Path.Combine(ProfilePicturesSubPath, user.Username + ".jpg");

        public async Task<byte[]> DownloadUserProfilePicture(User user)
        {
            string appPath = ResolveProfilePictureAppPath(user);
            var appPathBytes = Encoding.UTF8.GetBytes(appPath);
            string appPathBase64 = Convert.ToBase64String(appPathBytes);

            return await DownloadImageFromFileServer(appPathBase64);
        }

        public async Task<UserProfilePicture> UploadUserProfileImage(User user, Stream imageFile)
        {
            await UploadImageToFileServer(ProfilePicturesSubPath, user.Username + ".jpg", imageFile);

            return new UserProfilePicture
            {
                PictureUri = ResolveProfilePictureAppPath(user)
            };
        }

        public Task<PostPicture> UploadPostImage(Post post, Stream imageFile)
        {
            throw new NotImplementedException();
        }

        public async Task<byte[]> DownloadImage(string uri)
        {
            return await DownloadImageFromFileServer(uri);
        }

        private async Task<byte[]> DownloadImageFromFileServer(string appPathBase64)
        {
            var response = await _client.GetAsync($"{_fileServerUrl}/files/image?file={appPathBase64}");

            return await response.Content.ReadAsByteArrayAsync();
        }

        private async Task UploadImageToFileServer(string albumname, string filename, Stream file)
        {
            using (var content = new MultipartFormDataContent())
            {
                content.Add(new StreamContent(file), albumname, filename);

                var response = await _client.PostAsync($"{_fileServerUrl}/files", content);
                if (!response.IsSuccessStatusCode)
                    throw new Exception($"The Web Gallery file server returned a {response.StatusCode} status code.");
            }
        }
    }
}
