using Newtonsoft.Json;

namespace Sanakan.DAL.Models.Configuration
{
    public class WithoutExpChannel
    {
        public ulong Id { get; set; }
        public ulong Channel { get; set; }

        public ulong GuildOptionsId { get; set; }
        [JsonIgnore]
        public virtual GuildOptions GuildOptions { get; set; }
    }
}
