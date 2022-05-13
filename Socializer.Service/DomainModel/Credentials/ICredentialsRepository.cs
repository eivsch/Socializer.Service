namespace DomainModel.Credentials
{
    public interface ICredentialsRepository
    {
        Task<Credentials> SaveCredentials(Credentials credentials);
        Task<Credentials> ValidateCredentials(Credentials credentials);
        Task<string> GetUserIdByToken(string token);
    }
}
