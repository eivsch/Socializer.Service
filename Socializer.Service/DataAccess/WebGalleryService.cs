using DomainModel.Posts;
using Newtonsoft.Json;

namespace Infrastructure
{
    public interface IWebGalleryService
    {
        Task<PostPicture> GetRandomPicture();
    }

    public class WebGalleryService : IWebGalleryService
    {
        private string _webGalleryApiEndpoint;
        private string _webGalleryUser;

        public WebGalleryService(string webGalleryApiEndpoint, string webGalleryApiUser)
        {
            _webGalleryApiEndpoint = webGalleryApiEndpoint;
            _webGalleryUser = webGalleryApiUser;
        }

        public async Task<PostPicture> GetRandomPicture()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(_webGalleryApiEndpoint);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("Gallery-User", _webGalleryUser);

            var request = new HttpRequestMessage(HttpMethod.Get, $"pictures/random");
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                var postPicture = JsonConvert.DeserializeObject<PictureDTO>(result);

                return new PostPicture
                {
                    PictureId = postPicture.Id,
                    PictureAppPath = postPicture.AppPath,
                    Tags = postPicture.Tags,
                };
            }
            else
            {
                throw new Exception($"The Web Gallery API returned a {response.StatusCode} status code.");
            }
        }
    }

    internal class PictureDTO
    {
        public string Id { get; set; }
        public string AppPath { get; set; }
        public IEnumerable<string> Tags { get; set; }
    }
}
