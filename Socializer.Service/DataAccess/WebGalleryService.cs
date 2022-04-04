using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainModel.Posts;
using Newtonsoft.Json;

namespace Infrastructure
{
    public class WebGalleryService
    {
        const string WebGalleryApiEndpoint = "https://webgallery-api.azurewebsites.net";

        public async Task<PostPicture> GetRandomPicture(string webGalleryApiUser)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(WebGalleryApiEndpoint);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("Gallery-User", webGalleryApiUser);

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
