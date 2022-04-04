namespace DomainModel.Posts
{
    public interface IPostRepository
    {
        Task SavePost(Post post);
        Task<List<Post>> GetPostsWithoutData(string userId);
        Task<List<Post>> GetPosts(string userId, int size, string lastReadPostId);
        Task<Post?> GetPost(string postId);
        Task<string> GetPostData(string postId);
    }
}
