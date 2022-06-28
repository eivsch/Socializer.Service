using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainModel.Credentials;
using DomainModel.Posts;

namespace Logic
{
    public interface IFeedService
    {
        Task<List<Post>> GetFeed(int size, string userToken);
    }

    public class FeedService : IFeedService
    {
        ICredentialsRepository _credentialsRepository;
        IPostRepository _postRepository;

        public FeedService(ICredentialsRepository credentialsRepository, IPostRepository postRepository)
        {
            _credentialsRepository = credentialsRepository;
            _postRepository = postRepository;
        }

        public async Task<List<Post>> GetFeed(int size, string userToken)
        {
            if (size > 24)
                size = 24;
            if (size < 1)
                size = 1;

            List<Post> result = new List<Post>();
            if (string.IsNullOrWhiteSpace(userToken))
            {
                result = await _postRepository.GetAllPosts(size);
            }
            else
            {
                string userId = await _credentialsRepository.GetUserIdByToken(userToken);
                result = await _postRepository.GetFeed(size, userId);
            }

            return result;
        }
    }
}
