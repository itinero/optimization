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

using System.Collections.Generic;
using System.Linq;

namespace Itinero.Optimization.Tours
{
    /// <summary>
    /// Contains tour extensions.
    /// </summary>
    public static class TourExtensions
    {
        /// <summary>
        /// Puts the elements of the enumerator (back) in a list.
        /// </summary>
        /// <returns></returns>
        public static List<T> ToList<T>(this IEnumerator<T> enumerator)
        {
            var list = new List<T>();
            while(enumerator.MoveNext())
            {
                list.Add(enumerator.Current);
            }
            return list;
        }

        /// <summary>
        /// Returns true if the given route is closed.
        /// </summary>
        public static bool IsClosed(this ITour tour)
        {
            return tour.Last.HasValue &&
                tour.Last.Value == tour.First;
        }

        /// <summary>
        /// Generates pairs based on the given IEnumerable representing a tour.
        /// </summary>
        public static IEnumerable<Pair> Pairs(this IEnumerable<int> tour, bool isClosed)
        {
            return new PairEnumerable<IEnumerable<int>>(tour, isClosed);
        }

        /// <summary>
        /// Enumerates all sequences in the given sequence with the given size.
        /// </summary>
        public static IEnumerable<int[]> Seq(this IEnumerable<int> sequence, int size, bool isClosed = false)
        {
            return new Sequences.SequenceEnumerable(sequence, isClosed, size);
        }

        /// <summary>
        /// Enumerates all sequences in the given tour with the given size.
        /// </summary>
        public static IEnumerable<int[]> Seq(this ITour tour, int size)
        {
            return new Sequences.SequenceEnumerable(tour, size);
        }

        /// <summary>
        /// Enumerates all sequences in the given tour with the given size and smaller.
        /// </summary>
        public static IEnumerable<int[]> SeqAndSmaller(this ITour tour, int size)
        {
            return tour.SeqAndSmaller(2, size);
        }

        /// <summary>
        /// Enumerates all sequences in the given tour with the given size and smaller.
        /// </summary>
        public static IEnumerable<int[]> SeqAndSmaller(this ITour tour, int minSize, int maxSize)
        {
            var enumerable = System.Linq.Enumerable.Empty<int[]>();
            for (var i = minSize; i <= maxSize; i++)
            {
                enumerable = enumerable.Concat(tour.Seq(i));
            }
            return enumerable;
        }
    }
}