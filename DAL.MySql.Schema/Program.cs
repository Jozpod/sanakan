using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sanakan.Common;
using Sanakan.Common.Builder;
using Sanakan.Common.Configuration;
using Sanakan.Configuration;
using Sanakan.DAL.Builder;
using Sanakan.DAL.Repositories.Abstractions;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Text;
using System.Threading.Tasks;


namespace Sanakan.DAL.MySql.Schema
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var configurationRoot = builder.Build();

            serviceCollection.AddOptions();
            serviceCollection.AddSanakanDbContext(configurationRoot);
            serviceCollection.AddSingleton(configurationRoot);
            serviceCollection.AddCache(configurationRoot.GetSection("Cache"));
            serviceCollection.Configure<SanakanConfiguration>(configurationRoot);
            serviceCollection.AddRepositories();
            serviceCollection.AddFileSystem();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var fileSystem = serviceProvider.GetRequiredService<IFileSystem>();
            var dbContext = serviceProvider.GetRequiredService<SanakanDbContext>();
            var databaseFacade = dbContext.Database;
            await databaseFacade.EnsureCreatedAsync();
            using var connection = databaseFacade.GetDbConnection();
            await connection.OpenAsync();
            
            var tableNames = await GetTableNamesAsync(connection);

            var path = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\"));
            var tablesFolder = Path.Combine(path, "Tables");
            
            if(!fileSystem.DirectoryExists(tablesFolder))
            {
                fileSystem.CreateDirectory(tablesFolder);
            }

            var stringBuilder = new StringBuilder(1000);

            foreach(var tableName in tableNames)
            {
                var tableDefinition = await GetTableDefinitionAsync(connection, tableName);
                var filePath = Path.Combine(tablesFolder, $"{tableName}.sql");

                var indexes = await GetTableIndexesAsync(connection, tableName);

                stringBuilder.AppendLine(tableDefinition);

                foreach (var index in indexes)
                {
                    stringBuilder.AppendLine(index);
                }

                await fileSystem.WriteAllTextAsync(filePath, stringBuilder.ToString());
                stringBuilder.Clear();
            }

            await databaseFacade.EnsureDeletedAsync();
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
