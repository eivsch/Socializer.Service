using DomainModel.FeedEvents;
using DomainModel.Users;
using DomainModel.FeedEvents.Interfaces;
using DomainModel.Generators;
using Newtonsoft.Json;
using DomainModel.Credentials;
using Logic.Models;

namespace Logic
{
    public interface IUserRegistrationManager
    {
        Task<User?> RegisterUser(RegisterUserRequest registerUserRequest);
        Task<User?> GenerateRandomUserAndRegister();
    }

    public class UserRegistrationManager : IUserRegistrationManager
    {
        private readonly IUserRepository _userRepository;
        private readonly IFeedEventRepository _feedEventRepository;
        private readonly IUserGenerator _userGenerator;
        private readonly ICredentialsRepository _credentialsRepository;

        public UserRegistrationManager(
            IUserRepository userRepository, 
            IFeedEventRepository feedEventRepository,
            IUserGenerator userGenerator,
            ICredentialsRepository credentialsRepository)
        {
            _userRepository = userRepository;
            _feedEventRepository = feedEventRepository;
            _userGenerator = userGenerator;
            _credentialsRepository = credentialsRepository;
        }

        public async Task<User?> RegisterUser(RegisterUserRequest registerUserRequest)
        {
            var user = new User(Guid.NewGuid().ToString(), registerUserRequest.Username)
            {
                UserCreated = DateTime.UtcNow,
            };

            await _userRepository.Save(user);

            var userCredentials = new Credentials(user.UserId, registerUserRequest.Password);
            await _credentialsRepository.SaveCredentials(userCredentials);

            await RegisterNewUserEvent(user);

            return user;
        }

        public async Task<User?> GenerateRandomUserAndRegister()
        {
            var user = await _userGenerator.GenerateUser();
            if (user == null)
                return null;

            await _userRepository.Save(user);
            
            var credentials = GenerateUserCredentials();
            await _credentialsRepository.SaveCredentials(credentials);
            
            await RegisterNewUserEvent(user);

            return user;

            // TODO: Maybe create a separate generator for this?
            Credentials GenerateUserCredentials()
            {
                return new Credentials(user.UserId, "cowperson123");
            }
        }

        private async Task RegisterNewUserEvent(User newUser)
        {
            var userData = new
            {
                UserId = newUser.UserId,
                Username = newUser.Username,
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