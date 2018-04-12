/*
 *  Licensed to SharpSoftware under one or more contributor
 *  license agreements. See the NOTICE file distributed with this work for 
 *  additional information regarding copyright ownership.
 * 
 *  SharpSoftware licenses this file to you under the Apache License, 
 *  Version 2.0 (the "License"); you may not use this file except in 
 *  compliance with the License. You may obtain a copy of the License at
 * 
 *       http://www.apache.org/licenses/LICENSE-2.0
 * 
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */

using System;
using System.Collections.Generic;

namespace Itinero.Optimization.Algorithms.Random
{
    /// <summary>
    /// A random generator.
    /// </summary>
    public class RandomGenerator
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
        public virtual void Generate(byte[] buffer)
        {
            _rand.NextBytes(buffer);
        }

        /// <summary>
        /// Generates a random double
        /// </summary>
        public virtual float Generate(float max)
        {
            return (float)(_rand.NextDouble() * max);
        }

        /// <summary>
        /// Sets the seed of the static generator.
        /// </summary>
        public virtual int Generate(int max)
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

        /// <summary>
        /// Selects two random routes from the solution.
        /// Returns false if no routes can be selected (because the solution has only one route)
        /// </summary>
        public static bool RandomRoutes(int solutionCount, out int t1, out int t2){
            if(solutionCount < 2){
                t1 = 0;
                t2 = 0;
                return false;
            }

            var random = RandomGeneratorExtensions.GetRandom();
            t1 = random.Generate(solutionCount);
            t2 = random.Generate(solutionCount - 1);
            if (t2 >= t1)
            {
            // Move all tourIDs away, no overlap possible while still maintaining an even chance distribution
                t2++;
            }

            return true;

        }
    }
}