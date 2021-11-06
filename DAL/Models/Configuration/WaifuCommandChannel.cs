using System.Text.Json.Serialization;

namespace Sanakan.DAL.Models.Configuration
{
    public class WaifuCommandChannel
    {
        public ulong Id { get; set; }
        public ulong Channel { get; set; }

        public ulong WaifuId { get; set; }
        
        [JsonIgnore]
        public virtual WaifuConfiguration Waifu { get; set; }
    }
}
