using Dapper;
using DomainModel.Users;
using Newtonsoft.Json;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private class UserDTO
        {
            public string UserId { get; set; }
            public string Username { get; set; }
            public DateTime UserCreated { get; set; }
            public string Firstname { get; set; }
            public string Lastname { get; set; }
            public int Age { get; set; }
            public int Gender { get; set; }
            public string Emotion { get; set; }
            public string ProfilePicUri { get; set; }
        }

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
	                        UserDataJson,
                            JSON_VALUE(UserDataJson, '$.PersonalName.Firstname') AS Firstname,
                            JSON_VALUE(UserDataJson, '$.PersonalName.Lastname') AS Lastname,
                            JSON_VALUE(UserDataJson, '$.Details.Age') AS Age,
                            JSON_VALUE(UserDataJson, '$.Details.Gender') AS Gender,
                            JSON_VALUE(UserDataJson, '$.Details.Emotion') AS Emotion,
                            JSON_VALUE(UserDataJson, '$.ProfilePic.PictureUri') AS ProfilePicUri
                        FROM SocializerUser
                        ORDER BY NEWID()";

            using (var connection = _db.GetConnection())
            {
                var userDTO = await connection.QueryFirstAsync<UserDTO>(sql);

                return new User
                {
                    UserId = userDTO.UserId,
                    Username = userDTO.Username,
                    UserCreated = userDTO.UserCreated,
                    PersonalName = new UserPersonalName
                    {
                        Firstname = userDTO.Firstname,
                        Lastname = userDTO.Lastname
                    },
                    UserDetails = new UserDetails
                    {
                        Age = userDTO.Age,
                        Emotion = userDTO.Emotion,
                        Gender = (DomainModel.GenderType) userDTO.Gender,
                    },
                    ProfilePicture = new UserProfilePicture
                    {
                        PictureUri = userDTO.ProfilePicUri
                    }
                };
            }
        }

        public async Task<User> GetUserByName(string username)
        {
            string sql = @"SELECT 
                            CONVERT(NVARCHAR(255), UserId) AS UserId,
                            Username,
                            UserCreated,
	                        UserDataJson,
                            JSON_VALUE(UserDataJson, '$.PersonalName.Firstname') AS Firstname,
                            JSON_VALUE(UserDataJson, '$.PersonalName.Lastname') AS Lastname,
                            JSON_VALUE(UserDataJson, '$.Details.Age') AS Age,
                            JSON_VALUE(UserDataJson, '$.Details.Gender') AS Gender,
                            JSON_VALUE(UserDataJson, '$.Details.Emotion') AS Emotion,
                            JSON_VALUE(UserDataJson, '$.ProfilePic.PictureUri') AS ProfilePicUri
                        FROM SocializerUser
                        WHERE Username = @Username";

            using (var connection = _db.GetConnection())
            {
                var userDTO = await connection.QueryFirstOrDefaultAsync<UserDTO>(sql, new { Username = username });

                return new User
                {
                    UserId = userDTO.UserId,
                    Username = userDTO.Username,
                    UserCreated = userDTO.UserCreated,
                    PersonalName = new UserPersonalName
                    {
                        Firstname = userDTO.Firstname,
                        Lastname = userDTO.Lastname
                    },
                    UserDetails = new UserDetails
                    {
                        Age = userDTO.Age,
                        Emotion = userDTO.Emotion,
                        Gender = (DomainModel.GenderType)userDTO.Gender,
                    },
                    ProfilePicture = new UserProfilePicture
                    {
                        PictureUri = userDTO.ProfilePicUri
                    }
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
                var userData = new
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
    }
}
