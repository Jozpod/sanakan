using Shinden.Models.Initializers;

namespace Shinden.Models.Entities
{
    public class CharacterFavs : ICharacterFavs
    {
        public CharacterFavs(InitCharacterFavs Init)
        {
            FavCnt = Init.FavCnt;
            AvgPos = Init.AvgPos;
            UnFavCnt = Init.UnFavCnt;
            FirstPosCnt = Init.FirstPosCnt;
            Under3PosCnt = Init.Under3PosCnt;
            Under10PosCnt = Init.Under10PosCnt;
            Under50PosCnt = Init.Under50PosCnt;
        }

        // ICharacterFavs
        public long FavCnt { get; }
        public long UnFavCnt { get; }
        public double AvgPos { get; }
        public long FirstPosCnt { get; }
        public long Under3PosCnt { get; }
        public long Under10PosCnt { get; }
        public long Under50PosCnt { get; }

        public override string ToString() => $"{FavCnt}";
    }
}
