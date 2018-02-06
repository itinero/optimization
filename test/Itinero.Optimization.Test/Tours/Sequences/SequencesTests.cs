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
using Itinero.Optimization.Tours.Sequences;
using NUnit.Framework;

namespace Itinero.Optimization.Test.Tours.Sequences
{
    /// <summary>
    /// Contains tests for the sequence enumerators.
    /// </summary>
    [TestFixture]
    public class SequencesTests
    {
        /// <summary>
        /// Tests enumerating sequences of size 1.
        /// </summary>
        [Test]
        public void TestSequencesSize1Open()
        {
            var tourEnumerable = new int[] { 0, 1, 2, 3, 4 };

            var result = new SequenceEnumerable(tourEnumerable, false, 1).ToList();

            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.Count);
            Assert.IsNotNull(result[0]);
            Assert.IsNotNull(result[1]);
            Assert.IsNotNull(result[2]);
            Assert.IsNotNull(result[3]);
            Assert.IsNotNull(result[4]);
            Assert.AreEqual(0, result[0][0]);
            Assert.AreEqual(1, result[1][0]);
            Assert.AreEqual(2, result[2][0]);
            Assert.AreEqual(3, result[3][0]);
            Assert.AreEqual(4, result[4][0]);
        }

        /// <summary>
        /// Tests enumerating sequences of size 1.
        /// </summary>
        [Test]
        public void TestSequencesSize1Closed()
        {
            var tourEnumerable = new int[] { 0, 1, 2, 3, 4 };

            var result = new SequenceEnumerable(tourEnumerable, true, 1).ToList();

            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.Count);
            Assert.IsNotNull(result[0]);
            Assert.IsNotNull(result[1]);
            Assert.IsNotNull(result[2]);
            Assert.IsNotNull(result[3]);
            Assert.IsNotNull(result[4]);
            Assert.AreEqual(0, result[0][0]);
            Assert.AreEqual(1, result[1][0]);
            Assert.AreEqual(2, result[2][0]);
            Assert.AreEqual(3, result[3][0]);
            Assert.AreEqual(4, result[4][0]);
        }

        /// <summary>
        /// Tests enumerating sequences of size 2.
        /// </summary>
        [Test]
        public void TestSequencesSize2Open()
        {
            var tourEnumerable = new int[] { 0, 1, 2, 3, 4 };

            var result = new SequenceEnumerable(tourEnumerable, false, 2).ToList();

            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.Count);
            Assert.IsNotNull(result[0]);
            Assert.IsNotNull(result[1]);
            Assert.IsNotNull(result[2]);
            Assert.IsNotNull(result[3]);
            Assert.AreEqual(0, result[0][0]);
            Assert.AreEqual(1, result[0][1]);
            Assert.AreEqual(1, result[1][0]);
            Assert.AreEqual(2, result[1][1]);
            Assert.AreEqual(2, result[2][0]);
            Assert.AreEqual(3, result[2][1]);
            Assert.AreEqual(3, result[3][0]);
            Assert.AreEqual(4, result[3][1]);
        }

        /// <summary>
        /// Tests enumerating sequences of size 2.
        /// </summary>
        [Test]
        public void TestSequencesSize2Closed()
        {
            var tourEnumerable = new int[] { 0, 1, 2, 3, 4 };

            var result = new SequenceEnumerable(tourEnumerable, true, 2).ToList();

            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.Count);
            Assert.IsNotNull(result[0]);
            Assert.IsNotNull(result[1]);
            Assert.IsNotNull(result[2]);
            Assert.IsNotNull(result[3]);
            Assert.IsNotNull(result[4]);
            Assert.AreEqual(0, result[0][0]);
            Assert.AreEqual(1, result[0][1]);
            Assert.AreEqual(1, result[1][0]);
            Assert.AreEqual(2, result[1][1]);
            Assert.AreEqual(2, result[2][0]);
            Assert.AreEqual(3, result[2][1]);
            Assert.AreEqual(3, result[3][0]);
            Assert.AreEqual(4, result[3][1]);
            Assert.AreEqual(4, result[4][0]);
            Assert.AreEqual(0, result[4][1]);
        }

        /// <summary>
        /// Tests enumerating sequences of size 4.
        /// </summary>
        [Test]
        public void TestSequencesSize4Open()
        {
            var tourEnumerable = new int[] { 0, 1, 2, 3, 4 };

            var result = new SequenceEnumerable(tourEnumerable, false, 4).ToList();

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.IsNotNull(result[0]);
            Assert.IsNotNull(result[1]);
            Assert.AreEqual(0, result[0][0]);
            Assert.AreEqual(1, result[0][1]);
            Assert.AreEqual(2, result[0][2]);
            Assert.AreEqual(3, result[0][3]);
            Assert.AreEqual(1, result[1][0]);
            Assert.AreEqual(2, result[1][1]);
            Assert.AreEqual(3, result[1][2]);
            Assert.AreEqual(4, result[1][3]);
        }

        /// <summary>
        /// Tests enumerating sequences of size 4.
        /// </summary>
        [Test]
        public void TestSequencesSize4Closed()
        {
            var tourEnumerable = new int[] { 0, 1, 2, 3, 4 };

            var result = new SequenceEnumerable(tourEnumerable, true, 4).ToList();

            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.Count);
            Assert.IsNotNull(result[0]);
            Assert.IsNotNull(result[1]);
            Assert.IsNotNull(result[2]);
            Assert.IsNotNull(result[3]);
            Assert.IsNotNull(result[4]);
            Assert.AreEqual(0, result[0][0]);
            Assert.AreEqual(1, result[0][1]);
            Assert.AreEqual(2, result[0][2]);
            Assert.AreEqual(3, result[0][3]);

            Assert.AreEqual(1, result[1][0]);
            Assert.AreEqual(2, result[1][1]);
            Assert.AreEqual(3, result[1][2]);
            Assert.AreEqual(4, result[1][3]);

            Assert.AreEqual(2, result[2][0]);
            Assert.AreEqual(3, result[2][1]);
            Assert.AreEqual(4, result[2][2]);
            Assert.AreEqual(0, result[2][3]);

            Assert.AreEqual(3, result[3][0]);
            Assert.AreEqual(4, result[3][1]);
            Assert.AreEqual(0, result[3][2]);
            Assert.AreEqual(1, result[3][3]);

            Assert.AreEqual(4, result[4][0]);
            Assert.AreEqual(0, result[4][1]);
            Assert.AreEqual(1, result[4][2]);
            Assert.AreEqual(2, result[4][3]);
        }

        /// <summary>
        /// Tests enumerating sequences of size the same as the tour.
        /// </summary>
        [Test]
        public void TestSequencesSizeSameAsTourOpen()
        {
            var tourEnumerable = new int[] { 0, 1, 2, 3, 4 };

            var result = new SequenceEnumerable(tourEnumerable, false, 5).ToList();

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.IsNotNull(result[0]);
            Assert.AreEqual(0, result[0][0]);
            Assert.AreEqual(1, result[0][1]);
            Assert.AreEqual(2, result[0][2]);
            Assert.AreEqual(3, result[0][3]);
            Assert.AreEqual(4, result[0][4]);
        }

        /// <summary>
        /// Tests enumerating sequences of size the same as the tour.
        /// </summary>
        [Test]
        public void TestSequencesSizeSameAsTourClosed()
        {
            var tourEnumerable = new int[] { 0, 1, 2, 3, 4 };

            var result = new SequenceEnumerable(tourEnumerable, true, 5).ToList();

            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.Count);
            Assert.IsNotNull(result[0]);
            Assert.IsNotNull(result[1]);
            Assert.IsNotNull(result[2]);
            Assert.IsNotNull(result[3]);
            Assert.IsNotNull(result[4]);
            Assert.AreEqual(0, result[0][0]);
            Assert.AreEqual(1, result[0][1]);
            Assert.AreEqual(2, result[0][2]);
            Assert.AreEqual(3, result[0][3]);
            Assert.AreEqual(4, result[0][4]);

            Assert.AreEqual(1, result[1][0]);
            Assert.AreEqual(2, result[1][1]);
            Assert.AreEqual(3, result[1][2]);
            Assert.AreEqual(4, result[1][3]);
            Assert.AreEqual(0, result[1][4]);

            Assert.AreEqual(2, result[2][0]);
            Assert.AreEqual(3, result[2][1]);
            Assert.AreEqual(4, result[2][2]);
            Assert.AreEqual(0, result[2][3]);
            Assert.AreEqual(1, result[2][4]);

            Assert.AreEqual(3, result[3][0]);
            Assert.AreEqual(4, result[3][1]);
            Assert.AreEqual(0, result[3][2]);
            Assert.AreEqual(1, result[3][3]);
            Assert.AreEqual(2, result[3][4]);

            Assert.AreEqual(4, result[4][0]);
            Assert.AreEqual(0, result[4][1]);
            Assert.AreEqual(1, result[4][2]);
            Assert.AreEqual(2, result[4][3]);
            Assert.AreEqual(3, result[4][4]);
        }
    }
}