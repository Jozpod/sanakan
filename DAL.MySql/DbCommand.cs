using MySql.Data.MySqlClient;
using System.Threading.Tasks;

namespace Sanakan.DAL.MySql
{
    public class DbCommand : IDbCommand
    {
        private readonly MySqlCommand _dbCommand;

        public DbCommand(MySqlCommand dbCommand)
        {
            _dbCommand = dbCommand;
           
        }

        public string CommandText
        {
            get
            {
                return _dbCommand.CommandText;
            }
            set
            {
                _dbCommand.CommandText = value;
            }
        }

        public void Dispose() => _dbCommand.Dispose();

        public ValueTask DisposeAsync() => _dbCommand.DisposeAsync();

        public Task<int> ExecuteNonQueryAsync() => _dbCommand.ExecuteNonQueryAsync();

        public async Task<IDbDataReader> ExecuteReaderAsync() => new DbDataReader((MySqlDataReader)await _dbCommand.ExecuteReaderAsync());
    }
}
