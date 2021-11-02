namespace Sanakan.DAL.Models
{
    public enum ExpeditionCardType
    {
        None,
        NormalItemWithExp,
        ExtremeItemWithExp,
        DarkExp,
        DarkItems,
        DarkItemWithExp,
        LightExp,
        LightItems,
        LightItemWithExp,
        UltimateEasy,
        UltimateMedium,
        UltimateHard,
        UltimateHardcore
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

                default:
                case ExpeditionCardType.UltimateEasy:
                case ExpeditionCardType.UltimateMedium:
                case ExpeditionCardType.UltimateHard:
                case ExpeditionCardType.UltimateHardcore:
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
                case ExpeditionCardType.UltimateMedium:
                case ExpeditionCardType.UltimateHard:
                case ExpeditionCardType.UltimateHardcore:
                    return $"niezwykł{end}";

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
