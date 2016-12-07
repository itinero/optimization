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
