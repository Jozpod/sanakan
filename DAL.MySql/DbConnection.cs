using MySql.Data.MySqlClient;
using System.Threading.Tasks;

namespace Sanakan.DAL.MySql
{
    public class DbConnection : IDbConnection
    {
        private readonly MySqlConnection _dbConnection;

        public DbConnection(string connectionString)
        {
            _dbConnection = new MySqlConnection(connectionString);
        }

        public DbConnection(MySqlConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public System.Data.ConnectionState State => _dbConnection.State;

        public void Dispose() => _dbConnection.Dispose();

        public ValueTask DisposeAsync() => _dbConnection.DisposeAsync();

        public IDbCommand CreateCommand() => new DbCommand(_dbConnection.CreateCommand());

        public Task OpenAsync() => _dbConnection.OpenAsync();

        public Task CloseAsync() => _dbConnection.CloseAsync();

        public MySqlConnection GetBaseConnection() => _dbConnection;
    }
}
