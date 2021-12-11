using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.DAL.MySql.Schema
{
    public static class Utils
    {
        public const string Placeholder = "SELECT 12345679";

        private static Task ExecuteNonQueryAsync(IDbConnection connection, string commandText)
        {
            var command = connection.CreateCommand();
            command.CommandText = commandText;
            return command.ExecuteNonQueryAsync();
        }

        public static Task StubSelectAsync(IDbConnection connection)
            => ExecuteNonQueryAsync(connection, Placeholder);

        public static async Task<List<string>> GetLastQueriesAsync(IDbConnection connection)
        {
            var stringBuilder = new StringBuilder(1000);
            var queries = new List<string>();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT argument FROM mysql.general_log";

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var query = await reader.GetTextReader(0).ReadToEndAsync();

                if (query.Contains("SET GLOBAL")
                    || query.Contains("SET NAMES utf8mb4")
                    || query.Contains("root@localhost on SanakanDBSchema"))
                {
                    continue;
                }

                if (query.Contains(Placeholder))
                {
                    query = stringBuilder.ToString();
                    stringBuilder.Clear();

                    if (string.IsNullOrEmpty(query))
                    {
                        continue;
                    }

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

        public static Task TruncateGeneralLogAsync(IDbConnection connection)
            => ExecuteNonQueryAsync(connection, "TRUNCATE TABLE mysql.general_log");

        public static Task ToggleGeneralLogAsync(IDbConnection connection, string mode)
            => ExecuteNonQueryAsync(connection, $"SET GLOBAL general_log = '{mode}'");

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

        public static async Task<List<string>> GetTableNamesAsync(IDbConnection connection)
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
