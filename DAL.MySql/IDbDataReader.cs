using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace Sanakan.DAL.MySql
{
    public interface IDbDataReader
    {
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
