using System.Data.SqlClient;

namespace Infrastructure
{
    public interface IDatabaseConnection
    {
        SqlConnection GetConnection();
    }
}
