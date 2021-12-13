using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sanakan.Common.Converters;
using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Sanakan.Common
{
    internal class WritableOptions<T> : IWritableOptions<T>
        where T : class, new()
    {
        private readonly ILogger _logger;
        private readonly IHostEnvironment _environment;
        private readonly IFileSystem _fileSystem;
        private readonly IOptionsMonitor<T> _options;
        private readonly IConfigurationRoot _configuration;
        private readonly string _file;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public WritableOptions(
            ILogger<WritableOptions<T>> logger,
            IHostEnvironment environment,
            IFileSystem fileSystem,
            IOptionsMonitor<T> options,
            IConfigurationRoot configuration,
            string file)
        {
            _logger = logger;
            _environment = environment;
            _fileSystem = fileSystem;
            _options = options;
            _configuration = configuration;
            _file = file;
            _jsonSerializerOptions = new JsonSerializerOptions()
            {
                WriteIndented = true,
            };
            _jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            _jsonSerializerOptions.Converters.Add(new VersionConverter());
            _jsonSerializerOptions.Converters.Add(new TimeSpanConverter());
            _jsonSerializerOptions.Converters.Add(new TimeZoneInfoConverter());
            _jsonSerializerOptions.Converters.Add(new CultureInfoConverter());
        }

        public T Value => _options.CurrentValue;
        public T Get(string name) => _options.Get(name);

        public async Task<bool> UpdateAsync(Action<T> applyChanges)
        {
            var fileProvider = _environment.ContentRootFileProvider;
            var fileInfo = fileProvider.GetFileInfo(_file);
            var physicalPath = fileInfo.PhysicalPath;

            try
            {
                using var stream = _fileSystem.Open(physicalPath, FileMode.Truncate);
                applyChanges(_options.CurrentValue);
                await JsonSerializer.SerializeAsync(stream, _options.CurrentValue, _jsonSerializerOptions);
                stream.Close();

                _configuration.Reload();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Could not update {typeof(T)} configuration");
                return false;
            }
        }
    }
}
