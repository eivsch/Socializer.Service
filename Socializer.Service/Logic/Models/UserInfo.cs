namespace Logic.Models
{
    public class UserRelationInfo
    {
        public string CurrentUserToken { get; set; }
        public string RelatedUserName { get; set; }
        public bool CurrentUserFollowsRelatedUser { get; set; }
        public bool RelatedUserFollowsCurrentUser { get; set; }
    }
}
