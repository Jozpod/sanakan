using System.Collections.Generic;

namespace Sanakan.Common
{
    public interface IRandomNumberGenerator
    {
        IEnumerable<T> Shuffle<T>(IEnumerable<T> list);

        /// <summary>
        /// Returns a random integer between 0 and max.
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        int GetRandomValue(int max);

        /// <summary>
        /// Returns a random integer between min and max.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        int GetRandomValue(int min, int max);

        /// <summary>
        /// Returns a random item from collection.
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        T GetOneRandomFrom<T>(IEnumerable<T> enumerable);

        /// <summary>
        /// Returns a random boolean.
        /// </summary>
        /// <param name="chance"></param>
        /// <returns></returns>
        bool TakeATry(int chance);
    }
}
