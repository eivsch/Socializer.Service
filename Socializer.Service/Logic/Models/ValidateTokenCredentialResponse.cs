using DomainModel.Users;

namespace Logic.Models
{
    public class ValidateTokenCredentialResponse
    {
        public bool IsValid { get; set; }
        public User User { get; set; }
    }
}
