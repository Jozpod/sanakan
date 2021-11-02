using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Services;
using Sanakan.Game.Models;
using Sanakan.Services;
using System;

namespace Sanakan.Extensions
{
    public static class SlotMachineExtension
    {
        public static bool ApplySlotMachineSetting(this User user, SlotMachineSetting type, string value)
        {
            try
            {
                switch (type)
                {
                    case SlotMachineSetting.Beat:
                        var bt = (SlotMachineBeat)Enum.Parse(typeof(SlotMachineBeat), $"b{value}");
                        user.SMConfig.Beat = bt;
                        break;
                    case SlotMachineSetting.Rows:
                        var rw = (SlotMachineSelectedRows)Enum.Parse(typeof(SlotMachineSelectedRows), $"r{value}");
                        user.SMConfig.Rows = rw;
                        break;
                    case SlotMachineSetting.Multiplier:
                        var mt = (SlotMachineBeatMultiplier)Enum.Parse(typeof(SlotMachineBeatMultiplier), $"x{value}");
                        user.SMConfig.Multiplier = mt;
                        break;

                    default:
                        return false;
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static int Value(this SlotMachineBeatMultiplier multi) => (int)multi;
        public static int Value(this SlotMachineSlot winSlot) => (int)winSlot;
        public static int Value(this SlotMachineSelectedRows slot) => (int)slot;
        public static int Value(this SlotMachineBeat beat) => (int)beat;

        public static SlotMachineSlot ToSMS(this int sl) => (SlotMachineSlot)sl;
    }
}
