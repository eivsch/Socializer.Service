using DomainModel.Users;

namespace DomainModel.Generators
{
    public interface IUserGenerator
    {
        Task<User?> GenerateUser();
    }
}
