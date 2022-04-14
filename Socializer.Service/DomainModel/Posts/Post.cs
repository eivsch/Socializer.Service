namespace DomainModel.Posts
{
    public class Post
    {
        public string PostId { get; set; }
        public string PostUserId { get; set; }
        public string PostUsername { get; set; }
        public string PostHeader { get; set; }
        public DateTime PostCreated { get; set; }
        public string PostDataJson { get; set; }
        public PostPicture? PostPicture { get; set; }
    }
}
