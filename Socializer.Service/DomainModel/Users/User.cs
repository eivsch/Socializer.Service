namespace DomainModel.Users
{
    public class User
    {
        public User(string userId, string username)
        {
            UserId = userId;
            Username = username;
        }

        public string UserId { get; }
        public string Username { get; }
        public DateTime UserCreated { get; set; }
        public UserProfilePicture? ProfilePicture { get; set; }
        public UserPersonalName? PersonalName { get; set; }
        public UserDetails? UserDetails { get; set; }
        public bool HasData => PersonalName != null || UserDetails != null || ProfilePicture != null;
    }
}