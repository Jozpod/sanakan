using System;
using System.Security.Cryptography;

namespace Sanakan.Common
{
    public class RandomNumberGenerator : IRandomNumberGenerator
    {
        private readonly System.Security.Cryptography.RandomNumberGenerator _randomNumberGenerator;

        public RandomNumberGenerator()
        {
            _randomNumberGenerator = new RNGCryptoServiceProvider();
        }

        public int GetRandomValue(int max) => GetRandomValue(0, max);

        public int GetRandomValue(int min, int max)
        {
            uint scale = uint.MaxValue;

            while (scale == uint.MaxValue)
            {
                byte[] bytes = new byte[4];
                _randomNumberGenerator.GetBytes(bytes);

                scale = BitConverter.ToUInt32(bytes, 0);
            }

            return (int)(min + ((max - min) * (scale / (double)uint.MaxValue)));
        }
    }
}
