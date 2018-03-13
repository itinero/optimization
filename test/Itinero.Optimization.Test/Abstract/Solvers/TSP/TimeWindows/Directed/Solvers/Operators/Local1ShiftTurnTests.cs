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

using Itinero.Optimization.Algorithms.Random;
using Itinero.Optimization.Abstract.Models.TimeWindows;
using Itinero.Optimization.Abstract.Solvers.TSP.TimeWindows.Directed;
using Itinero.Optimization.Abstract.Solvers.TSP.TimeWindows.Directed.Solvers.Operators;
using NUnit.Framework;
using System.Linq;

namespace Itinero.Optimization.Test.TSP.TimeWindows.Directed.Solvers.Operators
{
    /// <summary>
    /// Containts tests for the local1shift turn operator.
    /// </summary>
    [TestFixture]
    public class Local1ShiftTurnTests
    {
        /// <summary>
        /// Initializes for these tests.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            RandomGeneratorExtensions.GetGetNewRandom = () =>
            {
                return new RandomGenerator(4541247);
            };
        }

        /// <summary>
        /// Tests the operator, it needs to find two customers to switch.
        /// </summary>
        [Test]
        public void TestShift1and3()
        {            
            // create the problem and make sure 0->1->2->3->4 is the solution.
            var objective = new TSPTWObjective();
            var problem = TSPTWHelper.CreateDirectedTSPTW(0, 0, 5, 10, 1);
            problem.Windows[2] = new TimeWindow()
            {
                Min = 1,
                Max = 3
            };

            // create a route with one shift.
            var route = new Optimization.Abstract.Tours.Tour(new int[] { 0, 5, 8, 12, 17 }, 0);

            // apply the operator this should shift 5 to 4.
            var localSearch = new Local1ShiftTurn();
            var delta = 0.0f;
            Assert.IsTrue(localSearch.Apply(problem, objective, route, out delta));
            Assert.AreEqual(2, delta);
            Assert.AreEqual(new int[] { 0, 4, 8, 12, 17 }, route.ToArray());

            // apply the operator again, this should shift 17 to 16.
            Assert.IsTrue(localSearch.Apply(problem, objective, route, out delta));
            Assert.AreEqual(2, delta);
            Assert.AreEqual(new int[] { 0, 4, 8, 12, 16 }, route.ToArray());
        }

        /// <summary>
        /// Cleans up for these tests.
        /// </summary>
        [OneTimeTearDown]
        public void Dispose()
        {
            RandomGeneratorExtensions.Reset();
        }
    }
}
