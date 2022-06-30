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

                return new User(userDTO.UserId, userDTO.Username)
                {
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

                return new User(userDTO.UserId, userDTO.Username)
                {
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
                string? userDataJson = null;
                if (user.HasData)
                {
                    var userData = new
                    {
                        PersonalName = user.PersonalName,
                        Details = user.UserDetails,
                        ProfilePic = user.ProfilePicture
                    };
                    userDataJson = JsonConvert.SerializeObject(userData);
                }

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

        public async Task AddUserToFollow(string currentUserName, string userToFollowName)
        {
            if (string.IsNullOrWhiteSpace(currentUserName))
                throw new ArgumentNullException(nameof(currentUserName));
            if (string.IsNullOrWhiteSpace(userToFollowName))
                throw new ArgumentNullException(nameof(userToFollowName));

            string sql = @"EXEC p_insert_user_relationship @CurrentUserName, @UserToFollowName";

            using (var connection = _db.GetConnection())
            {
                var affectedRows = await connection.ExecuteAsync(sql,
                    new
                    {
                        CurrentUserName = currentUserName,
                        UserToFollowName = userToFollowName
                    }
                );
            }
        }

        public async Task<UserRelationship> GetUserRelationship(string currentUserName, string relUsername)
        {
            string sql = @"SELECT
	                        CurrentUserName,
	                        CONVERT(NVARCHAR(255), CurrentUserId) AS CurrentUserId,
	                        RelatedUserName,
	                        CONVERT(NVARCHAR(255), RelatedUserId) AS RelatedUserId,
	                        CurrentUserFollowsRelatedUser,
	                        RelatedUserFollowsCurrentUser
                        FROM f_tbl_get_user_relation(@CurrentUserName, @RelUsername)";

            using (var connection = _db.GetConnection())
            {
                var relDTO = await connection.QueryFirstOrDefaultAsync<UserRelationship>(sql, new { CurrentUserName = currentUserName, RelUsername = relUsername });

                return relDTO;
            }
        }

        public async Task DeleteUserRelationship(string currentUserName, string userToUnFollowName)
        {
            if (string.IsNullOrWhiteSpace(currentUserName))
                throw new ArgumentNullException(nameof(currentUserName));
            if (string.IsNullOrWhiteSpace(userToUnFollowName))
                throw new ArgumentNullException(nameof(userToUnFollowName));

            string sql = @"EXEC p_delete_user_relationship @CurrentUserName, @UserToUnFollowName";

            using (var connection = _db.GetConnection())
            {
                var affectedRows = await connection.ExecuteAsync(sql,
                    new
                    {
                        CurrentUserName = currentUserName,
                        UserToUnFollowName = userToUnFollowName
                    }
                );
            }
        }
    }
}
