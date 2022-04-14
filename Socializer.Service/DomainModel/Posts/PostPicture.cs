namespace DomainModel.Posts
{
    public class PostPicture
    {
        public string PictureId { get; set; }
        public string PictureUri { get; set; }
        public IEnumerable<string> Tags { get; set; }
    }
}
