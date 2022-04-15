using System.Net;
using DomainModel.Generators;
using Newtonsoft.Json;

namespace Infrastructure.ThirdPartyServices
{
    public class RandommerClient : IRandomTextGenerator
    {
        string _randommerApiEndpoint;
        string _randommerApiKey;

        public RandommerClient(string randommerApiEndpoint, string randommerApiKey)
        {
            _randommerApiEndpoint = randommerApiEndpoint;
            _randommerApiKey = randommerApiKey;
        }

        public async Task<string> GenerateRandomText()
        {
            HttpClient client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_randommerApiEndpoint}/api/Text/LoremIpsum?loremType=normal&type=words&number=4");
            request.Headers.Add("X-Api-Key", _randommerApiKey);
            var respone = client.Send(request);
            if (respone.StatusCode == HttpStatusCode.OK)
            {
                string result = await respone.Content.ReadAsStringAsync();
                if (result != null)
                {
                    string text = JsonConvert.DeserializeObject<string>(result);
                    return text;
                }
            }

            throw new Exception("Unable to generate text - None or bad response from randommer.io");
        }

        [Obsolete]
        public async Task<string> GenerateUserName()
        {
            HttpClient client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_randommerApiEndpoint}/api/Name?nametype=fullname&quantity=1");
            request.Headers.Add("X-Api-Key", _randommerApiKey);
            var respone = client.Send(request);
            if (respone.StatusCode == HttpStatusCode.OK)
            {
                string result = await respone.Content.ReadAsStringAsync();
                if (result != null)
                {
                    var names = JsonConvert.DeserializeObject<IEnumerable<string>>(result);
                    string username = names.First().Replace(" ", ".").ToLower();

                    return username;
                }
            }

            throw new Exception("Unable to generate username - None or bad response from randommer.io");
        }
    }
}
