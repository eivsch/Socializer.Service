using DomainModel.FeedEvents;
using DomainModel.FeedEvents.Interfaces;
using DomainModel.Generators;
using DomainModel.Posts;
using DomainModel.Users;
using Newtonsoft.Json;

namespace Logic.Managers
{
    public interface IRandomUserPostManager
    {
        Task<Post?> PostRandomTextFromRandomUser();
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

        public async Task<Post?> PostRandomTextFromRandomUser()
        {
            var user = await _userRepository.GetRandomUser();
            var picture = await _postPictureGenerator.GeneratePostPicture();
            var text = await _randomTextGenerator.GenerateRandomText();

            if (user != null && picture != null && !string.IsNullOrWhiteSpace(text))
            {
                var postData = new
                {
                    user.Username,
                    Text = text,
                    picture.PictureId,
                    picture.PictureUri
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

                return post;
            }

            return null;
        }

        private async Task RegisterNewPostEvent(Post post)
        {
            var eventData = new
            {
                post.PostId,
                post.PostUserId,
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
