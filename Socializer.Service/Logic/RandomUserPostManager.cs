using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DomainModel.FeedEvents;
using DomainModel.FeedEvents.Interfaces;
using DomainModel.Posts;
using DomainModel.Users;
using Infrastructure;
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

        public RandomUserPostManager(IPostRepository postRepository, IUserRepository userRepository, IFeedEventRepository feedEventRepository)
        {
            _postRepository = postRepository;
            _userRepository = userRepository;
            _feedEventRepository = feedEventRepository;
        }

        public async Task PostRandomTextFromRandomUser()
        {
            var user = await _userRepository.GetRandomUser();
            var picture = await new WebGalleryService().GetRandomPicture("testuser");
            var text = await GenerateRandomText();

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

        private async Task<string> GenerateRandomText()
        {
            HttpClient client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://randommer.io/api/Text/LoremIpsum?loremType=normal&type=words&number=4");
            request.Headers.Add("X-Api-Key", "76c63aed24cc4096a3aa15986526c137");
            var respone = client.Send(request);
            if (respone.StatusCode == HttpStatusCode.OK)
            {
                string result = await respone.Content.ReadAsStringAsync();
                if (result != null)
                {
                    string text = JsonConvert.DeserializeObject<string>(result);
                    return text;
                }
            }

            throw new Exception("Unable to generate text - None or bad response from randommer.io");
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
