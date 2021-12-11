using Microsoft.EntityFrameworkCore;
using Sanakan.Common;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.DAL.MySql.Schema
{
    public class TableScripter
    {
        private readonly IFileSystem _fileSystem;
        private readonly Microsoft.EntityFrameworkCore.Infrastructure.DatabaseFacade _databaseFacade;

        public TableScripter(
            IFileSystem fileSystem,
            Microsoft.EntityFrameworkCore.Infrastructure.DatabaseFacade databaseFacade)
        {
            _fileSystem = fileSystem;
            _databaseFacade = databaseFacade;
        }

        public async Task RunAsync()
        {
            var path = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\"));
            var tablesFolder = Path.Combine(path, "Tables");

            var baseConnection = _databaseFacade.GetDbConnection();
            var connection = new DbConnection(baseConnection.ConnectionString);
            
            await connection.OpenAsync();
            var tableNames = await Utils.GetTableNamesAsync(connection);

            if (!_fileSystem.DirectoryExists(tablesFolder))
            {
                _fileSystem.CreateDirectory(tablesFolder);
            }

            var stringBuilder = new StringBuilder(1000);

            foreach (var tableName in tableNames)
            {
                var tableDefinition = await Utils.GetTableDefinitionAsync(connection, tableName);
                var filePath = Path.Combine(tablesFolder, $"{tableName}.sql");

                var tableIndexes = await Utils.GetTableIndexesAsync(connection, tableName);

                stringBuilder.AppendLine(tableDefinition);

                foreach (var tableIndex in tableIndexes)
                {
                    stringBuilder.AppendLine(tableIndex);
                }

                await _fileSystem.WriteAllTextAsync(filePath, stringBuilder.ToString());
                stringBuilder.Clear();
            }
        }
    }
}
