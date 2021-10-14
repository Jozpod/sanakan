using Newtonsoft.Json;

namespace Sanakan.DAL.Models.Configuration
{
    public class WaifuFightChannel
    {
        public ulong Id { get; set; }
        public ulong Channel { get; set; }

        public ulong WaifuId { get; set; }
        [JsonIgnore]
        public virtual Waifu Waifu { get; set; }
    }
}
