namespace DomainModel.Users
{
    public class UserRelationship
    {
        public string CurrentUserId { get; set; }
        public string CurrentUserName { get; set; }
        public string RelatedUserId { get; set; }
        public string RelatedUserName { get; set; }
        public bool CurrentUserFollowsRelatedUser { get; set; }
        public bool RelatedUserFollowsCurrentUser { get; set; }
    }
}
