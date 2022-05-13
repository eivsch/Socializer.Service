namespace Logic.Models
{
    public class AddFollowerRequest
    {
        public string? CurrentUserToken { get; set; }
        public string UserToFollowId { get; set; }
    }
}
