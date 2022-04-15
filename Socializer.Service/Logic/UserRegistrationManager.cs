using DomainModel.FeedEvents;
using DomainModel.Users;
using DomainModel.FeedEvents.Interfaces;
using DomainModel.Generators;
using Newtonsoft.Json;
using Infrastructure;

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
        private readonly IFaceGenerator _faceGenerator;
        private readonly INameGenerator _userNameGenerator;
        private readonly IFaceClassifier _faceClassifier;
        private readonly IFileServerClient _fileServerClient;

        public UserRegistrationManager(
            IUserRepository userRepository, 
            IFeedEventRepository feedEventRepository,
            IFaceGenerator faceGenerator,
            INameGenerator userNameGenerator,
            IFaceClassifier faceClassifier,
            IFileServerClient fileServerClient)
        {
            _userRepository = userRepository;
            _feedEventRepository = feedEventRepository;
            _faceGenerator = faceGenerator;
            _userNameGenerator = userNameGenerator;
            _faceClassifier = faceClassifier;
            _fileServerClient = fileServerClient;
        }

        public async Task<User> RegisterUser()
        {
            Stream inStream;
            Stream copyStream;
            UserPersonalName userPersonalName;
            string username;
            UserFace userFaceInfo;

            while (true)
            {
                copyStream = new MemoryStream();
                inStream = await _faceGenerator.GenerateFace();
                await inStream.CopyToAsync(copyStream);
                inStream.Position = 0;  // Reset position after copying
                copyStream.Position = 0;

                userFaceInfo = await _faceClassifier.ClassifyUserFace(inStream);    // This closes the inStream for some reason
                if (userFaceInfo != null && userFaceInfo.Gender.HasValue)
                {
                    userPersonalName = await _userNameGenerator.GenerateName(country: "US", gender: userFaceInfo.Gender.Value);
                    if (userPersonalName == null)
                        throw new Exception("Unable to generate username");

                    username = userPersonalName.Firstname.ToLower() + "." + userPersonalName.Lastname.ToLower();

                    break;
                }

                copyStream.Dispose();
            }

            var user = new User
            {
                UserId = Guid.NewGuid().ToString(),
                UserCreated = DateTime.UtcNow,
                Username = username,
                PersonalName = userPersonalName,
                UserDetails = new UserDetails
                {
                    Age = (int?)userFaceInfo.Age,
                    Gender = userFaceInfo.Gender.Value
                }
            };
            var userProfilePictureInfo = await _fileServerClient.UploadUserProfileImage(user, copyStream);
            user.ProfilePicture = userProfilePictureInfo;
            await copyStream.DisposeAsync();

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