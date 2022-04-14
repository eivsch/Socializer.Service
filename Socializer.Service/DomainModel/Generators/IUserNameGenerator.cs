namespace DomainModel.Generators
{
    public interface IUserNameGenerator
    {
        Task<string> GenerateUserName();
    }
}
