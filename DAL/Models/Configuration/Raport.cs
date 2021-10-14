using Newtonsoft.Json;

namespace Sanakan.DAL.Models.Configuration
{
    public class Raport
    {
        public ulong Id { get; set; }
        public ulong User { get; set; }
        public ulong Message { get; set; }

        public ulong GuildOptionsId { get; set; }
        [JsonIgnore]
        public virtual GuildOptions GuildOptions { get; set; }
    }
}
