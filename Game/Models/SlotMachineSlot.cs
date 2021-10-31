using System;

namespace Sanakan.Game.Models
{
    public enum SlotMachineSlot : int
    {
        p = 0,
        c = 1,
        q = 2,
        f = 3,
        g = 4,
        z = 5,
        j = 6,
        max = 7
    }

    public static class SlotMachineSlotsExtensions
    {
        public static SlotMachineSlot[] SlotMachineSlots = (SlotMachineSlot[])Enum.GetValues(typeof(SlotMachineSlot));

        public static SlotMachineWinSlots WinType(this SlotMachineSlot slot, int count)
        {
            if (count < 3)
            {
                return SlotMachineWinSlots.Nothing;
            }
            return (SlotMachineWinSlots)Enum.Parse(typeof(SlotMachineWinSlots), $"{slot}{count}");
        }

        public static string Icon(this SlotMachineSlot slot, bool psay = false)
        {
            switch (slot)
            {
                case SlotMachineSlot.j: return psay ? "<:psajajaj:481762467534471178>" : "⭐";
                case SlotMachineSlot.z: return psay ? "<:rozowypsaj:481757430943055892>" : "🍑";
                case SlotMachineSlot.g: return psay ? "<:fioletowypsaj:481756959419400192>" : "🍒";
                case SlotMachineSlot.f: return psay ? "<:niebieskipsaj:481758813024681994>" : "🦋";
                case SlotMachineSlot.c: return psay ? "<:zielonypsaj:481757219394813952>" : "🐸";
                case SlotMachineSlot.p: return psay ? "<:brazowypsaj:482158913744142368>" : "🐷";
                case SlotMachineSlot.q: return "<:klasycznypsaj:482136878120828938>";
                default: return "✖";
            }
        }

        public static int WinValue(this SlotMachineSlot slot, int count, bool psay = false)
        {
            if (count < 3)
            {
                return 0;
            }
            var win = (int)(SlotMachineWinSlots)Enum.Parse(typeof(SlotMachineWinSlots), $"{slot}{count}");
            return psay ? (win * 2) : win;
        }
    }
}
