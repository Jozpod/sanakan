using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public abstract class TableEnumerator<T> : IAsyncEnumerable<T>, IAsyncEnumerator<T>, IDisposable
    {
        private readonly IDbConnection _connection;
        protected IDbDataReader _reader;
        private IDbCommand _command;
        private bool _disposed = false;

        public TableEnumerator(IDbConnection connection)
        {
            _connection = connection;
        }

        public abstract string TableName { get; }

        public abstract T Current { get; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
        }

        public Task OpenAsync() => _connection.OpenAsync();

        public async ValueTask DisposeAsync()
        {
            if (_connection.State == System.Data.ConnectionState.Open)
            {
                await _connection.CloseAsync();
            }

            if(_disposed)
            {
                GC.SuppressFinalize(this);
                return;
            }

            await _connection.DisposeAsync();

            Dispose(false);
            GC.SuppressFinalize(this);
        }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            _command = _connection.CreateCommand();
            _command.CommandText = string.Format(Queries.SelectAllFrom, TableName);

            return this;
        }

        public async ValueTask<bool> MoveNextAsync()
        {
            try
            {
                if (_reader == null)
                {
                    _reader = await _command.ExecuteReaderAsync();
                }

                return await _reader.ReadAsync();
            }
            catch (Exception ex)
            {

                throw;
            }           
        }
    }
}