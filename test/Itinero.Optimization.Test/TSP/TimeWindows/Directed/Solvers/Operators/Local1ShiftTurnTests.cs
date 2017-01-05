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

using Itinero.Optimization.Algorithms.Random;
using Itinero.Optimization.TimeWindows;
using Itinero.Optimization.TSP.TimeWindows.Directed;
using Itinero.Optimization.TSP.TimeWindows.Directed.Solvers.Operators;
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
            var route = new Optimization.Tours.Tour(new int[] { 0, 5, 8, 12, 17 }, 0);

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
