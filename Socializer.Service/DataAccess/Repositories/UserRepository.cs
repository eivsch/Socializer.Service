using Dapper;
using DomainModel.Users;
using Newtonsoft.Json;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private IDatabaseConnection _db;

        public UserRepository(IDatabaseConnection socializerDbConnection)
        {
            _db = socializerDbConnection;
        }

        public async Task<User> GetRandomUser()
        {
            string sql = @"SELECT TOP 1 
                            CONVERT(NVARCHAR(255), UserId) AS UserId,
                            Username,
                            UserCreated,
                            UserDataJson
                        FROM SocializerUser
                        ORDER BY NEWID()";

            using (var connection = _db.GetConnection())
            {
                var userDTO = await connection.QueryFirstAsync<UserDTO>(sql);
                var userDataDTO = JsonConvert.DeserializeObject<UserDataJsonDTO>(userDTO.UserDataJson);

                return new User
                {
                    UserId = userDTO.UserId,
                    Username = userDTO.Username,
                    UserCreated = userDTO.UserCreated,
                    PersonalName = userDataDTO.PersonalName,
                    UserDetails = userDataDTO.Details,
                    ProfilePicture = userDataDTO.ProfilePic
                };
            }
        }

        public async Task<User> GetUserByName(string username)
        {
            string sql = @"SELECT 
                            CONVERT(NVARCHAR(255), UserId) AS UserId,
                            Username,
                            UserCreated,
                            UserDataJson
                        FROM SocializerUser
                        WHERE Username = @Username";

            using (var connection = _db.GetConnection())
            {
                var userDTO = await connection.QueryFirstOrDefaultAsync<UserDTO>(sql, new { Username = username });
                var userDataDTO = JsonConvert.DeserializeObject<UserDataJsonDTO>(userDTO.UserDataJson);

                return new User
                {
                    UserId = userDTO.UserId,
                    Username = userDTO.Username,
                    UserCreated = userDTO.UserCreated,
                    PersonalName = userDataDTO.PersonalName,
                    UserDetails = userDataDTO.Details,
                    ProfilePicture = userDataDTO.ProfilePic
                };
            }
        }

        public async Task Save(User user)
        {
            string sql = @"INSERT INTO SocializerUser(
                            UserId, 
	                        Username, 
	                        UserCreated,
                            UserDataJson)
                        VALUES(
                            @UserId,
                            @Username,
                            @UserCreated,
                            @UserDataJson)";

            using (var connection = _db.GetConnection())
            {
                var userData = new UserDataJsonDTO
                {
                    PersonalName = user.PersonalName,
                    Details = user.UserDetails,
                    ProfilePic = user.ProfilePicture
                };
                string userDataJson = JsonConvert.SerializeObject(userData);

                var affectedRows = await connection.ExecuteAsync(sql,
                    new
                    {
                        UserId = user.UserId,
                        Username = user.Username,
                        UserCreated = user.UserCreated,
                        UserDataJson = userDataJson
                    }
                );
            }
        }

        private class UserDTO
        {
            public string UserId { get; set; }
            public string Username { get; set; }
            public DateTime UserCreated { get; set; }
            public string UserDataJson { get; set; }
        }

        private class UserDataJsonDTO
        {
            public UserPersonalName PersonalName { get; set; }
            public UserDetails Details { get; set; }
            public UserProfilePicture ProfilePic { get; set; }
        }
    }
}
