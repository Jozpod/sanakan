using System.Collections.Generic;

namespace Sanakan.DAL.Models
{
    public enum StatusType : byte
    {
        Hourly = 0,
        Daily = 1,
        Globals = 2,
        Color = 3,
        Market = 4,
        Card = 5,

        /// <summary>
        /// Related to card bundles/packets.
        /// </summary>
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

        public static List<StatusType> WeeklyQuestTypes = new()
        {
            StatusType.WCardPlus,
            StatusType.WDaily,
        };

        public static bool IsQuest(this StatusType type) => type.IsWeeklyQuestType() || type.IsDailyQuestType();

        public static string Name(this StatusType type) => type switch
        {
            StatusType.Color => "Kolor",
            StatusType.Globals => "Globalne emoty",
            StatusType.DHourly => "Odbierz zaskórniaki",
            StatusType.DExpeditions => "Wyślij karte na wyprawę",
            StatusType.DMarket => "Odwiedź rynek lub czarny rynek",
            StatusType.DPacket => "Otwórz pakiet kart",
            StatusType.DPvp => "Rozegraj pojedynek PVP",
            StatusType.DUsedItems => "Użyj przedmiot",
            StatusType.WCardPlus => "Odbierz Karte+",
            StatusType.WDaily => "Odbierz drobne",
            _ => "--",
        };

        public static bool IsSubType(this StatusType type) => type switch
        {
            StatusType.Color or StatusType.Globals => true,
            _ => false,
        };

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

        public static bool IsWeeklyQuestType(this StatusType type) => type switch
        {
            StatusType.WCardPlus or StatusType.WDaily => true,
            _ => false,
        };

        public static int ToComplete(this StatusType type) => type switch
        {
            StatusType.DExpeditions => 3,
            StatusType.DUsedItems => 10,
            StatusType.DHourly => 4,
            StatusType.DPacket => 2,
            StatusType.DMarket => 2,
            StatusType.DPvp => 5,
            StatusType.WCardPlus => 7,
            StatusType.WDaily => 7,
            _ => -1,
        };

        public static string GetEmoteString(this StatusType type) => type switch
        {
            StatusType.DExpeditions => "<:icon_expeditions:829327738124369930>",
            StatusType.DUsedItems => "<:icon_items:829327738141409310>",
            StatusType.DHourly => "<:icon_money:829327739340718121>",
            StatusType.DPacket => "<:icon_packet:829327738585743400>",
            StatusType.DMarket => "<:icon_market:829327738145210399>",
            StatusType.DPvp => "<:icon_pvp:829327738157662229>",
            StatusType.WCardPlus => "<a:miko:826132578703507526>",
            StatusType.WDaily => "<a:gamemoney:465528603266777101>",
            _ => "",
        };

        public static string GetRewardString(this StatusType type) => type switch
        {
            StatusType.DExpeditions => "1 AC",
            StatusType.DUsedItems => "1 AC",
            StatusType.DHourly => "100 SC",
            StatusType.DPacket => "2 AC",
            StatusType.DMarket => "2 AC",
            StatusType.DPvp => "200 PC",
            StatusType.WCardPlus => "50 AC",
            StatusType.WDaily => "1000 SC i 10 AC",
            _ => "",
        };
    }
}
