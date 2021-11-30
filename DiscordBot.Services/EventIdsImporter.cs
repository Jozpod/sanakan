using Sanakan.DiscordBot.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Services
{
    public class EventIdsImporter : IEventIdsImporter
    {
        private readonly HttpClient _httpClient;

        public EventIdsImporter(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<EventIdsImporterResult> RunAsync(string url)
        {
            var ids = new List<ulong>();
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return new EventIdsImporterResult
                {
                    State = EventIdsImporterState.InvalidStatusCode,
                };
            }

            using var stream = await response.Content.ReadAsStreamAsync();
            using var streamReader = new StreamReader(stream);
            var content = await streamReader.ReadToEndAsync();

            try
            {
                return new EventIdsImporterResult
                {
                    Value = content.Split(";").Select(x => ulong.Parse(x)),
                };
            }
            catch (Exception ex)
            {
                return new EventIdsImporterResult
                {
                    State = EventIdsImporterState.InvalidFileFormat,
                    Exception = ex,
                };
            }
        }
    }
}
