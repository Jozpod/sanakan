using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Services;
using Sanakan.Game.Models;
using Sanakan.Services;
using System;

namespace Sanakan.Extensions
{
    public static class SlotMachineExtension
    {

        public static int Value(this SlotMachineBeatMultiplier multi) => (int)multi;
        public static int Value(this SlotMachineSlot winSlot) => (int)winSlot;
        public static int Value(this SlotMachineSelectedRows slot) => (int)slot;
        public static int Value(this SlotMachineBeat beat) => (int)beat;

        public static SlotMachineSlot ToSMS(this int sl) => (SlotMachineSlot)sl;
    }
}
