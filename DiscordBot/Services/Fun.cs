using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Discord.WebSocket;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Services;
using Sanakan.Extensions;

namespace Sanakan.Services
{
    public class Fun
    {

        public static bool TakeATry(int chance) => (GetRandomValue(chance*100) % chance) == 1;

        public CoinSide RandomizeSide()
            => (CoinSide) GetRandomValue(2);

        public string GetSlotMachineInfo()
        {
            return $"**Nastawa** / **Wartośći** \n"
                    + $"Info `(wypisywanie informacji)`\n"
                    + $"Mnożnik / `1`, `2`, `3`\n"
                    + $"Stawka / `1`, `10`, `100`\n"
                    + $"Rzędy / `1`, `2`, `3`";
        }

        public string GetSlotMachineResult(string slots, SocketUser user, User botUser, long win)
        {
            string psay = (botUser.SMConfig.PsayMode > 0) ? "<:klasycznypsaj:482136878120828938> " : " ";

            return $"{psay}**Gra:** {user.Mention}\n\n ➖➖➖➖➖➖ \n{slots}\n ➖➖➖➖➖➖ \n"
                + $"**Stawka:** `{botUser.SMConfig.Beat.Value()} SC`\n" 
                + $"**Mnożnik:** `x{botUser.SMConfig.Multiplier.Value()}`\n\n**Wygrana:** `{win} SC`";
        }

        public string GetSlotMachineGameInfo()
        {
            string info = $"**Info:**\n\n✖ - nieaktywny rząd\n✔ - aktywny rząd\n\n**Wygrane:**\n\n"
                + $"3-5x<:klasycznypsaj:482136878120828938> - tryb psaja (podwójne wygrane)\n\n";

            foreach (SlotMachineSlots em in Enum.GetValues(typeof(SlotMachineSlots)))
            {
                if (em != SlotMachineSlots.max && em != SlotMachineSlots.q)
                {
                    for (int i = 3; i < 6; i++)
                    {
                        string val = $"x{em.WinValue(i)}";
                        info += $"{i}x{em.Icon()} - {val.PadRight(5, ' ')} ";
                    }
                    info += "\n";
                }
            }

            return info;
        }
    }
}