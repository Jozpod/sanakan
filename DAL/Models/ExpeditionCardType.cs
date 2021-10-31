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
