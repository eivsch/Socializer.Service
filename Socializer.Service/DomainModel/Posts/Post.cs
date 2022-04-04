namespace DomainModel.Posts
{
    public class Post
    {
        public string PostId { get; set; }
        public string PostUserId { get; set; }
        public DateTime PostCreated { get; set; }
        public string PostDataJson { get; set; }
    }
}
