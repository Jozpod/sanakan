using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Sanakan.Common
{
    public static class ServiceCollectionExtensions
    {
        public static void AddWritableOption<T>(
            this IServiceCollection services,
            string file = "appsettings.json")
            where T : class, new()
        {
            services.AddTransient<IWritableOptions<T>>(serviceProvider =>
            {
                var configuration = (IConfigurationRoot)serviceProvider.GetRequiredService<IConfiguration>();
                var logger = serviceProvider.GetRequiredService<ILogger<WritableOptions<T>>>();
                var environment = serviceProvider.GetRequiredService<IHostEnvironment>();
                var options = serviceProvider.GetRequiredService<IOptionsMonitor<T>>();
                var fileSystem = serviceProvider.GetRequiredService<IFileSystem>();
                return new WritableOptions<T>(
                    logger,
                    environment,
                    fileSystem,
                    options,
                    configuration,
                    file);
            });
        }
    }
}
