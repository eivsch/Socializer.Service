namespace DomainModel.Posts
{
    public interface IPostRepository
    {
        Task<List<Post>> GetAllPosts(int size);
        Task SavePost(Post post);
        Task<List<Post>> GetPostsForUser(string userId);
        Task<Post?> GetPost(string postId);
        Task<string> GetPostData(string postId);
    }
}
