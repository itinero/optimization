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
using Itinero.Optimization.Algorithms.Random;
using Itinero.Optimization.Solutions.STSP.Directed;
using Itinero.Optimization.Solutions.STSP.Directed.Solver.Operators;
using NUnit.Framework;

namespace Itinero.Optimization.Test.STSP.Directed.Solvers.Operators
{
    /// <summary>
    /// Tests the direction local search operator.
    /// </summary>
    [TestFixture]
    public class DirectionLocalSearchOperatorTests
    {
        /// <summary>
        /// Tests an 'open' route where cheapest insertion can help.
        /// </summary>
        [Test]
        public void TestOpen()
        {
            RandomGeneratorExtensions.GetGetNewRandom = () => new RandomGenerator(4541247);

            // create the problem and make sure 0->1->2->3->4 is the solution.
            var problem = STSPHelper.CreateDirectedSTSP(0, 5, 10, 1, 40);

            // create a route with one shift.
            var route = new Optimization.Tours.Tour(new int[] { 0, 4, 9, 12, 16 }, null);

            // apply the operator.
            var localSearch = new DirectionLocalSearchOperator();
            STSPFitness delta;
            Assert.IsTrue(localSearch.Apply(problem, new STSPObjective(), route, out delta));
        }

        /// <summary>
        /// Tests an 'open' fixed route where cheapest insertion can help.
        /// </summary>
        [Test]
        public void TestOpenFixed()
        {
            RandomGeneratorExtensions.GetGetNewRandom = () => new RandomGenerator(4541247);

            // create the problem and make sure 0->1->2->3->4 is the solution.
            var problem = STSPHelper.CreateDirectedSTSP(0, 4, 5, 10, 1, 40);

            // create a route with one shift.
            var route = new Optimization.Tours.Tour(new int[] { 0, 4, 9, 12, 16 }, 16);

            // apply the operator.
            var localSearch = new DirectionLocalSearchOperator();
            STSPFitness delta;
            Assert.IsTrue(localSearch.Apply(problem, new STSPObjective(), route, out delta));
        }

        /// <summary>
        /// Tests a 'closed' route where cheapest insertion can help.
        /// </summary>
        [Test]
        public void TestClosed()
        {
            RandomGeneratorExtensions.GetGetNewRandom = () => new RandomGenerator(4541247);

            // create the problem and make sure 0->1->2->3->4 is the solution.
            var problem = STSPHelper.CreateDirectedSTSP(0, 0, 5, 10, 1, 40);

            // create a route with one shift.
            var route = new Optimization.Tours.Tour(new int[] { 0, 4, 9, 12, 16 }, 0);

            // apply the operator.
            var localSearch = new DirectionLocalSearchOperator();
            STSPFitness delta;
            Assert.IsTrue(localSearch.Apply(problem, new STSPObjective(), route, out delta));
        }
    }
}