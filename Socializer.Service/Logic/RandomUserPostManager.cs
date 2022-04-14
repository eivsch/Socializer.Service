using DomainModel.FeedEvents;
using DomainModel.FeedEvents.Interfaces;
using DomainModel.Generators;
using DomainModel.Posts;
using DomainModel.Users;
using Newtonsoft.Json;

namespace Logic
{
    public interface IRandomUserPostManager
    {
        Task PostRandomTextFromRandomUser();
    }

    public class RandomUserPostManager : IRandomUserPostManager
    {
        private IPostRepository _postRepository;
        private IUserRepository _userRepository;
        private IFeedEventRepository _feedEventRepository;
        private IPostPictureGenerator _postPictureGenerator;
        private IRandomTextGenerator _randomTextGenerator;

        public RandomUserPostManager(
            IPostRepository postRepository, 
            IUserRepository userRepository, 
            IFeedEventRepository feedEventRepository,
            IPostPictureGenerator postPictureGenerator,
            IRandomTextGenerator randomTextGenerator)
        {
            _postRepository = postRepository;
            _userRepository = userRepository;
            _feedEventRepository = feedEventRepository;
            _postPictureGenerator = postPictureGenerator;
            _randomTextGenerator = randomTextGenerator;
        }

        public async Task PostRandomTextFromRandomUser()
        {
            var user = await _userRepository.GetRandomUser();
            var picture = await _postPictureGenerator.GeneratePostPicture();
            var text = await _randomTextGenerator.GenerateRandomText();

            var postData = new
            {
                Username = user.Username,
                Text = text,
                PictureId = picture.PictureId,
                PictureAppPath = picture.PictureAppPath
            };
            var post = new Post
            {
                PostId = Guid.NewGuid().ToString(),
                PostUserId = user.UserId,
                PostCreated = DateTime.UtcNow,
                PostDataJson = JsonConvert.SerializeObject(postData),
            };

            await _postRepository.SavePost(post);
            await RegisterNewPostEvent(post);
        }

        private async Task RegisterNewPostEvent(Post post)
        {
            var eventData = new
            {
                PostId = post.PostId,
                PostUserId = post.PostUserId,
            };

            var feedEvent = new FeedEvent
            {
                Id = Guid.NewGuid().ToString(),
                EventCreated = DateTime.UtcNow,
                EventType = "NewPost",
                EventDataJson = JsonConvert.SerializeObject(eventData)
            };

            await _feedEventRepository.AddEventToQueue(feedEvent);
        }
    }
}
