using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainModel.Posts;
using DomainModel.Users;
using Newtonsoft.Json;

namespace Logic
{
    public interface IPostManager
    {
        Task<List<Post>?> GetAll(int size);
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

        public async Task<PostPicture> GetPictureForPost(string postId)
        {
            var postDataJson = await _postRepository.GetPostData(postId);
            PostPicture pic = JsonConvert.DeserializeObject<PostPicture>(postDataJson);

            return pic;
        }

        public async Task<List<Post>?> GetPosts(string username)
        {
            var user = await _userRepository.GetUserByName(username);
            if (user == null)
                return null;

            var posts = await _postRepository.GetPostsForUser(user.UserId);
            foreach (var post in posts)
            {
                PostPicture pic = JsonConvert.DeserializeObject<PostPicture>(post.PostDataJson);
                post.PostPicture = pic;
                post.PostDataJson = null; // don't return the raw data
            }

            return posts;
        }

        public async Task<List<Post>?> GetAll(int size)
        {
            if (size > 24 )
                size = 24;

            var posts = await _postRepository.GetAllPosts(size);
            foreach (var post in posts)
            {
                PostPicture pic = JsonConvert.DeserializeObject<PostPicture>(post.PostDataJson);
                post.PostPicture = pic;
                post.PostDataJson = null; // don't return the raw data
            }

            return posts;
            //return posts.OrderByDescending(p => p.PostCreated).ToList();
        }
    }
}
