using System.Web.Helpers;
using Dapper;
using DomainModel.Credentials;

namespace Infrastructure.Repositories
{
    public class CredentialsRepository : ICredentialsRepository
    {
        class CredentialsDTO
        {
            public int CredentialsId { get; set; }
            public string UserId_Fk { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public string Token { get; set; }
            public DateTime Created { get; set; }
        }

        private IDatabaseConnection _db;

        public CredentialsRepository(IDatabaseConnection socializerDbConnection)
        {
            _db = socializerDbConnection;
        }

        public async Task<Credentials?> ValidateCredentials(Credentials credentials)
        {
            string sql = @"SELECT
                            cre.Password,
	                        cre.Token
                        FROM UserCredentials cre
                        JOIN SocializerUser usr ON usr.UserId = cre.UserId_Fk
                        WHERE usr.Username = @Username";

            using (var connection = _db.GetConnection())
            {
                var credentialsDto = await connection.QueryFirstOrDefaultAsync<CredentialsDTO>(sql, new { Username = credentials.Username });

                if (credentialsDto != null)
                {
                    if (Crypto.VerifyHashedPassword(credentialsDto.Password, credentials.Password))
                    {
                        return new Credentials(credentials.Username, credentials.Password)
                        {
                            Token = credentialsDto.Token,
                        };
                    }
                }
                    
                return null;
            }
        }

        public async Task<Credentials> SaveCredentials(Credentials credentials)
        {
            var passwordHash = Crypto.HashPassword(credentials.Password);
            var token = GenerateToken();
            var timestamp = DateTime.UtcNow;

            if (credentials.CredentialsId < 1)
            {
                string sql = @"INSERT INTO UserCredentials
	                (UserId_Fk
                    ,Password
	                ,Token
	                ,Created)
                VALUES
	                (@UserId
	                ,@Password
	                ,@Token
	                ,@Created)";

                using (var connection = _db.GetConnection())
                {
                    var affectedRows = await connection.ExecuteAsync(sql,
                        new
                        {
                            UserId = credentials.CredentialsUserId,
                            Password = passwordHash,
                            Token = token,
                            Created = timestamp
                        }
                    );
                }
            }
            else
            {
                string sql = @"UPDATE UserCredentials
                        SET Password = @Password
                            ,Token = @Token
                        WHERE CredentialsId = @CredentialsId";

                using (var connection = _db.GetConnection())
                {
                    var affectedRows = await connection.ExecuteAsync(sql,
                        new
                        {
                            CredentialsId = credentials.CredentialsId,
                            Password = passwordHash,
                            Token = token,
                        }
                    );
                }
            }

            return new Credentials("", passwordHash)
            {
                CredentialsId = credentials.CredentialsId,
                CredentialsUserId = credentials.CredentialsUserId,
                Created = timestamp,
                Token = token
            };
        }

        public async Task<string> GetUserIdByToken(string token)
        {
            string sql = @"SELECT
                            CONVERT(NVARCHAR(255), UserId_Fk) AS UserId_Fk
                        FROM UserCredentials
                        WHERE Token = @Token";

            using (var connection = _db.GetConnection())
            {
                string userId = await connection.QueryFirstOrDefaultAsync<string>(sql, new { Token = token });

                return userId;
            }
        }

        private string GenerateToken()
        {
            return Guid.NewGuid().ToString().Substring(1, 11).Replace("-", "");
        }
    }
}
