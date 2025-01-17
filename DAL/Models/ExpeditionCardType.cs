﻿namespace Sanakan.DAL.Models
{
    public enum ExpeditionCardType : byte
    {
        None = 0,
        NormalItemWithExp = 1,
        ExtremeItemWithExp = 2,
        DarkExp = 3,
        DarkItems = 4,
        DarkItemWithExp = 5,
        LightExp = 6,
        LightItems = 7,
        LightItemWithExp = 8,
        UltimateEasy = 9,
        UltimateMedium = 10,
        UltimateHard = 11,
        UltimateHardcore = 12
    }

    public static class ExpeditionCardTypeExtensions
    {
        public static double GetCostOfExpeditionPerMinuteRaw(this ExpeditionCardType expeditionCardType)
        {
            switch (expeditionCardType)
            {
                case ExpeditionCardType.NormalItemWithExp:
                    return 0.015;

                case ExpeditionCardType.ExtremeItemWithExp:
                    return 0.17;

                case ExpeditionCardType.DarkExp:
                case ExpeditionCardType.LightExp:
                case ExpeditionCardType.LightItems:
                case ExpeditionCardType.DarkItems:
                    return 0.12;

                case ExpeditionCardType.DarkItemWithExp:
                case ExpeditionCardType.LightItemWithExp:
                    return 0.07;

                case ExpeditionCardType.UltimateEasy:
                case ExpeditionCardType.UltimateMedium:
                    return 1;

                case ExpeditionCardType.UltimateHard:
                    return 2;

                case ExpeditionCardType.UltimateHardcore:
                    return 0.5;

                default:
                    return 0;
            }
        }

        public static double GetKarmaCostInExpeditionPerMinute(this Card card)
        {
            switch (card.Expedition)
            {
                case ExpeditionCardType.NormalItemWithExp:
                    return 0.0009;

                case ExpeditionCardType.ExtremeItemWithExp:
                    return 0.028;

                case ExpeditionCardType.DarkItemWithExp:
                case ExpeditionCardType.DarkItems:
                case ExpeditionCardType.DarkExp:
                    return 0.0018;

                case ExpeditionCardType.LightItemWithExp:
                case ExpeditionCardType.LightExp:
                case ExpeditionCardType.LightItems:
                    return 0.0042;

                default:
                case ExpeditionCardType.UltimateEasy:
                case ExpeditionCardType.UltimateMedium:
                case ExpeditionCardType.UltimateHard:
                case ExpeditionCardType.UltimateHardcore:
                    return 0;
            }
        }

        public static string GetName(this ExpeditionCardType expedition, string end = "a")
        {
            switch (expedition)
            {
                case ExpeditionCardType.NormalItemWithExp:
                    return $"normaln{end}";

                case ExpeditionCardType.ExtremeItemWithExp:
                    return $"niemożliw{end}";

                case ExpeditionCardType.DarkExp:
                case ExpeditionCardType.DarkItems:
                case ExpeditionCardType.DarkItemWithExp:
                    return $"nikczemn{end}";

                case ExpeditionCardType.LightExp:
                case ExpeditionCardType.LightItems:
                case ExpeditionCardType.LightItemWithExp:
                    return $"heroiczn{end}";

                case ExpeditionCardType.UltimateEasy:
                    return $"niezwykł{end} (E)";
                case ExpeditionCardType.UltimateMedium:
                    return $"niezwykł{end} (M)";
                case ExpeditionCardType.UltimateHard:
                    return $"niezwykł{end} (H)";
                case ExpeditionCardType.UltimateHardcore:
                    return $"niezwykł{end} (HH)";

                default:
                case ExpeditionCardType.None:
                    return "-";
            }
        }

        public static bool HasDifferentQualitiesOnExpedition(this ExpeditionCardType expedition)
        {
            switch (expedition)
            {
                case ExpeditionCardType.UltimateEasy:
                case ExpeditionCardType.UltimateMedium:
                case ExpeditionCardType.UltimateHard:
                case ExpeditionCardType.UltimateHardcore:
                case ExpeditionCardType.ExtremeItemWithExp:
                    return true;

                default:
                    return false;
            }
        }
    }
}
