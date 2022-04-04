using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DomainModel.Users;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        public async Task<User> GetRandomUser()
        {
            string sql = @"SELECT TOP 1 
                            CONVERT(NVARCHAR(255), UserId) AS UserId,
                            Username,
                            UserCreated
                        FROM SocializerUser
                        ORDER BY NEWID()";

            using (var connection = new SqlConnection("Server=.;Database=WebGallery;Trusted_Connection=True;"))
            {
                var user = await connection.QueryFirstAsync<User>(sql);

                return user;
            }
        }

        public async Task<User> GetUserByName(string username)
        {
            string sql = @"SELECT 
                            CONVERT(NVARCHAR(255), UserId) AS UserId,
                            Username,
                            UserCreated
                        FROM SocializerUser
                        WHERE Username = @Username";

            using (var connection = new SqlConnection("Server=.;Database=WebGallery;Trusted_Connection=True;"))
            {
                var user = await connection.QueryFirstOrDefaultAsync<User>(sql, new { Username = username });

                return user;
            }
        }

        public async Task Save(User user)
        {
            string sql = @"INSERT INTO SocializerUser(
                            UserId, 
	                        Username, 
	                        UserCreated)
                        VALUES(
                            @UserId,
                            @Username,
                            @UserCreated)";

            using (var connection = new SqlConnection("Server=.;Database=WebGallery;Trusted_Connection=True;"))
            {
                var affectedRows = await connection.ExecuteAsync(sql,
                    new
                    {
                        UserId = user.UserId,
                        Username = user.Username,
                        UserCreated = user.UserCreated,
                    }
                );
            }
        }
    }
}
