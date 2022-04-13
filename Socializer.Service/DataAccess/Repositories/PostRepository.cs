using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DomainModel.Posts;
using Newtonsoft.Json;

namespace Infrastructure.Repositories
{
    public class PostRepository : IPostRepository
    {
        private IDatabaseConnection _db;

        public PostRepository(IDatabaseConnection socializerDbConnection)
        {
            _db = socializerDbConnection;
        }

        public async Task<string> GetPostData(string postId)
        {
            string sql = @"SELECT PostDataJson FROM Post WHERE PostId LIKE @PostId";

            using (var connection = _db.GetConnection())
            {
                var post = await connection.QueryFirstAsync<Post>(sql, new { PostId = postId + "%" });

                return post.PostDataJson;
            }
        }

        public async Task<List<Post>> GetPostsWithoutData(string userId)
        {
            string sql = @"SELECT 
                            CONVERT(NVARCHAR(255), PostId) AS PostId,
                            CONVERT(NVARCHAR(255), PostUserId_Fk) AS PostUserId,
                            PostCreated
                        FROM Post
                        WHERE PostUserId_Fk = @UserId";

            using (var connection = _db.GetConnection())
            {
                var posts = await connection.QueryAsync<Post>(sql, new { UserId = userId });

                return posts.ToList();
            }
        }

        public async Task<Post?> GetPost(string postId)
        {
            string sql = @"SELECT 
                            CONVERT(NVARCHAR(255), PostId) AS PostId,
                            CONVERT(NVARCHAR(255), PostUserId_Fk) AS PostUserId,
                            PostCreated,
                            PostDataJson
                        FROM Post
                        WHERE PostId LIKE @PostId";

            using (var connection = _db.GetConnection())
            {
                var post = await connection.QueryFirstOrDefaultAsync<Post>(sql, new { PostId = postId + "%" });

                return post;
            }
        }

        public async Task SavePost(Post post)
        {
            string sql = @"INSERT INTO Post(
                            PostId, 
	                        PostUserId_Fk,
                            PostCreated,
	                        PostDataJson)
                        VALUES(
                            @PostId,
                            @PostUserId,
                            @PostCreated,
                            @PostDataJson)";

            using (var connection = _db.GetConnection())
            {
                var affectedRows = await connection.ExecuteAsync(sql,
                    new
                    {
                        PostId = post.PostId,
                        PostUserId = post.PostUserId,
                        PostCreated = post.PostCreated,
                        PostDataJson = post.PostDataJson,
                    }
                );
            }
        }

        public Task<List<Post>> GetPosts(string userId, int size, string lastReadPostId)
        {
            throw new NotImplementedException();
        }
    }
}
