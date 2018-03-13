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

using Itinero.Optimization.Algorithms.CheapestInsertion;
using Itinero.Optimization.Algorithms.Directed;
using Itinero.Optimization.Abstract.Tours;
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