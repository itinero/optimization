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
            Assert.AreEqual(new int[] { 0, 1, 2, 3, 4 }, route.ToArray());
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
