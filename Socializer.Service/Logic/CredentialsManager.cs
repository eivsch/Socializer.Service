using DomainModel.Credentials;
using Logic.Models;

namespace Logic
{
    public interface ICredentialsManager
    {
        Task<ValidateCredentialsResponse> ValidateCredentials(ValidateCredentialsRequest validateCredentialsRequest);
    }

    public class CredentialsManager : ICredentialsManager
    {
        ICredentialsRepository _credentialsRepository;

        public CredentialsManager(ICredentialsRepository credentialsRepository)
        {
            _credentialsRepository = credentialsRepository;
        }

        public async Task<ValidateCredentialsResponse> ValidateCredentials(ValidateCredentialsRequest validateCredentialsRequest)
        {
            var credentials = new Credentials(validateCredentialsRequest.Username, validateCredentialsRequest.Password);
            credentials = await _credentialsRepository.ValidateCredentials(credentials);

            if (credentials != null && !string.IsNullOrWhiteSpace(credentials.Token))
            {
                return new ValidateCredentialsResponse
                {
                    IsValid = true,
                    Token = credentials.Token,
                };
            }

            return new ValidateCredentialsResponse
            {
                IsValid = false
            };
        }
    }
}
