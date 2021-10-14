using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.DAL.Models
{
    public enum StatusType
    {
        Hourly = 0,
        Daily = 1, Globals = 2,
        Color = 3, Market = 4, Card = 5,
        Packet = 6, Pvp = 7,
        Flood = 16,   // normal
        DPacket = 8,
        DHourly = 9,
        DMarket = 10,
        DUsedItems = 11,
        DExpeditions = 12,
        DPvp = 13,                  // daily quests
        WDaily = 14,
        WCardPlus = 15,                                                                            // weekly quests
    }
}
