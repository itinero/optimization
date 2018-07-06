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

using System.Linq;
using Itinero.Optimization.Solvers.Tours.Sequences;
using Xunit;

namespace Itinero.Optimization.Tests.Solvers.Tours.Sequences
{
    public class SequenceEnumerableTests
    {
        /// <summary>
        /// The sequence enumerable should just enumerate sequences of size 1 when enumerating.
        /// </summary>
        [Fact]
        public void SequenceEnumerable_ShouldJustEnumerateVisitsOnOpenWhenSizeOne()
        {
            var tourEnumerable = new int[] { 0, 1, 2, 3, 4 };
            var result = new SequenceEnumerable(tourEnumerable, false, 1).ToList();

            Assert.NotNull(result);
            Assert.Equal(5, result.Count);
            Assert.NotNull(result[0]);
            Assert.NotNull(result[1]);
            Assert.NotNull(result[2]);
            Assert.NotNull(result[3]);
            Assert.NotNull(result[4]);
            Assert.Equal(0, result[0][0]);
            Assert.Equal(1, result[1][0]);
            Assert.Equal(2, result[2][0]);
            Assert.Equal(3, result[3][0]);
            Assert.Equal(4, result[4][0]);
        }
        
        /// <summary>
        /// The sequence enumerable should just enumerate sequences of size 1 when enumerating.
        /// </summary>
        [Fact]
        public void SequenceEnumerable_ShouldJustEnumerateVisitsOnClosedWhenSizeOne()
        {
            var tourEnumerable = new int[] { 0, 1, 2, 3, 4 };
            var result = new SequenceEnumerable(tourEnumerable, true, 1).ToList();

            Assert.NotNull(result);
            Assert.Equal(5, result.Count);
            Assert.NotNull(result[0]);
            Assert.NotNull(result[1]);
            Assert.NotNull(result[2]);
            Assert.NotNull(result[3]);
            Assert.NotNull(result[4]);
            Assert.Equal(0, result[0][0]);
            Assert.Equal(1, result[1][0]);
            Assert.Equal(2, result[2][0]);
            Assert.Equal(3, result[3][0]);
            Assert.Equal(4, result[4][0]);
        }

        /// <summary>
        /// The sequence enumerable should enumerate pairs when size is 2.
        /// </summary>
        [Fact]
        public void SequenceEnumerable_ShouldJustEnumeratePairsOnOpenWhenSizeTwo()
        {
            var tourEnumerable = new int[] { 0, 1, 2, 3, 4 };

            var result = new SequenceEnumerable(tourEnumerable, false, 2).ToList();

            Assert.NotNull(result);
            Assert.Equal(4, result.Count);
            Assert.NotNull(result[0]);
            Assert.NotNull(result[1]);
            Assert.NotNull(result[2]);
            Assert.NotNull(result[3]);
            Assert.Equal(0, result[0][0]);
            Assert.Equal(1, result[0][1]);
            Assert.Equal(1, result[1][0]);
            Assert.Equal(2, result[1][1]);
            Assert.Equal(2, result[2][0]);
            Assert.Equal(3, result[2][1]);
            Assert.Equal(3, result[3][0]);
            Assert.Equal(4, result[3][1]);
        }

        /// <summary>
        /// The sequence enumerable should enumerate pairs when size is 2.
        /// </summary>
        [Fact]
        public void SequenceEnumerable_ShouldJustEnumeratePairsOnClosedWhenSizeTwo()
        {
            var tourEnumerable = new int[] { 0, 1, 2, 3, 4 };

            var result = new SequenceEnumerable(tourEnumerable, true, 2).ToList();

            Assert.NotNull(result);
            Assert.Equal(5, result.Count);
            Assert.NotNull(result[0]);
            Assert.NotNull(result[1]);
            Assert.NotNull(result[2]);
            Assert.NotNull(result[3]);
            Assert.Equal(0, result[0][0]);
            Assert.Equal(1, result[0][1]);
            Assert.Equal(1, result[1][0]);
            Assert.Equal(2, result[1][1]);
            Assert.Equal(2, result[2][0]);
            Assert.Equal(3, result[2][1]);
            Assert.Equal(3, result[3][0]);
            Assert.Equal(4, result[3][1]);
            Assert.Equal(4, result[4][0]);
            Assert.Equal(0, result[4][1]);
        }

        /// <summary>
        /// The sequence enumerable should enumerate all sequences of size 4 when size is 4.
        /// </summary>
        [Fact]
        public void SequenceEnumerable_ShouldJustEnumerateOnOpenWhenSizeFour()
        {
            var tourEnumerable = new int[] { 0, 1, 2, 3, 4 };

            var result = new SequenceEnumerable(tourEnumerable, false, 4).ToList();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.NotNull(result[0]);
            Assert.NotNull(result[1]);
            Assert.Equal(0, result[0][0]);
            Assert.Equal(1, result[0][1]);
            Assert.Equal(2, result[0][2]);
            Assert.Equal(3, result[0][3]);
            Assert.Equal(1, result[1][0]);
            Assert.Equal(2, result[1][1]);
            Assert.Equal(3, result[1][2]);
            Assert.Equal(4, result[1][3]);
        }

        /// <summary>
        /// The sequence enumerable should enumerate all sequences of size 4 when size is 4.
        /// </summary>
        [Fact]
        public void SequenceEnumerable_ShouldJustEnumerateOnClosedWhenSizeFour()
        {
            var tourEnumerable = new int[] { 0, 1, 2, 3, 4 };

            var result = new SequenceEnumerable(tourEnumerable, true, 4).ToList();

            Assert.NotNull(result);
            Assert.Equal(5, result.Count);
            Assert.NotNull(result[0]);
            Assert.NotNull(result[1]);
            Assert.NotNull(result[2]);
            Assert.NotNull(result[3]);
            Assert.NotNull(result[4]);
            Assert.Equal(0, result[0][0]);
            Assert.Equal(1, result[0][1]);
            Assert.Equal(2, result[0][2]);
            Assert.Equal(3, result[0][3]);

            Assert.Equal(1, result[1][0]);
            Assert.Equal(2, result[1][1]);
            Assert.Equal(3, result[1][2]);
            Assert.Equal(4, result[1][3]);

            Assert.Equal(2, result[2][0]);
            Assert.Equal(3, result[2][1]);
            Assert.Equal(4, result[2][2]);
            Assert.Equal(0, result[2][3]);

            Assert.Equal(3, result[3][0]);
            Assert.Equal(4, result[3][1]);
            Assert.Equal(0, result[3][2]);
            Assert.Equal(1, result[3][3]);

            Assert.Equal(4, result[4][0]);
            Assert.Equal(0, result[4][1]);
            Assert.Equal(1, result[4][2]);
            Assert.Equal(2, result[4][3]);
        }

        /// <summary>
        /// Tests enumerating sequences of size the same as the tour.
        /// </summary>
        [Fact]
        public void SequenceEnumerable_ShouldJustEnumerateItselfOnOpenWhenSizeIdentical()
        {
            var tourEnumerable = new int[] { 0, 1, 2, 3, 4 };

            var result = new SequenceEnumerable(tourEnumerable, false, 5).ToList();

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.NotNull(result[0]);
            Assert.Equal(0, result[0][0]);
            Assert.Equal(1, result[0][1]);
            Assert.Equal(2, result[0][2]);
            Assert.Equal(3, result[0][3]);
            Assert.Equal(4, result[0][4]);
        }

        /// <summary>
        /// Tests enumerating sequences of size the same as the tour.
        /// </summary>
        [Fact]
        public void SequenceEnumerable_ShouldJustEnumerateAllOnClosedWhenSizeIdentical()
        {
            var tourEnumerable = new int[] { 0, 1, 2, 3, 4 };

            var result = new SequenceEnumerable(tourEnumerable, true, 5).ToList();

            Assert.NotNull(result);
            Assert.Equal(5, result.Count);
            Assert.NotNull(result[0]);
            Assert.NotNull(result[1]);
            Assert.NotNull(result[2]);
            Assert.NotNull(result[3]);
            Assert.NotNull(result[4]);
            Assert.Equal(0, result[0][0]);
            Assert.Equal(1, result[0][1]);
            Assert.Equal(2, result[0][2]);
            Assert.Equal(3, result[0][3]);
            Assert.Equal(4, result[0][4]);

            Assert.Equal(1, result[1][0]);
            Assert.Equal(2, result[1][1]);
            Assert.Equal(3, result[1][2]);
            Assert.Equal(4, result[1][3]);
            Assert.Equal(0, result[1][4]);

            Assert.Equal(2, result[2][0]);
            Assert.Equal(3, result[2][1]);
            Assert.Equal(4, result[2][2]);
            Assert.Equal(0, result[2][3]);
            Assert.Equal(1, result[2][4]);

            Assert.Equal(3, result[3][0]);
            Assert.Equal(4, result[3][1]);
            Assert.Equal(0, result[3][2]);
            Assert.Equal(1, result[3][3]);
            Assert.Equal(2, result[3][4]);

            Assert.Equal(4, result[4][0]);
            Assert.Equal(0, result[4][1]);
            Assert.Equal(1, result[4][2]);
            Assert.Equal(2, result[4][3]);
            Assert.Equal(3, result[4][4]);
        }
    }
}