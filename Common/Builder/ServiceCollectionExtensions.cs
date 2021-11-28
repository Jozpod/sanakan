using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace Sanakan.Common
{
    public static class ServiceCollectionExtensions
    {
        public static void AddWritableOption<T>(
            this IServiceCollection services,
            string file = "appsettings.json") where T : class, new()
        {
            services.AddTransient<IWritableOptions<T>>(serviceProvider =>
            {
                var configuration = (IConfigurationRoot)serviceProvider.GetRequiredService<IConfiguration>();
                var logger = serviceProvider.GetService<ILogger<WritableOptions<T>>>();
                var environment = serviceProvider.GetService<IHostEnvironment>();
                var options = serviceProvider.GetService<IOptionsMonitor<T>>();
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
