using System.Net;
using DomainModel.Generators;
using DomainModel.Users;

namespace Infrastructure.ThirdPartyServices
{
    public class ThisPersonDoesNotExistClient : IProfilePicGenerator
    {
        private readonly IFileServerClient _fileServerClient;

        public ThisPersonDoesNotExistClient(IFileServerClient fileServerClient)
        {
            _fileServerClient = fileServerClient;
        }

        public async Task<UserProfilePicture> GeneratePicture(User user)
        {
            var imageFile = await DownloadGeneratedPicture();

            var pic = await _fileServerClient.UploadUserProfileImage(user, imageFile);

            return pic;
        }

        private async Task<Stream> DownloadGeneratedPicture()
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
