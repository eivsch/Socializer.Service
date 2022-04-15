using System.Net;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        const string ApiKey = "e5ea205e054a5ca620415395f527ec21";

        public TestController()
        {

        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            //var faceReqClient = new FaceReqClient();
            //faceReqClient.Detect();

            HttpClient client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.parser.name/?api_key={ApiKey}&endpoint=generate&country_code=US&gender=f");
            var respone = await client.SendAsync(request);
            if (respone.StatusCode == HttpStatusCode.OK)
            {
                var resultString = await respone.Content.ReadAsStringAsync();

                if (resultString != null)
                {
                    var response = JsonConvert.DeserializeObject<NameParserResponseDTO>(resultString);

                    string nm = response.Data.First().Name.Firstname.Name;
                }
            }

            return Ok();
        }
    }

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
}
