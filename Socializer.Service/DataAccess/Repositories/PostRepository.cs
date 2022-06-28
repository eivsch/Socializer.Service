using Dapper;
using DomainModel.Posts;

namespace Infrastructure.Repositories
{
    public class PostRepository : IPostRepository
    {
        private class PostDTO
        {
            public string PostId { get; set; }
            public string PostUserId { get; set; }
            public DateTime PostCreated { get; set; }
            public string PostUserName { get; set; }
            public string PostHeader { get; set; }
            public string PictureId { get; set; }
            public string PictureUri { get; set; }

        }

        private IDatabaseConnection _db;

        public PostRepository(IDatabaseConnection socializerDbConnection)
        {
            _db = socializerDbConnection;
        }

        public async Task<Post?> GetPost(string postId)
        {
            string sql = @"SELECT 
                            CONVERT(NVARCHAR(255), p.PostId) AS PostId,
                            CONVERT(NVARCHAR(255), p.PostUserId_Fk) AS PostUserId,
                            p.PostCreated,
	                        u.Username AS PostUsername,
                            JSON_VALUE(p.PostDataJson, '$.Text') AS PostHeader,
	                        JSON_VALUE(p.PostDataJson, '$.PictureId') AS PictureId,
	                        JSON_VALUE(p.PostDataJson, '$.PictureUri') AS PictureUri
                        FROM Post p
                        JOIN SocializerUser u ON u.UserId = p.PostUserId_Fk
                        WHERE p.PostId LIKE @PostId";

            using (var connection = _db.GetConnection())
            {
                var postDto = await connection.QueryFirstOrDefaultAsync<PostDTO>(sql, new { PostId = postId + "%" });

                return MapFromDto(postDto);
            }
        }

        public async Task<List<Post>> GetPostsForUser(string userId)
        {
            string sql = @"SELECT 
                            CONVERT(NVARCHAR(255), p.PostId) AS PostId,
                            CONVERT(NVARCHAR(255), p.PostUserId_Fk) AS PostUserId,
                            p.PostCreated,
	                        u.Username AS PostUsername,
                            JSON_VALUE(p.PostDataJson, '$.Text') AS PostHeader,
	                        JSON_VALUE(p.PostDataJson, '$.PictureId') AS PictureId,
	                        JSON_VALUE(p.PostDataJson, '$.PictureUri') AS PictureUri
                        FROM Post p
                        JOIN SocializerUser u ON u.UserId = p.PostUserId_Fk
                        WHERE p.PostUserId_Fk = @UserId
                        ORDER BY p.PostCreated DESC";

            using (var connection = _db.GetConnection())
            {
                var posts = await connection.QueryAsync<PostDTO>(sql, new { UserId = userId });

                return posts.Select(s => MapFromDto(s)).ToList();
            }
        }

        public async Task<List<Post>> GetAllPosts(int size)
        {
            if (size > 1000)
                size = 1000;

            string sql = @"SELECT TOP (@Size)
                            CONVERT(NVARCHAR(255), p.PostId) AS PostId,
                            CONVERT(NVARCHAR(255), p.PostUserId_Fk) AS PostUserId,
                            p.PostCreated,
	                        u.Username AS PostUsername,
                            JSON_VALUE(PostDataJson, '$.Text') AS PostHeader,
	                        JSON_VALUE(p.PostDataJson, '$.PictureId') AS PictureId,
	                        JSON_VALUE(p.PostDataJson, '$.PictureUri') AS PictureUri
                        FROM Post p
                        JOIN SocializerUser u ON u.UserId = p.PostUserId_Fk
                        ORDER BY p.PostCreated DESC";

            using (var connection = _db.GetConnection())
            {
                var posts = await connection.QueryAsync<PostDTO>(sql, new { Size = size });

                return posts.Select(s => MapFromDto(s)).ToList();
            }
        }

        public async Task<List<Post>> GetFeed(int size, string feedUserId)
        {
            if (size > 1000)
                size = 1000;

            string sql = @"SELECT TOP (@Size)
	                        FeedUserId,
	                        CONVERT(NVARCHAR(255), PostId) AS PostId,
	                        CONVERT(NVARCHAR(255), PostUserId_Fk) AS PostUserId,
	                        PostCreated,
	                        Username AS PostUsername,
	                        PostHeader,
	                        PictureId,
	                        PictureUri
                        FROM v_user_feed
                        WHERE FeedUserId = @FeedUserId
                        ORDER BY PostCreated DESC";

            using (var connection = _db.GetConnection())
            {
                var posts = await connection.QueryAsync<PostDTO>(sql, new { Size = size, FeedUserId = feedUserId });

                return posts.Select(s => MapFromDto(s)).ToList();
            }
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

        private Post MapFromDto(PostDTO dto)
        {
            return new Post
            {
                PostId = dto.PostId,
                PostUserId = dto.PostUserId,
                PostCreated = dto.PostCreated,
                PostUsername = dto.PostUserName,
                PostHeader = dto.PostHeader,
                PostPicture = new PostPicture
                {
                    PictureId = dto.PictureId,
                    PictureUri = dto.PictureUri,
                },
            };
        }
    }
}
