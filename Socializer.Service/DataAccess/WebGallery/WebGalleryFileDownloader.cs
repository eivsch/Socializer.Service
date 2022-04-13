namespace Infrastructure.WebGallery
{
    public interface IWebGalleryFileDownloader
    {
        Task<byte[]> DownloadImageFromFileServer(string appPathBase64);
    }

    public class WebGalleryFileDownloader : IWebGalleryFileDownloader
    {
        private string _fileServerUrl;
        private string _webGalleryApiUser;
        private IHttpClientFactory _clientFactory;

        public WebGalleryFileDownloader(IWebGalleryOptions webGalleryOptions, IHttpClientFactory clientFactory)
        {
            _fileServerUrl = webGalleryOptions.FileServerEndpoint;
            _webGalleryApiUser = webGalleryOptions.User;
            _clientFactory = clientFactory;
        }
        
        public async Task<byte[]> DownloadImageFromFileServer(string appPathBase64)
        {
            HttpClient client = _clientFactory.CreateClient("HttpClientWithSSLUntrusted");
            client.DefaultRequestHeaders.Add("Gallery-User", _webGalleryApiUser);

            var response = await client.GetAsync($"{_fileServerUrl}/files/image?file={appPathBase64}");

            return await response.Content.ReadAsByteArrayAsync();
        }

        //public async Task UploadFileToFileServer(string albumname, string filename, Stream file)
        //{
        //    HttpClient client = _clientFactory.CreateClient("HttpClientWithSSLUntrusted");
        //    client.DefaultRequestHeaders.Add("Gallery-User", webGalleryApiUser);

        //    using (var content = new MultipartFormDataContent())
        //    {
        //        content.Add(new StreamContent(file), albumname, filename);

        //        var response = await client.PostAsync($"{FileServerUrl}/files", content);
        //        if (response.IsSuccessStatusCode)
        //        {
        //            using var responseStream = await response.Content.ReadAsStreamAsync();
        //            var responseData = await JsonSerializer.DeserializeAsync<SavedFileInfo>(responseStream);

        //            return new SavedFileInfo
        //            {
        //                FileName = responseData.FileName,
        //                FilePathFull = responseData.FilePathFull,
        //                FileSize = responseData.FileSize
        //            };
        //        }

        //        throw new Exception($"The API returned a {response.StatusCode} status code.");
        //    }
        //}
    }
}
