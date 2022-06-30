namespace Logic.Models
{
    public class AddFollowerRequest
    {
        public string CurrentUserName { get; set; }
        public string UserToFollowName { get; set; }
    }
}
