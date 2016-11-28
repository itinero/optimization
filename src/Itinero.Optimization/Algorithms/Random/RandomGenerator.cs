// Itinero.Optimization - Route optimization for .NET
// Copyright (C) 2016 Abelshausen Ben
// 
// This file is part of Itinero.
// 
// Itinero is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// Itinero is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Itinero. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;

namespace Itinero.Optimization.Algorithms.Random
{
    /// <summary>
    /// A random generator.
    /// </summary>
    public sealed class RandomGenerator
    {
        private System.Random _rand;

        /// <summary>
        /// Creates a new random generator.
        /// </summary>
        public RandomGenerator()
        {
            _rand = new System.Random();
        }

        /// <summary>
        /// Creates a new random generator.
        /// </summary>
        public RandomGenerator(int seed)
        {
            _rand = new System.Random(seed);
        }

        /// <summary>
        /// Generates a random integer
        /// </summary>
        public void Generate(byte[] buffer)
        {
            _rand.NextBytes(buffer);
        }

        /// <summary>
        /// Generates a random double
        /// </summary>
        public float Generate(float max)
        {
            return (float)(_rand.NextDouble() * max);
        }

        /// <summary>
        /// Sets the seed of the static generator.
        /// </summary>
        public int Generate(int max)
        {
            return _rand.Next(max);
        }
    }

    /// <summary>
    /// Random generator extensions.
    /// </summary>
    public static class RandomGeneratorExtensions
    {
        private static RandomGenerator _random;

        /// <summary>
        /// Resets the current random generator.
        /// </summary>
        /// <returns></returns>

        public static void Reset()
        {
            _random = null;
        }

        /// <summary>
        /// Gets a new random generator.
        /// </summary>
        public static RandomGenerator GetRandom()
        {
            if (_random == null)
            {
                if (RandomGeneratorExtensions.GetGetNewRandom != null)
                {
                    _random = GetGetNewRandom();
                }
                else
                {
                    _random = new RandomGenerator();
                }
            }
            return _random;
        }

        /// <summary>
        /// Holds a custom random genetor getter.
        /// </summary>
        public static Func<RandomGenerator> GetGetNewRandom
        {
            get;
            set;
        }

        /// <summary>
        /// Shuffles the list using Fisher-Yates shuffle.
        /// </summary>
        public static void Shuffle<T>(this IList<T> list)
        {
            list.Shuffle(RandomGeneratorExtensions.GetRandom());
        }

        /// <summary>
        /// Shuffles the list using Fisher-Yates shuffle.
        /// </summary>
        public static void Shuffle<T>(this IList<T> list, RandomGenerator random)
        {
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = random.Generate(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}