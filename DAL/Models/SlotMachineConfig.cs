using Newtonsoft.Json;

namespace Sanakan.DAL.Models
{

    public class SlotMachineConfig
    {
        public ulong Id { get; set; }
        public long PsayMode { get; set; }
        public SlotMachineBeat Beat { get; set; }
        public SlotMachineSelectedRows Rows { get; set; }
        public SlotMachineBeatMultiplier Multiplier { get; set; }

        public ulong UserId { get; set; }
        [JsonIgnore]
        public virtual User User { get; set; }
    }
}
