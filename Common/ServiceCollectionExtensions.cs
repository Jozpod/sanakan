﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sanakan.Common
{
    public static class ServiceCollectionExtensions
    {
        public static void ConfigureWritable<T>(
            this IServiceCollection services,
            IConfigurationSection section,
            string file = "appsettings.json") where T : class, new()
        {
            services.Configure<T>(section);
            services.AddTransient<IWritableOptions<T>>(provider =>
            {
                var configuration = (IConfigurationRoot)provider.GetRequiredService<IConfiguration>();
                var environment = provider.GetService<IHostingEnvironment>();
                var options = provider.GetService<IOptionsMonitor<T>>();
                var fileSystem = provider.GetRequiredService<IFileSystem>();
                return new WritableOptions<T>(
                    environment,
                    fileSystem,
                    options,
                    configuration,
                    section.Key,
                    file);
            });
        }
    }
}
