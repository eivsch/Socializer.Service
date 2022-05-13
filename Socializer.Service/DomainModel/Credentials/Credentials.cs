namespace DomainModel.Credentials
{
    public class Credentials
    {
        public Credentials(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public Credentials(string token)
        {
            Token = token;
        }

        public int CredentialsId { get; set; }
        public string? CredentialsUserId { get; set; }
        public string Username { get; }
        public string Password { get; }
        public string? Token { get; set; }
        public DateTime Created { get; set; }
    }
}
