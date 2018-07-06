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
using System.Threading;

namespace Itinero.Optimization.Strategies.Random
{
    /// <summary>
    /// A random generator.
    /// </summary>
    public abstract class RandomGenerator
    {
        /// <summary>
        /// Generates a random integer
        /// </summary>
        public abstract void Generate(byte[] buffer);

        /// <summary>
        /// Generates a random double
        /// </summary>
        public abstract float Generate(float max);

        /// <summary>
        /// Sets the seed of the static generator.
        /// </summary>
        public abstract int Generate(int max);

        private static Lazy<RandomGenerator> DefaultRandomGenerator = new Lazy<RandomGenerator>(() =>
        {
            if (RandomGenerator.GetGetNewRandom != null)
            {
                return GetGetNewRandom();
            }
            else
            {
                return new DefaultRandomGenerator();
            }
        });

        /// <summary>
        /// Resets the default random generator.
        /// </summary>
        public static void ResetDefault()
        {
            if (DefaultRandomGenerator.IsValueCreated)
            { 
                DefaultRandomGenerator = new Lazy<RandomGenerator>(() =>
                {
                    if (RandomGenerator.GetGetNewRandom != null)
                    {
                        return GetGetNewRandom();
                    }
                    else
                    {
                        return new DefaultRandomGenerator();
                    }
                });
            }
        }

        /// <summary>
        /// Gets a new random generator.
        /// </summary>
        public static RandomGenerator Default => DefaultRandomGenerator.Value;

        /// <summary>
        /// A custom random generator creation function to inject custom random generators.
        /// </summary>
        public static Func<RandomGenerator> GetGetNewRandom
        {
            get;
            set;
        }

        /// <summary>
        /// Generates a second random int distinct from the first.
        /// </summary>
        /// <param name="max">The # of elements.</param>
        /// <param name="first">The existing integer.</param>
        /// <param name="second">The second distinct integer.</param>
        /// <returns>False if max is smaller <![CDATA[< 2]]></returns>
        public static bool GenerateSecond(int max, int first, out int second)
        {
            if (max < 2)
            {
                second = 0;
                return false;
            }
            
            var random = RandomGenerator.Default;
            second = random.Generate(max - 1);
            if (second >= first)
            {
                second++;
            }

            return true;
        }

        /// <summary>
        /// Generates 2 random int's.
        /// </summary>
        /// <param name="max">The # of elements.</param>
        /// <param name="t1">The first random result.</param>
        /// <param name="t2">The second random result.</param>
        /// <returns>False if max is smaller <![CDATA[< 2]]></returns>
        public static bool Generate2(int max, out int t1, out int t2)
        {
            return Generate2Between(0, max, out t1, out t2);
        }

        /// <summary>
        /// Generates 2 distinct random int's in the given range.
        /// </summary>
        /// <param name="start">The start of the range.</param>
        /// <param name="count">The # of elements.</param>
        /// <param name="t1">The first random result.</param>
        /// <param name="t2">The second random result.</param>
        /// <returns>False if count is smaller <![CDATA[< 2]]></returns>
        public static bool Generate2Between(int start, int count, out int t1, out int t2)
        {
            if(count < 2)
            {
                t1 = 0;
                t2 = 0;
                return false;
            }

            var random = RandomGenerator.Default;
            t1 = random.Generate(count);
            t2 = random.Generate(count - 1);
            if (t2 >= t1)
            {
                t2++;
            }

            if (start == 0) return true;
            
            t1 += count;
            t2 += count;

            return true;
        }
    }
}