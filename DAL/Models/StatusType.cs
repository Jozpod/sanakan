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
        Daily = 1,
        Globals = 2,
        Color = 3,
        Market = 4,
        Card = 5,
        Packet = 6,
        Pvp = 7,
        Flood = 16, // normal
        DPacket = 8,
        DHourly = 9,
        DMarket = 10,
        DUsedItems = 11,
        DExpeditions = 12,
        DPvp = 13, // daily quests
        WDaily = 14,
        WCardPlus = 15,  // weekly quests
    }

    public static class StatusTypeExtensions
    {
        public static List<StatusType> DailyQuestTypes = new List<StatusType>()
        {
            StatusType.DExpeditions,
            StatusType.DUsedItems,
            StatusType.DHourly,
            StatusType.DPacket,
            StatusType.DMarket,
            StatusType.DPvp,
        };

        public static List<StatusType> WeeklyQuestTypes = new List<StatusType>()
        {
            StatusType.WCardPlus,
            StatusType.WDaily,
        };

        public static string Name(this StatusType type)
        {
            switch (type)
            {
                case StatusType.Color:
                    return "Kolor";

                case StatusType.Globals:
                    return "Globalne emoty";

                case StatusType.DHourly:
                    return "Odbierz zaskórniaki";

                case StatusType.DExpeditions:
                    return "Wyślij karte na wyprawę";

                case StatusType.DMarket:
                    return "Odwiedź rynek lub czarny rynek";

                case StatusType.DPacket:
                    return "Otwórz pakiet kart";

                case StatusType.DPvp:
                    return "Rozegraj pojedynek PVP";

                case StatusType.DUsedItems:
                    return "Użyj przedmiot";

                case StatusType.WCardPlus:
                    return "Odbierz Karte+";

                case StatusType.WDaily:
                    return "Odbierz drobne";

                default:
                    return "--";
            }
        }

        public static bool IsSubType(this StatusType type)
        {
            switch (type)
            {
                case StatusType.Color:
                case StatusType.Globals:
                    return true;

                default:
                    return false;
            }
        }

        public static bool IsDailyQuestType(this StatusType type)
        {
            switch (type)
            {
                case StatusType.DExpeditions:
                case StatusType.DUsedItems:
                case StatusType.DHourly:
                case StatusType.DPacket:
                case StatusType.DMarket:
                case StatusType.DPvp:
                    return true;

                default:
                    return false;
            }
        }

        public static bool IsWeeklyQuestType(this StatusType type)
        {
            switch (type)
            {
                case StatusType.WCardPlus:
                case StatusType.WDaily:
                    return true;

                default:
                    return false;
            }
        }

        public static int ToComplete(this StatusType type)
        {
            switch (type)
            {
                case StatusType.DExpeditions: return 3;
                case StatusType.DUsedItems: return 10;
                case StatusType.DHourly: return 4;
                case StatusType.DPacket: return 2;
                case StatusType.DMarket: return 2;
                case StatusType.DPvp: return 5;

                case StatusType.WCardPlus: return 7;
                case StatusType.WDaily: return 7;

                default:
                    return -1;
            }
        }

        public static string GetEmoteString(this StatusType type)
        {
            switch (type)
            {
                case StatusType.DExpeditions: return "<:icon_expeditions:829327738124369930>";
                case StatusType.DUsedItems: return "<:icon_items:829327738141409310>";
                case StatusType.DHourly: return "<:icon_money:829327739340718121>";
                case StatusType.DPacket: return "<:icon_packet:829327738585743400>";
                case StatusType.DMarket: return "<:icon_market:829327738145210399>";
                case StatusType.DPvp: return "<:icon_pvp:829327738157662229>";

                case StatusType.WCardPlus: return "<a:miko:826132578703507526>";
                case StatusType.WDaily: return "<a:gamemoney:465528603266777101>";

                default:
                    return "";
            }
        }

        public static string GetRewardString(this StatusType type)
        {
            switch (type)
            {
                case StatusType.DExpeditions: return "1 AC";
                case StatusType.DUsedItems: return "1 AC";
                case StatusType.DHourly: return "100 SC";
                case StatusType.DPacket: return "2 AC";
                case StatusType.DMarket: return "2 AC";
                case StatusType.DPvp: return "200 PC";

                case StatusType.WCardPlus: return "50 AC";
                case StatusType.WDaily: return "1000 SC i 10 AC";

                default:
                    return "";
            }
        }
    }
}
