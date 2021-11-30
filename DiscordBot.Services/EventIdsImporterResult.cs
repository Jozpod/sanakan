using System;
using System.Collections.Generic;
using System.Linq;

namespace Sanakan.DiscordBot.Services
{
    public class EventIdsImporterResult
    {
        public EventIdsImporterState State { get; set; }

        public Exception? Exception { get; set; }

        public IEnumerable<ulong> Value { get; set; } = Enumerable.Empty<ulong>();
    }
}
