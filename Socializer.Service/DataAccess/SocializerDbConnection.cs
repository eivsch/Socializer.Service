using System.Data.SqlClient;

namespace Infrastructure
{
    public class SocializerDbConnection : IDatabaseConnection
    {
        private string _connectionString;

        public SocializerDbConnection(string connectionString)
        {
            _connectionString = connectionString;
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
