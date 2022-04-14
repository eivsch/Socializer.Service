using DomainModel.Posts;
using DomainModel.Users;

namespace Infrastructure
{
    public interface IFileServerClient
    {
        Task<byte[]> DownloadImage(string uri);
        Task<byte[]> DownloadUserProfilePicture(User user);
        Task<UserProfilePicture> UploadUserProfileImage(User user, Stream imageFile);
        Task<PostPicture> UploadPostImage(Post post, Stream imageFile);
    }
}
