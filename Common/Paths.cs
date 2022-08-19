namespace Sanakan.Common
{
    public static class Paths
    {
        public const string ShieldPicture = "./Pictures/PW/shield.png";
        public const string HeartPicture = "./Pictures/PW/heart.png";
        public const string FirePicture = "./Pictures/PW/fire.png";
        public const string StatsAnimePicture = "./Pictures/statsAnime.png";
        public const string StatsMangaPicture = "./Pictures/statsManga.png";
        public const string DefaultBackgroundPicture = "./Pictures/defBg.png";
        public const string ProfileBodyPicture = "./Pictures/profileBody.png";
        public const string SiteStatisticsPicture = "./Pictures/siteStatsBody.png";

        public const string NegativeStatsPicture = "./Pictures/PW/neg.png";
        public const string BackBorderPicture = "./Pictures/PW/CG/{0}/BorderBack.png";
        public const string PWEmptyPicture = "./Pictures/PW/empty.png";
        public const string PWPicture = "./Pictures/PW/{0}.png";
        public const string PWCGPicture = "./Pictures/PW/CG/{0}/Border.png";
        public const string PWCGFolderFile = "./Pictures/PW/CG/{0}/{1}.png";
        public const string PWCGDerePicture = "./Pictures/PW/CG/{0}/Dere/{1}.png";
        public const string PWCGStatsPicture = "./Pictures/PW/CG/{0}/Stats.png";
        public const string PWStarPicture = "./Pictures/PW/stars/{0}_{1}.png";

        public const string CoinPicture = "./Pictures/coin{0}.png";
        public const string PokePicture = "./Pictures/Poke/{0}.jpg";
        public const string DefaultPokePicture = "./Pictures/PW/poke.jpg";
        public const string DefaultPokeaPicture = "./Pictures/PW/pokea.jpg";

        public const string DuelPicture = "./Pictures/Duel/{0}{1}.jpg";
        public const string PWDuelPicture = "./Pictures/PW/duel{0}.jpg";
        private const string BaseOutput = "../GOut";

        public static string Cards = $"{BaseOutput}/Cards";
        public static string CardsMiniatures = $"{Cards}/Small";
        public static string CardsInProfiles = $"{Cards}/Profile";
        public static string SavedData = $"{BaseOutput}/Saved";
        public static string Profiles = $"{BaseOutput}/Profile";
        public static string PokeList = "./Pictures/Poke/List.json";
    }
}