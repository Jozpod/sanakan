using System;
using System.IO;
using System.Threading.Tasks;

namespace Sanakan.DAL.MySql
{
    public interface IDbDataReader : IDisposable
    {
        TextReader GetTextReader(int ordinal);

        long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length);

        int GetValues(object[] values);

        bool IsDBNull(int ordinal);

        Task<bool> IsDBNullAsync(int ordinal);

        double GetDouble(int ordinal);

        DateTime GetDateTime(int ordinal);

        string GetString(int ordinal);

        bool GetBoolean(int ordinal);

        int GetInt32(int ordinal);

        uint GetUInt32(int ordinal);

        long GetInt64(int ordinal);

        ulong GetUInt64(int ordinal);

        Task<bool> ReadAsync();
    }
}
