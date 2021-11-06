using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Sanakan.Common.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sanakan.Common
{
    public class WritableOptions<T> : IWritableOptions<T> where T : class, new()
    {
        private readonly IHostEnvironment _environment;
        private readonly IFileSystem _fileSystem;
        private readonly IOptionsMonitor<T> _options;
        private readonly IConfigurationRoot _configuration;
        private readonly string _file;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public WritableOptions(
            IHostEnvironment environment,
            IFileSystem fileSystem,
            IOptionsMonitor<T> options,
            IConfigurationRoot configuration,
            string file)
        {
            _environment = environment;
            _fileSystem = fileSystem;
            _options = options;
            _configuration = configuration;
            _file = file;
            _jsonSerializerOptions = new JsonSerializerOptions();
            _jsonSerializerOptions.Converters.Add(new VersionConverter());
            _jsonSerializerOptions.Converters.Add(new TimeSpanConverter());
            _jsonSerializerOptions.Converters.Add(new TimeZoneInfoConverter());
            _jsonSerializerOptions.Converters.Add(new CultureInfoConverter());
        }

        public T Value => _options.CurrentValue;
        public T Get(string name) => _options.Get(name);

        public async Task UpdateAsync(Action<T> applyChanges)
        {
            var fileProvider = _environment.ContentRootFileProvider;
            var fileInfo = fileProvider.GetFileInfo(_file);
            var physicalPath = fileInfo.PhysicalPath;

            using var stream = _fileSystem.Open(physicalPath, FileMode.OpenOrCreate);
            var configuration = await JsonSerializer.DeserializeAsync<T>(stream, _jsonSerializerOptions);
            applyChanges(configuration);

            stream.Seek(0, SeekOrigin.Begin);
            await JsonSerializer.SerializeAsync(stream, configuration, _jsonSerializerOptions);
            _configuration.Reload();
        }
    }
}
