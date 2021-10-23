using System.Collections.Generic;

namespace Sanakan.Common
{
    public interface IRandomNumberGenerator
    {
        IEnumerable<T> Shuffle<T>(IEnumerable<T> list);
        int GetRandomValue(int max);
        int GetRandomValue(int min, int max);
        T GetOneRandomFrom<T>(IEnumerable<T> enumerable);
        bool TakeATry(int chance);
    }
}
