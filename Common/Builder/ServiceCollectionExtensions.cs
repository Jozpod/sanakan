﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Sanakan.Common
{
    public static class ServiceCollectionExtensions
    {
        public static void AddWritableOption<T>(
            this IServiceCollection services,
            string file = "appsettings.json") where T : class, new()
        {
            services.AddTransient<IWritableOptions<T>>(provider =>
            {
                var configuration = (IConfigurationRoot)provider.GetRequiredService<IConfiguration>();
                var environment = provider.GetService<IHostEnvironment>();
                var options = provider.GetService<IOptionsMonitor<T>>();
                var fileSystem = provider.GetRequiredService<IFileSystem>();
                return new WritableOptions<T>(
                    environment,
                    fileSystem,
                    options,
                    configuration,
                    file);
            });
        }
    }
}
