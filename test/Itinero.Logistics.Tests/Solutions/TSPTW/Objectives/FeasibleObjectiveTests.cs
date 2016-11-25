// Itinero.Logistics - Route optimization for .NET
// Copyright (C) 2015 Abelshausen Ben
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
using Itinero.Logistics.Solutions.TSPTW.Objectives;

namespace Itinero.Logistics.Tests.Solutions.TSPTW.Objectives
{
    /// <summary>
    /// Tests the feasible objective.
    /// </summary>
    [TestFixture]
    public class FeasibleObjectiveTests
    {
        /// <summary>
        /// Tests the objective with no windows.
        /// </summary>
        [Test]
        public void TestNoWindows()
        { 
            // create problem.
            var problem = TSPTWHelper.CreateTSP(0, 0, 5, 10);

            // calculate objective function.
            var objective = new FeasibleObjective<float>();
            Assert.AreEqual(0, objective.Calculate(problem, new Logistics.Routes.Route(new int[] { 0, 1, 2, 3, 4})));
        }

        /// <summary>
        /// Tests the objective with valid windows.
        /// </summary>
        [Test]
        public void TestWindowsValid()
        {
            // create problem.
            var problem = TSPTWHelper.CreateTSP(0, 0, 5, 10);
            problem.Windows[1] = new Logistics.Solutions.TimeWindow()
            {
                Min = 5,
                Max = 15
            };
            problem.Windows[2] = new Logistics.Solutions.TimeWindow()
            {
                Min = 15,
                Max = 25
            };
            problem.Windows[3] = new Logistics.Solutions.TimeWindow()
            {
                Min = 25,
                Max = 35
            };
            problem.Windows[4] = new Logistics.Solutions.TimeWindow()
            {
                Min = 35,
                Max = 45
            };

            // calculate objective function.
            var objective = new FeasibleObjective<float>();
            Assert.AreEqual(0, objective.Calculate(problem, new Logistics.Routes.Route(new int[] { 0, 1, 2, 3, 4 })));
        }

        /// <summary>
        /// Tests the objective with one invalid window.
        /// </summary>
        [Test]
        public void TestWindowsOneInvalid()
        {
            // create problem.
            var problem = TSPTWHelper.CreateTSP(0, 0, 5, 10);
            problem.Windows[1] = new Logistics.Solutions.TimeWindow()
            {
                Min = 5,
                Max = 15
            };
            problem.Windows[2] = new Logistics.Solutions.TimeWindow()
            {
                Min = 15,
                Max = 25
            };
            problem.Windows[3] = new Logistics.Solutions.TimeWindow()
            {
                Min = 25,
                Max = 29 // invalid max.
            };
            problem.Windows[4] = new Logistics.Solutions.TimeWindow()
            {
                Min = 35,
                Max = 45
            };

            // calculate objective function.
            var objective = new FeasibleObjective<float>();
            Assert.AreEqual(1, objective.Calculate(problem, new Logistics.Routes.Route(new int[] { 0, 1, 2, 3, 4 })));
        }

        /// <summary>
        /// Tests the objective with valid windows but only with a wait.
        /// </summary>
        [Test]
        public void TestWindowsValidOnlyWithWait()
        {
            // create problem.
            var problem = TSPTWHelper.CreateTSP(0, 0, 5, 10);
            problem.Windows[1] = new Logistics.Solutions.TimeWindow()
            {
                Min = 5,
                Max = 15
            };
            problem.Windows[2] = new Logistics.Solutions.TimeWindow()
            {
                Min = 15,
                Max = 25
            };
            problem.Windows[3] = new Logistics.Solutions.TimeWindow()
            {
                Min = 35, // wait here until it's '35'.
                Max = 45
            };
            problem.Windows[4] = new Logistics.Solutions.TimeWindow()
            {
                Min = 45,
                Max = 55
            };

            // calculate objective function.
            var objective = new FeasibleObjective<float>();
            Assert.AreEqual(0, objective.Calculate(problem, new Logistics.Routes.Route(new int[] { 0, 1, 2, 3, 4 })));
        }
    }
}