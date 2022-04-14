using DomainModel.Posts;

namespace DomainModel.Generators
{
    public interface IPostPictureGenerator
    {
        Task<PostPicture> GeneratePostPicture();
    }
}
