﻿using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.Common;
using Sanakan.Common.Builder;
using Sanakan.Common.Configuration;
using Sanakan.Common.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sanakan.DAL.Tests
{
    [TestClass]
    public class WritableOptionsTests
    {
        private ServiceProvider _serviceProvider;

        public WritableOptionsTests()
        {
            var _jsonSerializerOptions = new JsonSerializerOptions();
            _jsonSerializerOptions.Converters.Add(new VersionConverter());
            _jsonSerializerOptions.Converters.Add(new TimeSpanConverter());
            _jsonSerializerOptions.Converters.Add(new TimeZoneInfoConverter());
            _jsonSerializerOptions.Converters.Add(new CultureInfoConverter());

            var stream = File.OpenRead("appsettings.json");
            var configuration = JsonSerializer.DeserializeAsync<SanakanConfiguration>(stream, _jsonSerializerOptions).Result;
            stream.Close();

            var serviceCollection = new ServiceCollection();

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            var configurationRoot = builder.Build();
            
            serviceCollection.AddOptions();
            serviceCollection.AddFileSystem();
            serviceCollection.AddSingleton<IHostEnvironment>((sp) => {
                return new HostingEnvironment()
                {
                    ContentRootFileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory()),
                };
            });

            serviceCollection.Configure<SanakanConfiguration>(configurationRoot);
            serviceCollection.AddWritableOption<SanakanConfiguration>();
            serviceCollection.AddSingleton<IConfiguration>(configurationRoot);
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        [TestMethod]
        public async Task Should_Save_Options_And_Read_Again()
        {
            var sanakanConfiguration = _serviceProvider.GetRequiredService<IWritableOptions<SanakanConfiguration>>();
            var random = new Random();
            var charPerPacket = (ulong)random.Next(1000);
            var charPerPoint = random.Next(1000);
            var minPerMessage = random.Next(1000);

            await sanakanConfiguration.UpdateAsync(opts =>
            {
                opts.Experience.CharPerPacket = charPerPacket;
                opts.Experience.CharPerPoint = charPerPoint;
                opts.Experience.MinPerMessage = minPerMessage;
            });
            sanakanConfiguration.Value.Experience.CharPerPacket.Should().Be(charPerPacket);
            sanakanConfiguration.Value.Experience.CharPerPoint.Should().Be(charPerPoint);
            sanakanConfiguration.Value.Experience.MinPerMessage.Should().Be(minPerMessage);
        }
    }
}
