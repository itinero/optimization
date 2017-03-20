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

using Itinero.Optimization.TimeWindows;
using Itinero.Optimization.TSP.TimeWindows;
using NUnit.Framework;

namespace Itinero.Optimization.Test.TSP.TimeWindows
{
    /// <summary>
    /// Tests the feasible objective.
    /// </summary>
    [TestFixture]
    public class TSPTWFeasibleObjectiveTests
    {
        /// <summary>
        /// Tests the objective with no windows.
        /// </summary>
        [Test]
        public void TestNoWindows()
        {
            // create problem.
            var problem = TSPTWHelper.CreateTSPTW(0, 0, 5, 10);

            // calculate objective function.
            var objective = new TSPTWFeasibleObjective();
            Assert.AreEqual(0, objective.Calculate(problem, new Optimization.Tours.Tour(new int[] { 0, 1, 2, 3, 4 })));
        }

        /// <summary>
        /// Tests the objective with valid windows.
        /// </summary>
        [Test]
        public void TestWindowsValid()
        {
            // create problem.
            var problem = TSPTWHelper.CreateTSPTW(0, 0, 5, 10);
            problem.Windows[1] = new TimeWindow()
            {
                Min = 5,
                Max = 15
            };
            problem.Windows[2] = new TimeWindow()
            {
                Min = 15,
                Max = 25
            };
            problem.Windows[3] = new TimeWindow()
            {
                Min = 25,
                Max = 35
            };
            problem.Windows[4] = new TimeWindow()
            {
                Min = 35,
                Max = 45
            };

            // calculate objective function.
            var objective = new TSPTWFeasibleObjective();
            Assert.AreEqual(0, objective.Calculate(problem, new Optimization.Tours.Tour(new int[] { 0, 1, 2, 3, 4 })));
        }

        /// <summary>
        /// Tests the objective with one invalid window.
        /// </summary>
        [Test]
        public void TestWindowsOneInvalid()
        {
            // create problem.
            var problem = TSPTWHelper.CreateTSPTW(0, 0, 5, 10);
            problem.Windows[1] = new TimeWindow()
            {
                Min = 5,
                Max = 15
            };
            problem.Windows[2] = new TimeWindow()
            {
                Min = 15,
                Max = 25
            };
            problem.Windows[3] = new TimeWindow()
            {
                Min = 25,
                Max = 29 // invalid max.
            };
            problem.Windows[4] = new TimeWindow()
            {
                Min = 35,
                Max = 45
            };

            // calculate objective function.
            var objective = new TSPTWFeasibleObjective();
            Assert.AreEqual(1, objective.Calculate(problem, new Optimization.Tours.Tour(new int[] { 0, 1, 2, 3, 4 })));
        }

        /// <summary>
        /// Tests the objective with valid windows but only with a wait.
        /// </summary>
        [Test]
        public void TestWindowsValidOnlyWithWait()
        {
            // create problem.
            var problem = TSPTWHelper.CreateTSPTW(0, 0, 5, 10);
            problem.Windows[1] = new TimeWindow()
            {
                Min = 5,
                Max = 15
            };
            problem.Windows[2] = new TimeWindow()
            {
                Min = 15,
                Max = 25
            };
            problem.Windows[3] = new TimeWindow()
            {
                Min = 35, // wait here until it's '35'.
                Max = 45
            };
            problem.Windows[4] = new TimeWindow()
            {
                Min = 45,
                Max = 55
            };

            // calculate objective function.
            var objective = new TSPTWFeasibleObjective();
            Assert.AreEqual(0, objective.Calculate(problem, new Optimization.Tours.Tour(new int[] { 0, 1, 2, 3, 4 })));
        }
    }
}
