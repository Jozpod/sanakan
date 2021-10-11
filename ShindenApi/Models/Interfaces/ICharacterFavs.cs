namespace Shinden.Models
{
    public interface ICharacterFavs
    {
        long FavCnt { get; }
        long UnFavCnt { get; }
        double AvgPos { get; }
        long FirstPosCnt { get; }
        long Under3PosCnt { get; }
        long Under10PosCnt { get; }
        long Under50PosCnt { get; }
    }
}