using System.Collections.Generic;

namespace Sanakan.Common
{
    public interface IRandomNumberGenerator
    {
        IEnumerable<T> Shuffle<T>(IEnumerable<T> list);

        /// <summary>
        /// Returns a random integer between 0 and max.
        /// </summary>
        /// <param name="max">The maximum value.</param>
        /// <returns></returns>
        int GetRandomValue(int max);

        /// <summary>
        /// Returns a random integer between min and max.
        /// </summary>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <returns></returns>
        int GetRandomValue(int min, int max);

        /// <summary>
        /// Returns a random item from collection.
        /// </summary>
        /// <param name="enumerable">The collection.</param>
        /// <typeparam name="T">the collection type.</typeparam>
        /// <returns></returns>
        T GetOneRandomFrom<T>(IEnumerable<T> enumerable);

        /// <summary>
        /// Returns random boolean.
        /// </summary>
        /// <param name="chance">a value.</param>
        /// <returns></returns>
        bool TakeATry(int chance);
    }
}
