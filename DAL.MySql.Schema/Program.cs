using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sanakan.Common.Builder;
using Sanakan.Configuration;
using Sanakan.DAL.Builder;
using Sanakan.DAL.Repositories.Abstractions;
using System.IO;
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
            serviceCollection.AddDbContext<SanakanDbContext>();
            serviceCollection.AddSingleton(configurationRoot);
            serviceCollection.AddCache(configurationRoot.GetSection("Cache"));
            serviceCollection.Configure<SanakanConfiguration>(configurationRoot);
            serviceCollection.AddRepositories();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var dbContext = serviceProvider.GetRequiredService<SanakanDbContext>();
            await dbContext.Database.EnsureCreatedAsync();

            var connection = dbContext.Database.GetDbConnection();

            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = "SHOW CREATE TABLE Card";
            var schemaDefinition = await command.ExecuteScalarAsync();

            await connection.CloseAsync();
        }
    }
}
