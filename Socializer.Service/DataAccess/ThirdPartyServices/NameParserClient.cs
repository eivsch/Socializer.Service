using System.Net;
using DomainModel;
using DomainModel.Generators;
using DomainModel.Users;
using Newtonsoft.Json;

namespace Infrastructure.ThirdPartyServices
{
    public class NameParserClient : INameGenerator
    {
        private readonly string _apiKey;

        public NameParserClient(string apiKey)
        {
            _apiKey = apiKey;
        }

        public async Task<UserPersonalName?> GenerateName(string country = "US", GenderType gender = GenderType.Male)
        {
            string genderParam = gender == GenderType.Male ? "m" : "f";

            HttpClient client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.parser.name/?api_key={_apiKey}&endpoint=generate&country_code={country}&gender={genderParam}");
            var respone = client.Send(request);
            if (respone.StatusCode == HttpStatusCode.OK)
            {
                string resultString = await respone.Content.ReadAsStringAsync();
                if (resultString != null)
                {
                    var response = JsonConvert.DeserializeObject<NameParserResponseDTO>(resultString);

                    if (response != null)
                    {
                        string firstname = response.Data.First().Name.Firstname.Name;
                        string lastname = response.Data.First().Name.Lastname.Name;

                        return new UserPersonalName
                        {
                            Firstname = firstname,
                            Lastname = lastname
                        };
                    }
                }

                return null;
            }

            throw new Exception("Unable to generate username - None or bad response from api.parser.name");
        }

        #region DTOs
        class NameParserResponseDTO
        {
            public List<NameParserDataResponseDTO> Data { get; set; }
        }

        class NameParserDataResponseDTO
        {
            public NameParserNameContainerDTO Name { get; set; }
        }

        class NameParserNameContainerDTO
        {
            public NameParserNameDTO Firstname { get; set; }
            public NameParserNameDTO Lastname { get; set; }
        }

        class NameParserNameDTO
        {
            public string Name { get; set; }
        }
        #endregion DTOs
    }
}
