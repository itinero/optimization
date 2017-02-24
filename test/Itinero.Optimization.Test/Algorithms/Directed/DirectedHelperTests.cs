// Itinero.Optimization - Route optimization for .NET
// Copyright (C) 2017 Abelshausen Ben
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

using Itinero.Optimization.Algorithms.Directed;
using Itinero.Optimization.Tours;
using NUnit.Framework;

namespace Itinero.Optimization.Test.Algorithms.Directed
{
    /// <summary>
    /// Contains tests for the directed helper functions.
    /// </summary>
    [TestFixture]
    public class DirectedHelperTests
    {
        /// <summary>
        /// Tests calculating weights of directed tours.
        /// </summary>
        [Test]
        public void TestWeight()
        {
            var weights = WeightsHelper.CreateDirected(5, 10);
            var turnPenalties = new float[] { 0, 2, 2, 0 };

            // open ended.
            Assert.AreEqual(10, (new Tour(new int[] { 1, 9 }, null)).Weight(
                weights, turnPenalties));
            Assert.AreEqual(22, (new Tour(new int[] { 1, 5, 9 }, null)).Weight(
                weights, turnPenalties));

            // open but fixed ended.
            Assert.AreEqual(10, (new Tour(new int[] { 1, 9 }, 9)).Weight(
                weights, turnPenalties));
            Assert.AreEqual(22, (new Tour(new int[] { 1, 5, 9 }, 9)).Weight(
                weights, turnPenalties));

            // closed.
            Assert.AreEqual(24, (new Tour(new int[] { 1, 9 }, 1)).Weight(
                weights, turnPenalties));
            Assert.AreEqual(36, (new Tour(new int[] { 1, 5, 9 }, 1)).Weight(
                weights, turnPenalties));
        }

        /// <summary>
        /// Tests average weights.
        /// </summary>
        [Test]
        public void TestAverageWeight()
        {
            var weights = new float[][]
            {
                new float[]
                {
                    10,
                    100,
                    20,
                    200
                },
                new float[]
                {
                    1000,
                    10000,
                    2000,
                    20000
                },
                new float[]
                {
                    10000,
                    100000,
                    20000,
                    200000
                },
                new float[]
                {
                    100000,
                    1000000,
                    200000,
                    2000000
                }
            };

            var average = weights.AverageWeight(0, 1);
            Assert.AreEqual(5555, average);
            average = weights.AverageWeight(1, 0);
            Assert.AreEqual(302500, average);
        }
    }
}