using System.Net;

namespace Infrastructure.ThirdPartyServices
{
    public interface IThisPersonDoesNotExistClient
    {
        Task<Stream> DownloadRandomGeneratedPicture();
    }

    public class ThisPersonDoesNotExistClient : IThisPersonDoesNotExistClient
    {
        public async Task<Stream> DownloadRandomGeneratedPicture()
        {
            HttpClient client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://thispersondoesnotexist.com/image");
            var respone = await client.SendAsync(request);
            if (respone.StatusCode == HttpStatusCode.OK)
            {
                var resultStream = await respone.Content.ReadAsStreamAsync();

                return resultStream;
            }

            throw new Exception("Unable to generate profile picture - None or bad response from thispersondonotexist.com");
        }
    }
}
