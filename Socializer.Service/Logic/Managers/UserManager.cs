using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainModel.Credentials;
using DomainModel.Users;
using Logic.Models;

namespace Logic.Managers
{
    public interface IUserManager
    {
        Task FollowUser(AddFollowerRequest addFollowerRequest);
        Task UnFollowUser(UnFollowRequest unFollowRequest);
        Task<UserRelationInfo> GetUserRelationInfo(string currentUserName, string relUsername);
    }

    public class UserManager : IUserManager
    {
        ICredentialsRepository _credentialsRepository;
        IUserRepository _userRepository;

        public UserManager(ICredentialsRepository credentialsRepository, IUserRepository userRepository)
        {
            _credentialsRepository = credentialsRepository;
            _userRepository = userRepository;
        }

        public async Task FollowUser(AddFollowerRequest addFollowerRequest)
        {
            await _userRepository.AddUserToFollow(addFollowerRequest.CurrentUserName, addFollowerRequest.UserToFollowName);
        }

        public async Task<UserRelationInfo> GetUserRelationInfo(string currentUserName, string relUsername)
        {
            UserRelationInfo result = new UserRelationInfo
            {
                CurrentUserToken = currentUserName,
                RelatedUserName = relUsername,
            };

            var relationship = await _userRepository.GetUserRelationship(currentUserName, relUsername);
            result.RelatedUserFollowsCurrentUser = relationship.RelatedUserFollowsCurrentUser;
            result.CurrentUserFollowsRelatedUser = relationship.CurrentUserFollowsRelatedUser;

            return result;
        }

        public async Task UnFollowUser(UnFollowRequest unFollowRequest)
        {
            await _userRepository.DeleteUserRelationship(unFollowRequest.CurrentUserName, unFollowRequest.UserToUnfollowName);
        }
    }
}
