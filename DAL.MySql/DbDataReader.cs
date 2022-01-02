using MySql.Data.MySqlClient;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Sanakan.DAL.MySql
{
    public class DbDataReader : IDbDataReader, IAsyncDisposable
    {
        private readonly MySqlDataReader _dbDataReader;

        public DbDataReader(MySqlDataReader dbDataReader)
        {
            _dbDataReader = dbDataReader;
        }

        public TextReader GetTextReader(int ordinal) => _dbDataReader.GetTextReader(ordinal);

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length) =>
            _dbDataReader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);

        public int GetValues(object[] values) => _dbDataReader.GetValues(values);

        public bool IsDBNull(int ordinal) => _dbDataReader.IsDBNull(ordinal);

        public Task<bool> IsDBNullAsync(int ordinal) => _dbDataReader.IsDBNullAsync(ordinal);

        public double GetDouble(int ordinal) => _dbDataReader.GetDouble(ordinal);

        public DateTime GetDateTime(int ordinal) => _dbDataReader.GetDateTime(ordinal);

        public string GetString(int ordinal) => _dbDataReader.GetString(ordinal);

        public bool GetBoolean(int ordinal) => _dbDataReader.GetBoolean(ordinal);

        public uint GetUInt32(int ordinal) => _dbDataReader.GetUInt32(ordinal);

        public int GetInt32(int ordinal) => _dbDataReader.GetInt32(ordinal);

        public long GetInt64(int ordinal) => _dbDataReader.GetInt64(ordinal);

        public ulong GetUInt64(int ordinal) => _dbDataReader.GetUInt64(ordinal);

        public void Dispose() => _dbDataReader.Dispose();

        public ValueTask DisposeAsync() => _dbDataReader.DisposeAsync();

        public Task<bool> ReadAsync() => _dbDataReader.ReadAsync();
    }
}
