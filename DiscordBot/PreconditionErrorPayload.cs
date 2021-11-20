using System;
using System.Text.Json;

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
