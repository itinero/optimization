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
using Itinero.Optimization.TimeWindows;
using Itinero.Optimization.TSP.TimeWindows.Directed;
using Itinero.Optimization.TSP.TimeWindows.Directed.Solvers.Operators;
using NUnit.Framework;
using System.Linq;

namespace Itinero.Optimization.Test.TSP.TimeWindows.Directed.Solvers.Operators
{
    /// <summary>
    /// Containts tests for the 2opt tests.
    /// </summary>
    [TestFixture]
    public class Local2OptTests
    {
        /// <summary>
        /// Tests a feasible route where there is a 2opt-move possible.
        /// </summary>
        [Test]
        public void Test1MovePossible()
        {
            // create the problem and make sure 0->1->2->3->4 is the solution.
            var objective = new TSPTWObjective();
            var problem = TSPTWHelper.CreateDirectedTSPTW(0, 0, 5, 10, 1);
            problem.Times.SetWeight(0, 1, 1);
            problem.Times.SetWeight(1, 2, 1);
            problem.Times.SetWeight(2, 3, 1);
            problem.Times.SetWeight(3, 4, 1);
            problem.Times.SetWeight(4, 0, 1);

            problem.Times.SetWeight(3, 1, 100);

            var route = new Optimization.Tours.Tour(new int[] { 0, 13, 9, 5, 17 }, 0);

            var localSearch = new Local2Opt();
            var delta = 0.0f;
            Assert.IsTrue(localSearch.Apply(problem, objective, route, out delta));

            // test result.
            Assert.AreEqual(42, delta);
            Assert.AreEqual(new int[] { 0, 4, 8, 12, 17 }, route.ToArray());
        }

        /// <summary>
        /// Tests a feasible route where there is a 2opt-move possible.
        /// </summary>
        [Test]
        public void Test1MoveImpossible()
        {
            // create the problem and make sure 0->1->2->3->4 is the solution.
            var objective = new TSPTWObjective();
            var problem = TSPTWHelper.CreateDirectedTSPTW(0, 0, 5, 10, 1);
            problem.Times.SetWeight(0, 1, 1);
            problem.Times.SetWeight(0, 3, 2);
            problem.Times.SetWeight(1, 2, 1);
            problem.Times.SetWeight(2, 3, 1);
            problem.Times.SetWeight(3, 4, 1);
            problem.Times.SetWeight(4, 0, 1);

            problem.Times.SetWeight(3, 1, 100);
            problem.Windows[3] = new TimeWindow()
            {
                Min = 1,
                Max = 2
            };
            problem.Windows[2] = new TimeWindow()
            {
                Min = 11,
                Max = 12
            };

            var route = new Optimization.Tours.Tour(new int[] { 0, 13, 9, 4, 16 }, 0);

            var localSearch = new Local2Opt();
            var delta = 0.0f;
            Assert.IsFalse(localSearch.Apply(problem, objective, route, out delta));
        }
    }
}
