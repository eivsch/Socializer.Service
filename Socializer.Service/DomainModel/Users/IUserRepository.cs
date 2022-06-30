namespace DomainModel.Users
{
    public interface IUserRepository
    {
        Task Save(User user);
        Task<User> GetRandomUser();
        Task<User> GetUserByName(string username);
        Task AddUserToFollow(string currentUserName, string userToFollowName);
        Task DeleteUserRelationship(string currentUserName, string userToUnFollowName);
        Task<UserRelationship> GetUserRelationship(string currentUserName, string relUsername);
    }
}
