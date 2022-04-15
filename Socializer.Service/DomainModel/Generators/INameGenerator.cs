using DomainModel.Users;

namespace DomainModel.Generators
{
    public interface INameGenerator
    {
        Task<UserPersonalName?> GenerateName(string country, GenderType gender);
    }
}
