using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainModel.Posts;
using Infrastructure.Repositories;
using Newtonsoft.Json;

namespace Logic
{
    public class PostManager
    {
        public async Task<List<Post>?> GetPosts(string username)
        {
            var user = await new UserRepository().GetUserByName(username);
            if (user == null)
                return null;

            var posts = await new PostRepository().GetPostsWithoutData(user.UserId);

            return posts;
        }

        public async Task<Post?>GetPost(string postId)
        {
            var post = await new PostRepository().GetPost(postId);

            return post;
        }

        public async Task<PostPicture> GetPictureForPost(string postId)
        {
            var postDataJson = await new PostRepository().GetPostData(postId);
            PostPicture pic = JsonConvert.DeserializeObject<PostPicture>(postDataJson);

            return pic;
        }
    }
}
