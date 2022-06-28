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
            string userId = await _credentialsRepository.GetUserIdByToken(addFollowerRequest.CurrentUserToken);
            if (!string.IsNullOrWhiteSpace(userId))
                await _userRepository.AddUserToFollow(userId, addFollowerRequest.UserToFollowId);
        }
    }
}
