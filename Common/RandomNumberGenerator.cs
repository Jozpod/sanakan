﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace Sanakan.Common
{
    internal class RandomNumberGenerator : IRandomNumberGenerator
    {
        private readonly System.Security.Cryptography.RandomNumberGenerator _randomNumberGenerator;

        public RandomNumberGenerator()
        {
            _randomNumberGenerator = System.Security.Cryptography.RandomNumberGenerator.Create();
        }

        public IEnumerable<T> Shuffle<T>(IEnumerable<T> list)
        {
            var buffer = list.ToList();
            for (var i = 0; i < buffer.Count; i++)
            {
                var j = GetRandomValue(i, buffer.Count);
                yield return buffer[j];

                buffer[j] = buffer[i];
            }
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

        public T GetOneRandomFrom<T>(IEnumerable<T> enumerable)
        {
            var randomIndex = GetRandomValue(enumerable.Count());
            return enumerable.ElementAt(randomIndex);
        }

        public bool TakeATry(int chance) => (GetRandomValue(chance * 100) % chance) == 1;
    }
}