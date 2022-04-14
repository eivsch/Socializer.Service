namespace Infrastructure.WebGallery
{
    public interface IWebGalleryFileServerClient
    {
        Task<byte[]> DownloadImageFromFileServer(string appPathBase64);
        Task UploadFileToFileServer(string albumname, string filename, Stream file);
    }

    public class WebGalleryFileServerClient : IWebGalleryFileServerClient
    {
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

        public async Task<byte[]> DownloadImageFromFileServer(string appPathBase64)
        {
            var response = await _client.GetAsync($"{_fileServerUrl}/files/image?file={appPathBase64}");

            return await response.Content.ReadAsByteArrayAsync();
        }

        public async Task UploadFileToFileServer(string albumname, string filename, Stream file)
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
