using System;

namespace Sanakan.Game.Models
{
    public enum SlotMachineWinSlots : int
    {
        Nothing = 0,
        q3 = 4,
        q4 = 8,
        q5 = 18,
        p3 = 2,
        p4 = 10,
        p5 = 20,
        c3 = 3,
        c4 = 15,
        c5 = 30,
        f3 = 5,
        f4 = 25,
        f5 = 50,
        g3 = 10,
        g4 = 50,
        g5 = 100,
        z3 = 30,
        z4 = 150,
        z5 = 300,
        j3 = 100,
        j4 = 300,
        j5 = 500,
    }

    public static class SlotMachineWinSlotsExtensions
    {
        public static string ToIcon(this SlotMachineWinSlots wins)
        {
            if (wins == SlotMachineWinSlots.Nothing)
                return "✖";

            var winString = wins.ToString();
            var winChars = winString.ToCharArray();
            var count = winChars[1];
            var icon = (SlotMachineSlot)Enum.Parse(typeof(SlotMachineSlot), $"{winChars[0]}");

            return $"{count}x{icon.Icon()}";
        }
    }
}
