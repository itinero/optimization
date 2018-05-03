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

namespace Itinero.Optimization.Strategies.Random
{
   /// <summary>
    /// Random generator extensions.
    /// </summary>
    public static class RandomGeneratorExtensions
    {

        /// <summary>
        /// Shuffles the list using Fisher-Yates shuffle.
        /// </summary>
        public static void Shuffle<T>(this IList<T> list)
        {
            list.Shuffle(RandomGenerator.Default);
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
        /// Generates 2 distinct random int's in the given range.
        /// </summary>
        /// <param name="start">The start of the range.</param>
        /// <param name="count">The # of elements.</param>
        /// <param name="t1">The first random result.</param>
        /// <param name="t2">The second random result.</param>
        /// <returns>False if count is smaller <![CDATA[< 2]]></returns>
        public static bool Random2Between(int start, int count, out int t1, out int t2)
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

            if (start != 0)
            {
                t1 += count;
                t2 += count;
            }

            return true;
        }
    }
}