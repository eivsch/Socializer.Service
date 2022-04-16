using System.Text;
using DomainModel.Generators;
using Newtonsoft.Json;

namespace Infrastructure.ThirdPartyServices
{

    //Write a completely random social media post that has a political content:
    //"Danna is making her first ever post on the Socializer app. She is happy, clever and very friendly. Write her post:",

    // Write a completely random social media post by someone who posted a picture containing the following object: "person"
    // Frank posted a picture containing the object: "couch". Write a triumphant social media post:

    // Write a completely random social media post from someone who just made an account:

    public class Gpt3Client : IRandomTextGenerator
    {
        private readonly string _apiKey;

        public Gpt3Client(string apiKey)
        {
            _apiKey = apiKey;
        }

        public async Task<string?> GenerateRandomText()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
            
            var dto = new
            {
                prompt = "Write a completely random social media post that is informative:",
                temperature = 0.6,
                max_tokens = 120,
            };

            var jsonString = JsonConvert.SerializeObject(dto);

            var response = await client.PostAsync(
                "https://api.openai.com/v1/engines/text-davinci-002/completions", 
                new StringContent(jsonString, Encoding.UTF8, "application/json")
            );

            string result = await response.Content.ReadAsStringAsync();
            if (result != null)
            {
                var gptResponse = JsonConvert.DeserializeObject<GptResponseDTO>(result);
                
                return gptResponse?.Choices?.FirstOrDefault()?.Text ?? null;
            }

            return null;
        }

        class GptResponseDTO
        {
            public List<GptChoiceDTO> Choices { get; set; }
        }

        class GptChoiceDTO
        {
            public string Text { get; set; }
        }
    }
}
