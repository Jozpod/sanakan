using MySqlConnector;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public abstract class TableEnumerator<T> : IAsyncEnumerable<T>, IAsyncEnumerator<T>
    {
        private readonly MySqlConnection _connection;
        protected MySqlDataReader _reader;
        private MySqlCommand _command;

        public TableEnumerator(MySqlConnection connection)
        {
            _connection = connection;
        }

        public abstract string TableName { get; }

        public abstract T Current { get; }

        public ValueTask DisposeAsync() => _connection.DisposeAsync();

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            _command = _connection.CreateCommand();
            _command.CommandText = string.Format(Queries.SelectAllFrom, TableName);

            return this;
        }

        public async ValueTask<bool> MoveNextAsync()
        {
            if(_reader == null)
            {
                _reader = await _command.ExecuteReaderAsync();
            }

            return await _reader.ReadAsync();
        }
    }
}
