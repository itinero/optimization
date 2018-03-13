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

using Itinero.Optimization.Abstract.Solvers.TSP;
using Itinero.Optimization.Abstract.Solvers.TSP.Solvers.Operators;
using NUnit.Framework;
using System.Linq;

namespace Itinero.Optimization.Test.TSP.Solvers.Operators
{
    /// <summary>
    /// Contains tests for the Local1Shift local search operator.
    /// </summary>
    [TestFixture]
    public class Local1ShiftTests
    {
        /// <summary>
        /// Tests a route where only one shift is possible/needed.
        /// </summary>
        [Test]
        public void Test1OneShiftClosed()
        {
            // create the problem and make sure 0->1->2->3->4 is the solution.
            var problem = TSPHelper.CreateTSP(0, 0, 5, 10);
            problem.Weights[0][1] = 1;
            problem.Weights[1][2] = 1;
            problem.Weights[2][3] = 1;
            problem.Weights[3][4] = 1;
            problem.Weights[4][0] = 1;

            // create a route with one shift.
            var route = new Optimization.Abstract.Tours.Tour(new int[] { 0, 2, 3, 1, 4 }, 0);

            // apply the 1-shift local search, it should find the customer to replocate.
            var localSearch = new Local1Shift();
            var delta = 0.0f;
            localSearch.Apply(problem, new TSPObjective(), route, out delta);

            // test result.
            Assert.AreEqual(-27, delta);
            Assert.AreEqual(new int[] { 0, 1, 2, 3, 4 }, route.ToArray());
        }

        /// <summary>
        /// Tests a route where only one shift is possible/needed.
        /// </summary>
        [Test]
        public void Test1OneShiftOpen()
        {
            // create the problem and make sure 0->1->2->3->4 is the solution.
            var problem = TSPHelper.CreateTSP(0, 5, 10);
            problem.Weights[0][1] = 1;
            problem.Weights[1][2] = 1;
            problem.Weights[2][3] = 1;
            problem.Weights[3][4] = 1;
            problem.Weights[4][0] = 1;

            // create a route with one shift.
            var route = new Optimization.Abstract.Tours.Tour(new int[] { 0, 2, 3, 1, 4 }, null);

            // apply the 1-shift local search, it should find the customer to replocate.
            var localSearch = new Local1Shift();
            var delta = 0.0f;
            localSearch.Apply(problem, new TSPObjective(), route, out delta);

            // test result.
            Assert.AreEqual(27, delta);
            Assert.AreEqual(new int[] { 0, 1, 2, 3, 4 }, route.ToArray());
        }

        /// <summary>
        /// Tests a route where two shifts are possible/needed.
        /// </summary>
        [Test]
        public void Test2TwoShiftsClosed()
        {
            // create the problem and make sure 0->1->2->3->4 is the solution.
            var problem = TSPHelper.CreateTSP(0, 0, 5, 10);
            problem.Weights[0][1] = 1;
            problem.Weights[1][2] = 1;
            problem.Weights[2][3] = 1;
            problem.Weights[3][4] = 1;
            problem.Weights[4][0] = 1;

            // create a route with one shift.
            var route = new Optimization.Abstract.Tours.Tour(new int[] { 0, 2, 4, 1, 3 }, 0);

            // apply the 1-shift local search, it should find the customer to replocate.
            var localSearch = new Local1Shift();
            var delta = 0.0f;
            localSearch.Apply(problem, new TSPObjective(), route, out delta);

            // test result.
            Assert.AreEqual(-45, delta);
            Assert.AreEqual(new int[] { 0, 1, 2, 3, 4 }, route.ToArray());
        }

        /// <summary>
        /// Tests a route where two shifts are possible/needed.
        /// </summary>
        [Test]
        public void Test2TwoShiftsOpen()
        {
            // create the problem and make sure 0->1->2->3->4 is the solution.
            var problem = TSPHelper.CreateTSP(0, 5, 10);
            problem.Weights[0][1] = 1;
            problem.Weights[1][2] = 1;
            problem.Weights[2][3] = 1;
            problem.Weights[3][4] = 1;
            problem.Weights[4][0] = 1;

            // create a route with one shift.
            var route = new Optimization.Abstract.Tours.Tour(new int[] { 0, 2, 4, 1, 3 }, null);

            // apply the 1-shift local search, it should find the customer to replocate.
            var localSearch = new Local1Shift();
            var delta = 0.0f;
            localSearch.Apply(problem, new TSPObjective(), route, out delta);

            // test result.
            Assert.AreEqual(36, delta);
            Assert.AreEqual(new int[] { 0, 1, 2, 3, 4 }, route.ToArray());
        }
    }
}