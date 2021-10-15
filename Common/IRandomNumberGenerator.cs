namespace Sanakan.Common
{
    public interface IRandomNumberGenerator
    {
        int GetRandomValue(int max);
        int GetRandomValue(int min, int max);
    }
}
