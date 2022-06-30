﻿namespace DomainModel.Users
{
    public interface IUserRepository
    {
        Task Save(User user);
        Task<User> GetRandomUser();
        Task<User> GetUserByName(string username);
        Task AddUserToFollow(string currentUserToken, string userToFollowName);
        Task<UserRelationship> GetUserRelationship(string currentUserName, string relUsername);
    }
}
