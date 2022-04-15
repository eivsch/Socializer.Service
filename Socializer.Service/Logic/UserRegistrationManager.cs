using DomainModel.FeedEvents;
using DomainModel.Users;
using DomainModel.FeedEvents.Interfaces;
using DomainModel.Generators;
using Newtonsoft.Json;

namespace Logic
{
    public interface IUserRegistrationManager
    {
        Task<User?> RegisterUser();
    }

    public class UserRegistrationManager : IUserRegistrationManager
    {
        private readonly IUserRepository _userRepository;
        private readonly IFeedEventRepository _feedEventRepository;
        private readonly IUserGenerator _userGenerator;

        public UserRegistrationManager(
            IUserRepository userRepository, 
            IFeedEventRepository feedEventRepository,
            IUserGenerator userGenerator)
        {
            _userRepository = userRepository;
            _feedEventRepository = feedEventRepository;
            _userGenerator = userGenerator;
        }

        public async Task<User?> RegisterUser()
        {
            var user = await _userGenerator.GenerateUser();
            if (user == null)
                return null;

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
                ProfilePictureUri = newUser.ProfilePicture.PictureUri
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