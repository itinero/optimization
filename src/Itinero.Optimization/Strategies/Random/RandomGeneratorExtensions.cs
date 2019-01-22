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
using System.Diagnostics;

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
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        /// <summary>
        /// Selects random elements from the given collection and writes them to result.
        /// </summary>
        /// <param name="random">The random generator.</param>
        /// <param name="collection">The collection to select from.</param>
        /// <param name="result">The result.</param>
        public static void SelectRandomFrom(this RandomGenerator random, ICollection<int> collection, ref int[] result)
        {
            Debug.Assert(collection != null, $"{collection} cannot be null.");
            Debug.Assert(result != null, $"{result} cannot be null.");
            
            if (result.Length > collection.Count) throw new InvalidOperationException($"Cannot select {result.Length} random elements from a collection with size {collection.Count}");

            if (result.Length == collection.Count)
            {
                collection.CopyTo(result, 0);
                result.Shuffle();
                return;
            }

            if (result.Length * 2 > collection.Count)
            {
                using (var enumerator = collection.GetEnumerator())
                {
                    var j = 0;
                    while (j < result.Length)
                    {
                        enumerator.MoveNext();
                        result[j] = enumerator.Current;
                        j++;
                    }
                }
                result.Shuffle();
                return;
            }
            
            // this is only done from result sets < half of the collection size.
            // TODO: figure out a faster way to do this, without creating a new hashset every time, this can be performance critical.
            var selected = new HashSet<int>();
            var i = 0;
            while (i < result.Length)
            {
                var s = random.Generate(collection.Count);
                if (selected.Contains(s)) continue;

                selected.Add(s);
                result[i] = s;
                i++;
            }
        }
    }
}