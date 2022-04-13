using System.Net.Http;

namespace Infrastructure
{
    public interface IWebGalleryFileDownloader
    {
        Task<byte[]> DownloadImageFromFileServer(string appPathBase64, string webGalleryApiUser);
    }

    public class WebGalleryFileDownloader : IWebGalleryFileDownloader
    {
        const string FileServerUrl = "https://localhost:5056";

        private readonly IHttpClientFactory _clientFactory;

        public WebGalleryFileDownloader(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<byte[]> DownloadImageFromFileServer(string appPathBase64, string webGalleryApiUser)
        {
            HttpClient client = _clientFactory.CreateClient("HttpClientWithSSLUntrusted");
            client.DefaultRequestHeaders.Add("Gallery-User", webGalleryApiUser);

            var response = await client.GetAsync($"{FileServerUrl}/files/image?file={appPathBase64}");

            return await response.Content.ReadAsByteArrayAsync();
        }
    }
}
