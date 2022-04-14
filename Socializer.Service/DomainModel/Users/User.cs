namespace DomainModel.Users
{
    public class User
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public DateTime UserCreated { get; set; }
        public UserProfilePicture ProfilePicture { get; set; }
    }
}