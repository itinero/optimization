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

using NUnit.Framework;

namespace Itinero.Optimization.Test.Algorithms.NearestNeighbours
{
    /// <summary>
    /// Holds tests for the nearest neighbour algorithm.
    /// </summary>
    [TestFixture]
    public class NNearestNeighboursAlgorithmTests
    {
        /// <summary>
        /// Tests a 1-nearest neighbour in a 4x4 matrix.
        /// </summary>
        [Test]
        public void Test1Matrix4()
        {
            var matrix = new float[][] {
                new float[] { 0, 1, 2, 3 },
                new float[] { 3, 0, 1, 2 },
                new float[] { 2, 3, 0, 1 },
                new float[] { 1, 2, 3, 0 }};

            var nearest = Optimization.Algorithms.NearestNeighbour.NearestNeighbours.Forward(matrix, 1, 0);
            Assert.IsNotNull(nearest);
            Assert.AreEqual(1, nearest.N);
            Assert.AreEqual(1, nearest.Max);
            Assert.IsTrue(nearest.Contains(1));
            Assert.IsFalse(nearest.Contains(0));
            Assert.IsFalse(nearest.Contains(2));
            Assert.IsFalse(nearest.Contains(3));

            nearest = Optimization.Algorithms.NearestNeighbour.NearestNeighbours.Forward(matrix, 1, 1);
            Assert.IsNotNull(nearest);
            Assert.AreEqual(1, nearest.N);
            Assert.AreEqual(1, nearest.Max);
            Assert.IsTrue(nearest.Contains(2));
            Assert.IsFalse(nearest.Contains(0));
            Assert.IsFalse(nearest.Contains(1));
            Assert.IsFalse(nearest.Contains(3));

            nearest = Optimization.Algorithms.NearestNeighbour.NearestNeighbours.Forward(matrix, 1, 2);
            Assert.IsNotNull(nearest);
            Assert.AreEqual(1, nearest.N);
            Assert.AreEqual(1, nearest.Max);
            Assert.IsTrue(nearest.Contains(3));
            Assert.IsFalse(nearest.Contains(0));
            Assert.IsFalse(nearest.Contains(2));
            Assert.IsFalse(nearest.Contains(1));

            nearest = Optimization.Algorithms.NearestNeighbour.NearestNeighbours.Forward(matrix, 1, 3);
            Assert.IsNotNull(nearest);
            Assert.AreEqual(1, nearest.N);
            Assert.AreEqual(1, nearest.Max);
            Assert.IsTrue(nearest.Contains(0));
            Assert.IsFalse(nearest.Contains(1));
            Assert.IsFalse(nearest.Contains(2));
            Assert.IsFalse(nearest.Contains(3));
        }

        /// <summary>
        /// Tests a 2-nearest neighbour in a 4x4 matrix.
        /// </summary>
        [Test]
        public void Test2Matrix4()
        {
            var matrix = new float[][] {
                new float[] { 0, 1, 2, 3 },
                new float[] { 3, 0, 1, 2 },
                new float[] { 2, 3, 0, 1 },
                new float[] { 1, 2, 3, 0 }};

            var nearest = Optimization.Algorithms.NearestNeighbour.NearestNeighbours.Forward(matrix, 2, 0);
            Assert.IsNotNull(nearest);
            Assert.AreEqual(2, nearest.N);
            Assert.AreEqual(2, nearest.Max);
            Assert.IsTrue(nearest.Contains(1));
            Assert.IsTrue(nearest.Contains(2));
            Assert.IsFalse(nearest.Contains(0));
            Assert.IsFalse(nearest.Contains(3));

            nearest = Optimization.Algorithms.NearestNeighbour.NearestNeighbours.Forward(matrix, 2, 1);
            Assert.IsNotNull(nearest);
            Assert.AreEqual(2, nearest.N);
            Assert.AreEqual(2, nearest.Max);
            Assert.IsTrue(nearest.Contains(2));
            Assert.IsTrue(nearest.Contains(3));
            Assert.IsFalse(nearest.Contains(1));
            Assert.IsFalse(nearest.Contains(0));

            nearest = Optimization.Algorithms.NearestNeighbour.NearestNeighbours.Forward(matrix, 2, 2);
            Assert.IsNotNull(nearest);
            Assert.AreEqual(2, nearest.N);
            Assert.AreEqual(2, nearest.Max);
            Assert.IsTrue(nearest.Contains(3));
            Assert.IsTrue(nearest.Contains(0));
            Assert.IsFalse(nearest.Contains(2));
            Assert.IsFalse(nearest.Contains(1));

            nearest = Optimization.Algorithms.NearestNeighbour.NearestNeighbours.Forward(matrix, 2, 3);
            Assert.IsNotNull(nearest);
            Assert.AreEqual(2, nearest.N);
            Assert.AreEqual(2, nearest.Max);
            Assert.IsTrue(nearest.Contains(0));
            Assert.IsTrue(nearest.Contains(1));
            Assert.IsFalse(nearest.Contains(2));
            Assert.IsFalse(nearest.Contains(3));
        }
    }
}
