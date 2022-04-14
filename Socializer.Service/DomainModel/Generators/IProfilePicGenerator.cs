using DomainModel.Users;

namespace DomainModel.Generators
{
    public interface IProfilePicGenerator
    {
        Task<UserProfilePicture> GeneratePicture(User user);
    }
}
