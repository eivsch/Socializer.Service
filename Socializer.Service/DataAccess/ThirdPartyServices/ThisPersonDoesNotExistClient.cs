using System.Net;
using DomainModel.Generators;

namespace Infrastructure.ThirdPartyServices
{
    public class ThisPersonDoesNotExistClient : IProfilePicGenerator
    {
        public async Task<Stream> GeneratePicture()
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
