using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainModel.Credentials;
using DomainModel.Posts;

namespace Logic
{
    public interface IUserFeedService
    {
        Task<List<Post>> GetFeedForUser(int size, string userToken);
    }

    internal class UserFeedService : IUserFeedService
    {
        ICredentialsRepository _credentialsRepository = null;

        public UserFeedService(ICredentialsRepository credentialsRepository)
        {
            _credentialsRepository = credentialsRepository;
        }

        public async Task<List<Post>> GetFeedForUser(int size, string userToken)
        {
            if (size > 24)
                size = 24;
            if (size < 1)
                size = 1;

            string userId = await _credentialsRepository.GetUserIdByToken(userToken);

            return null;
        }
    }
}
