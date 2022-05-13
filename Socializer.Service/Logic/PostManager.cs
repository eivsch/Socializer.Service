using DomainModel.Posts;
using DomainModel.Users;
using Newtonsoft.Json;

namespace Logic
{
    public interface IPostManager
    {
        Task<List<Post>?> GetAll(int size, string userToken);
        Task<List<Post>?> GetPosts(string username);
        Task<Post?> GetPost(string postId);
        Task<PostPicture> GetPictureForPost(string postId);
    }

    public class PostManager : IPostManager
    {
        private IPostRepository _postRepository;
        private IUserRepository _userRepository;

        public PostManager(IPostRepository postRepository, IUserRepository userRepository)
        {
            _postRepository = postRepository;
            _userRepository = userRepository;
        }

        public async Task<Post?>GetPost(string postId)
        {
            var post = await _postRepository.GetPost(postId);

            return post;
        }

        public async Task<List<Post>?> GetPosts(string username)
        {
            var user = await _userRepository.GetUserByName(username);
            if (user == null)
                return null;

            var posts = await _postRepository.GetPostsForUser(user.UserId);

            return posts;
        }

        public async Task<List<Post>?> GetAll(int size, string userToken)
        {
            if (size > 24 )
                size = 24;

            // Find user corresponding to the token and get posts for that user

            var posts = await _postRepository.GetAllPosts(size);

            return posts;
        }

        public async Task<PostPicture> GetPictureForPost(string postId)
        {
            var postDataJson = await _postRepository.GetPostData(postId);
            PostPicture pic = JsonConvert.DeserializeObject<PostPicture>(postDataJson);

            return pic;
        }
    }
}
