using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.DAL.MySql.Schema
{
    public static class Utils
    {
        public static async Task StubSelectAsync(DbConnection connection)
        {
            var command = connection.CreateCommand();
            command.CommandText = "SELECT 1";
            await command.ExecuteNonQueryAsync();
        }

        public static async Task<List<string>> GetLastQueriesAsync(DbConnection connection)
        {
            var stringBuilder = new StringBuilder(1000);
            var queries = new List<string>();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT argument FROM mysql.general_log";

            using var reader = await command.ExecuteReaderAsync();
            var encoding = Encoding.UTF8;

            while (await reader.ReadAsync())
            {
                var length = (int)reader.GetBytes(0, 0, null, 0, 0);
                var buffer = new byte[length];
                var dataOffset = 0;
                var query = string.Empty;

                while (dataOffset < length)
                {
                    var bytesRead = (int)reader.GetBytes(0, dataOffset, buffer, dataOffset, length - dataOffset);
                    query = encoding.GetString(buffer, 0, bytesRead);
                    dataOffset += bytesRead;
                }

                if (query.Contains("SET GLOBAL"))
                {
                    continue;
                }

                if (query.Contains("SELECT 1"))
                {
                    query = stringBuilder.ToString();
                    stringBuilder.Clear();
                    queries.Add(query);
                }
                else
                {
                    stringBuilder.AppendLine(query);
                    stringBuilder.AppendLine();
                }
            }

            return queries;
        }

        public static async Task TruncateGeneralLogAsync(DbConnection connection)
        {
            var command = connection.CreateCommand();
            command.CommandText = "TRUNCATE TABLE mysql.general_log";
            await command.ExecuteNonQueryAsync();
        }

        public static async Task ToggleGeneralLogAsync(DbConnection connection, string mode)
        {
            var command = connection.CreateCommand();
            command.CommandText = $"SET GLOBAL general_log = '{mode}'";
            await command.ExecuteNonQueryAsync();
        }

        public static async Task<string> GetTableDefinitionAsync(DbConnection connection, string tableName)
        {
            var command = connection.CreateCommand();
            command.CommandText = string.Format(Queries.TableDefinition, tableName);

            using var reader = await command.ExecuteReaderAsync();
            await reader.ReadAsync();

            var text = reader.GetString(1);

            return text;
        }

        public static async Task<List<string>> GetTableIndexesAsync(DbConnection connection, string tableName)
        {
            var command = connection.CreateCommand();
            command.CommandText = string.Format(Queries.IndexesForTable, tableName);

            using var reader = await command.ExecuteReaderAsync();
            var list = new List<string>();

            while (await reader.ReadAsync())
            {
                list.Add(reader.GetString(0));
            }

            return list;
        }

        public static async Task<List<string>> GetTableNamesAsync(DbConnection connection)
        {
            var command = connection.CreateCommand();
            command.CommandText = Queries.TablesInDatabase;

            using var reader = await command.ExecuteReaderAsync();
            var list = new List<string>();

            while (await reader.ReadAsync())
            {
                list.Add(reader.GetString(0));
            }

            return list;
        }
    }
}
