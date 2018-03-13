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

using Itinero.Optimization.Algorithms.Directed;
using Itinero.Optimization.Abstract.Tours;
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