using DomainModel.Generators;
using DomainModel.Users;
using Infrastructure;
using Infrastructure.ThirdPartyServices;

namespace Logic
{
    public class UserGenerator : IUserGenerator
    {
        const int MaxIterations = 5;

        private readonly IFaceGenerator _faceGenerator;
        private readonly INameGenerator _userNameGenerator;
        private readonly IFaceClassifier _faceClassifier;
        private readonly IFileServerClient _fileServerClient;

        public UserGenerator(
            IFaceGenerator faceGenerator,
            INameGenerator userNameGenerator,
            IFaceClassifier faceClassifier,
            IFileServerClient fileServerClient)
        {
            _faceGenerator = faceGenerator;
            _userNameGenerator = userNameGenerator;
            _faceClassifier = faceClassifier;
            _fileServerClient = fileServerClient;
        }

        public async Task<User?> GenerateUser()
        {
            Stream inStream;
            Stream faceImageStream = null;
            UserPersonalName userPersonalName = null;
            string username = null;
            UserFaceFeatures userFaceInfo = null;

            int iterations = 0;
            while (iterations < MaxIterations)
            {
                faceImageStream = new MemoryStream();

                // Generate face image
                inStream = await _faceGenerator.GenerateFace();

                // Copy to a placeholder stream since it gets disposed by the Microsoft cognitive library
                await inStream.CopyToAsync(faceImageStream);
                inStream.Position = 0;  // Reset position after copying
                faceImageStream.Position = 0;

                // Classify the face - make sure we can determine the gender
                userFaceInfo = await _faceClassifier.ClassifyUserFace(inStream);    // This closes the inStream for some reason
                if (userFaceInfo != null && userFaceInfo.Gender.HasValue)
                {
                    // Generate username
                    userPersonalName = await _userNameGenerator.GenerateName(country: "US", gender: userFaceInfo.Gender.Value);
                    if (userPersonalName == null)
                        throw new Exception("Unable to generate username");

                    username = userPersonalName.Firstname.ToLower() + "." + userPersonalName.Lastname.ToLower();

                    break;
                }

                faceImageStream.Dispose();
                iterations++;
            }

            if (iterations >= MaxIterations)
                return null;

            var user = new User(Guid.NewGuid().ToString(), username)
            {
                UserCreated = DateTime.UtcNow,
                PersonalName = userPersonalName,
                UserDetails = new UserDetails
                {
                    Age = (int?)userFaceInfo.Age,
                    Gender = userFaceInfo.Gender.Value,
                    Emotion = userFaceInfo.EmotionType
                }
            };
            var userProfilePictureInfo = await _fileServerClient.UploadUserProfileImage(user, faceImageStream);
            user.ProfilePicture = userProfilePictureInfo;
            await faceImageStream.DisposeAsync();

            return user;
        }
    }
}
