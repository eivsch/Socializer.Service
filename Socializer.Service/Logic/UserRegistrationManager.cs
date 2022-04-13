using System.Net;
using Infrastructure;
using DomainModel.FeedEvents;
using DomainModel.Users;
using Newtonsoft.Json;
using DomainModel.FeedEvents.Interfaces;

namespace Logic
{
    public interface IUserRegistrationManager
    {
        Task<User> RegisterUser();
    }

    public class UserRegistrationManager : IUserRegistrationManager
    {
        private readonly IUserRepository _userRepository;
        private readonly IFeedEventRepository _feedEventRepository;

        public UserRegistrationManager(IUserRepository userRepository, IFeedEventRepository feedEventRepository)
        {
            _userRepository = userRepository;
            _feedEventRepository = feedEventRepository;
        }

        public async Task<User> RegisterUser()
        {
            string username = await GenerateUserName();
            // Make sure the username is unique
            while (true)
            {
                var existingUser = await _userRepository.GetUserByName(username);
                if (existingUser != null)
                    username = await GenerateUserName();
                else
                    break;
            }

            var user = new User
            {
                UserId = Guid.NewGuid().ToString(),
                UserCreated = DateTime.UtcNow,
                Username = username,
            };

            string profilePicPath = new ProfilePictureService().DownloadRandomProfilePicture(user.Username);
            user.ProfilePicturePath = profilePicPath;

            await _userRepository.Save(user);

            await RegisterNewUserEvent(user);

            return user;
        }

        private async Task<string> GenerateUserName()
        {
            HttpClient client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://randommer.io/api/Name?nametype=fullname&quantity=1");
            request.Headers.Add("X-Api-Key", "76c63aed24cc4096a3aa15986526c137");
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

        private async Task RegisterNewUserEvent(User newUser)
        {
            var userData = new
            {
                UserId = newUser.UserId,
                Username = newUser.Username,
                ProfilePicturePath = newUser.ProfilePicturePath
            };

            var feedEvent = new FeedEvent
            {
                Id = Guid.NewGuid().ToString(),
                EventCreated = DateTime.UtcNow,
                EventType = "UserRegistered",
                EventDataJson = JsonConvert.SerializeObject(userData)
            };

            await _feedEventRepository.AddEventToQueue(feedEvent);
        }
    }
}