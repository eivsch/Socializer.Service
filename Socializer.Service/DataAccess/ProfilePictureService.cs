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

        public byte[] GetProfilePictureFromDisk(string username)
        {
            string path = Path.Combine(RootPath, $"{username}.jpg");

            var fileBytes = File.ReadAllBytes(path);

            return fileBytes;
        }
    }
}
