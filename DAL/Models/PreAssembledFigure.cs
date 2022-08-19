namespace Sanakan.DAL.Models
{
    public enum PreAssembledFigure : byte
    {
        None = 0,
        Megumin = 1,
        Asuna = 2,
        Gintoki = 3
    }

    public static class PreAssembledFigureExtensions
    {
        public static ulong GetCharacterId(this PreAssembledFigure pas)
        {
            switch (pas)
            {
                case PreAssembledFigure.Asuna: return 45276;
                case PreAssembledFigure.Gintoki: return 663;
                case PreAssembledFigure.Megumin: return 72013;

                default:
                    return 0;
            }
        }

        public static string GetCharacterName(this PreAssembledFigure pas)
        {
            switch (pas)
            {
                case PreAssembledFigure.Asuna: return "Asuna Yuuki";
                case PreAssembledFigure.Gintoki: return "Gintoki Sakata";
                case PreAssembledFigure.Megumin: return "Megumin";

                default:
                    return "";
            }
        }

        public static string GetTitleName(this PreAssembledFigure pas)
        {
            switch (pas)
            {
                case PreAssembledFigure.Asuna: return "Sword Art Online";
                case PreAssembledFigure.Gintoki: return "Gintama";
                case PreAssembledFigure.Megumin: return "Kono Subarashii Sekai ni Shukufuku wo!";

                default:
                    return "";
            }
        }
    }
}
