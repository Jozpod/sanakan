using System.Text.Json.Serialization;

namespace Sanakan.DAL.Models.Configuration
{
    public class WithoutSupervisionChannel
    {
        public ulong ChannelId { get; set; }

        public ulong GuildOptionsId { get; set; }

        [JsonIgnore]
        public virtual GuildOptions GuildOptions { get; set; } = null;
    }
}
