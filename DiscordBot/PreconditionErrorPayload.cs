using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot
{
    public class PreconditionErrorPayload
    {
        public string? ImageUrl { get; set; }

        public string? Message { get; set; }

        public string Serialize()
        {
            return JsonSerializer.Serialize(this);
        }

        public static PreconditionErrorPayload Deserialize(string json)
        {
            return JsonSerializer.Deserialize<PreconditionErrorPayload>(json)
                ?? throw new Exception("Invalid json");
        }
    }
}
