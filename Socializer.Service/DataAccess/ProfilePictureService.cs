using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class ProfilePictureService
    {
        const string RootPath = @"C:\temp\Pics\Profiles";

        public string DownloadRandomProfilePicture(string username)
        {
            string savePath = Path.Combine(RootPath, $"{username}.jpg");

            WebClient webClient = new WebClient();
            webClient.DownloadFileAsync(new Uri("https://thispersondoesnotexist.com/image"), savePath);

            return savePath;
        }

        public async Task<Stream> DownloadRandomProfilePicture2(string username)
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

        public byte[] GetProfilePictureFromDisk(string username)
        {
            string path = Path.Combine(RootPath, $"{username}.jpg");

            var fileBytes = File.ReadAllBytes(path);

            return fileBytes;
        }
    }
}
