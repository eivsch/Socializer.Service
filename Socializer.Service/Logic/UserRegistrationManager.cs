using DomainModel.FeedEvents;
using DomainModel.Users;
using Newtonsoft.Json;
using DomainModel.FeedEvents.Interfaces;
using DomainModel.Generators;

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
        private readonly IProfilePictureManager _profilePictureManager;
        private readonly IProfilePicGenerator _profilePicGenerator;
        private readonly IUserNameGenerator _userNameGenerator;

        public UserRegistrationManager(
            IUserRepository userRepository, 
            IFeedEventRepository feedEventRepository, 
            IProfilePictureManager profilePictureManager,
            IProfilePicGenerator profilePicGenerator,
            IUserNameGenerator userNameGenerator)
        {
            _userRepository = userRepository;
            _feedEventRepository = feedEventRepository;
            _profilePictureManager = profilePictureManager;
            _profilePicGenerator = profilePicGenerator;
            _userNameGenerator = userNameGenerator;
        }

        public async Task<User> RegisterUser()
        {
            string username = await _userNameGenerator.GenerateUserName();
            // Make sure the username is unique
            while (true)
            {
                var existingUser = await _userRepository.GetUserByName(username);
                if (existingUser != null)
                    username = await _userNameGenerator.GenerateUserName();
                else
                    break;
            }

            var user = new User
            {
                UserId = Guid.NewGuid().ToString(),
                UserCreated = DateTime.UtcNow,
                Username = username,
            };

            Stream profilePicture = await _profilePicGenerator.GeneratePicture();
            string appPath = await _profilePictureManager.SaveProfilePictureForUser(username, profilePicture);
            user.ProfilePicturePath = appPath;

            await _userRepository.Save(user);
            await RegisterNewUserEvent(user);

            return user;
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