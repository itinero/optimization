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

namespace Itinero.Optimization.Abstract.Tours
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
            while (enumerator.MoveNext())
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
        public static IEnumerable<int[]> Seq(this IEnumerable<int> sequence, int size, bool isClosed = false, bool loopAround = true)
        {
            return new Sequences.SequenceEnumerable(sequence, isClosed, size, loopAround);
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

        /// <summary>
        /// Enumerates all sequences in the given tour with the given size and smaller.
        /// </summary>
        public static IEnumerable<int[]> SeqAndSmaller(this IEnumerable<int> tour, int size, bool isClosed = false)
        {
            return tour.SeqAndSmaller(2, size, isClosed);
        }

        /// <summary>
        /// Enumerates all sequences in the given tour with the given size and smaller.
        /// </summary>
        public static IEnumerable<int[]> SeqAndSmaller(this IEnumerable<int> tour, int minSize, int maxSize, bool isClosed = false, bool loopAround = true)
        {
            var enumerable = System.Linq.Enumerable.Empty<int[]>();
            for (var i = minSize; i <= maxSize; i++)
            {
                enumerable = enumerable.Concat(tour.Seq(i, isClosed, loopAround));
            }
            return enumerable;
        }

        /// <summary>
        /// Returns an enumerable that also includes the reverse sequences.
        /// </summary>
        /// <param name="sequences">The original sequences.</param>
        /// <param name="offsetStart">The start offset, everything before this index is kept as original.</param>
        /// <param name="offsetEnd">The end offset, everything after this index is kept as original.</param>
        /// <returns></returns>
        public static IEnumerable<int[]> AddSeqReversed(this IEnumerable<int[]> sequences, int offsetStart = 1, int offsetEnd = 1)
        {
            return sequences.Concat(sequences.SeqReversed(offsetStart, offsetStart));
        }

        /// <summary>
        /// Returns an enumerable that includes the reverse sequences.
        /// </summary>
        /// <param name="sequences">The original sequences.</param>
        /// <param name="offsetStart">The start offset, everything before this index is kept as original.</param>
        /// <param name="offsetEnd">The end offset, everything after this index is kept as original.</param>
        /// <returns></returns>
        public static IEnumerable<int[]> SeqReversed(this IEnumerable<int[]> sequences, int offsetStart = 1, int offsetEnd = 1)
        {
            var totalOffset = offsetStart + offsetEnd;
            foreach (var seq in sequences)
            {
                if (seq.Length <= totalOffset + 1)
                { // can't reverse single visits or sequence smaller than the offsets.
                    yield return seq;
                }
                var newSeq = seq.Clone() as int[]; // clone the sequence, important!
                for (var i = offsetStart; i < seq.Length - offsetEnd; i++)
                {
                    var j = seq.Length - i - 1;
                    newSeq[j] = seq[i];
                }
                yield return newSeq;
            }
        }

        /// <summary>
        /// Reverses a part of the given sequence.
        /// </summary>
        /// <param name="seq">The sequence.</param>
        /// <param name="offsetStart">The start offset.</param>
        /// <param name="offsetEnd">The end offset.</param>
        public static void ReverseRange<T>(this T[] seq, int offsetStart = 1, int offsetEnd = 1)
        {
            if (seq.Length <= offsetEnd + offsetStart + 1)
            { // can't reverse single visits or sequence smaller than the offsets.
                return;
            }
            for (var i = offsetStart; i < (seq.Length - offsetEnd) / 2 + 1; i++)
            {
                var j = seq.Length - i - 1;
                var jVal = seq[j];
                seq[j] = seq[i];
                seq[i] = jVal;
            }
        }

        /// <summary>
        /// Verifies the given tour.
        /// </summary>
        public static void Verify(this ITour tour, int count)
        {
#if DEBUG
            if (!tour.TryVerify(count))
            {
                throw new System.Exception("Tour probably invalid: enumerates more visits then it's supposed to.");
            }
#endif
        }

        /// <summary>
        /// Tries to verify the given tour.
        /// </summary>
        public static bool TryVerify(this ITour tour, int count)
        {
#if DEBUG
            var enumerator = tour.GetEnumerator();
            var enumCount = 0;
            while (enumerator.MoveNext())
            {
                enumCount++;
                if (enumCount > count)
                {
                return false;
                }
            }
#endif
            return true;
        }
    }
}