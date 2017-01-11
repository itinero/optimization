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

using Itinero.Optimization.Algorithms.CheapestInsertion;
using Itinero.Optimization.Algorithms.Directed;
using Itinero.Optimization.Tours;
using NUnit.Framework;

namespace Itinero.Optimization.Test.Algorithms.CheapestInsertion
{
    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    public class CheapestInsertionDirectedHelperTests
    {
        /// <summary>
        /// Tests adding a second customer to an open route.
        /// </summary>
        [Test]
        public void TestSecondOpen()
        {
            var weights = WeightsHelper.CreateDirected(5, 10);
            var turnPenalties = new float[] { 0, 2, 2, 0 };

            var tour = new Tour(new int[] { 0 }, null);
            var cost = tour.InsertCheapestDirected(weights, turnPenalties, 1);
            Assert.AreEqual(10, cost);
            Assert.AreEqual(new int[] { 0, 4 }, tour);

            tour = new Tour(new int[] { 1 }, null);
            cost = tour.InsertCheapestDirected(weights, turnPenalties, 1);
            Assert.AreEqual(10, cost);
            Assert.AreEqual(new int[] { 0, 4 }, tour);

            tour = new Tour(new int[] { 2 }, null);
            cost = tour.InsertCheapestDirected(weights, turnPenalties, 1);
            Assert.AreEqual(10, cost);
            Assert.AreEqual(new int[] { 0, 4 }, tour);

            tour = new Tour(new int[] { 3 }, null);
            cost = tour.InsertCheapestDirected(weights, turnPenalties, 1);
            Assert.AreEqual(10, cost);
            Assert.AreEqual(new int[] { 0, 4 }, tour);
        }

        /// <summary>
        /// Tests adding a third customer to an open route with a fixed last customer.
        /// </summary>
        [Test]
        public void TestThirdOpenFixed()
        {
            var weights = WeightsHelper.CreateDirected(5, 10);
            var turnPenalties = new float[] { 0, 2, 2, 0 };

            var tour = new Tour(new int[] { 0, 8 }, 8);
            var cost = tour.InsertCheapestDirected(weights, turnPenalties, 1);
            Assert.AreEqual(10, cost);
            Assert.AreEqual(new int[] { 0, 4, 8 }, tour);

            tour = new Tour(new int[] { 0, 10 }, 10);
            cost = tour.InsertCheapestDirected(weights, turnPenalties, 1);
            Assert.AreEqual(10, cost);
            Assert.AreEqual(new int[] { 0, 4, 8 }, tour);
        }

        /// <summary>
        /// Tests adding a second customer to a closed route.
        /// </summary>
        [Test]
        public void TestSecondClosed()
        {
            var weights = WeightsHelper.CreateDirected(5, 10);
            var turnPenalties = new float[] { 0, 2, 2, 0 };

            var tour = new Tour(new int[] { 0 }, 0);
            var cost = tour.InsertCheapestDirected(weights, turnPenalties, 1);
            Assert.AreEqual(20, cost);
            Assert.AreEqual(new int[] { 0, 4 }, tour);

            tour = new Tour(new int[] { 1 }, 1);
            cost = tour.InsertCheapestDirected(weights, turnPenalties, 1);
            Assert.AreEqual(20, cost);
            Assert.AreEqual(new int[] { 0, 4 }, tour);

            tour = new Tour(new int[] { 2 }, 2);
            cost = tour.InsertCheapestDirected(weights, turnPenalties, 1);
            Assert.AreEqual(20, cost);
            Assert.AreEqual(new int[] { 0, 4 }, tour);

            tour = new Tour(new int[] { 3 }, 3);
            cost = tour.InsertCheapestDirected(weights, turnPenalties, 1);
            Assert.AreEqual(20, cost);
            Assert.AreEqual(new int[] { 0, 4 }, tour);
        }

        /// <summary>
        /// Tests if turn cost handling is done correctly.
        /// </summary>
        [Test]
        public void TestTurnCostHandling()
        {
            var weights = WeightsHelper.CreateDirected(5, 10);
            weights.SetWeight(0, 1, 5);
            weights.SetWeight(1, 2, 5);
            weights.SetWeight(2, 3, 5);
            weights.SetWeight(3, 4, 5);
            var turnPenalties = new float[] { 0, 2, 2, 0 };

            var tour = new Tour(new int[] { 1, 5, 13 }, null);
            var weight = tour.Weight(weights, turnPenalties);
            var cost = tour.InsertCheapestDirected(weights, turnPenalties, 2);
            Assert.AreEqual(-2, cost);
            Assert.AreEqual(weight + cost, tour.Weight(weights, turnPenalties));
            Assert.AreEqual(new int[] { 1, 4, 8, 13 }, tour);

            tour = new Tour(new int[] { 1, 5, 13 }, 13);
            weight = tour.Weight(weights, turnPenalties);
            cost = tour.InsertCheapestDirected(weights, turnPenalties, 2);
            Assert.AreEqual(-2, cost);
            Assert.AreEqual(weight + cost, tour.Weight(weights, turnPenalties));
            Assert.AreEqual(new int[] { 1, 4, 8, 13 }, tour);

            tour = new Tour(new int[] { 1, 5, 13 }, 1);
            weight = tour.Weight(weights, turnPenalties);
            cost = tour.InsertCheapestDirected(weights, turnPenalties, 2);
            Assert.AreEqual(-4, cost);
            Assert.AreEqual(weight + cost, tour.Weight(weights, turnPenalties));
            Assert.AreEqual(new int[] { 1, 4, 8, 15 }, tour);
        }
    }
}