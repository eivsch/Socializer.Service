using DomainModel.FeedEvents;
using DomainModel.FeedEvents.Interfaces;
using DomainModel.Posts;
using DomainModel.Users;
using Infrastructure;
using Infrastructure.ThirdPartyServices;
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
        private IWebGalleryService _webGalleryService;
        private IRandommerClient _randommerClient;

        public RandomUserPostManager(
            IPostRepository postRepository, 
            IUserRepository userRepository, 
            IFeedEventRepository feedEventRepository, 
            IWebGalleryService webGalleryService,
            IRandommerClient randommerClient)
        {
            _postRepository = postRepository;
            _userRepository = userRepository;
            _feedEventRepository = feedEventRepository;
            _webGalleryService = webGalleryService;
            _randommerClient = randommerClient;
        }

        public async Task PostRandomTextFromRandomUser()
        {
            var user = await _userRepository.GetRandomUser();
            var picture = await _webGalleryService.GetRandomPicture();
            var text = await _randommerClient.GenerateRandomText();

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
