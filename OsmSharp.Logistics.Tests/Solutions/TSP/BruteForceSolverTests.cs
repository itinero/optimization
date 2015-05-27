// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

using NUnit.Framework;
using OsmSharp.Logistics.Solutions.TSP;
using System.Linq;

namespace OsmSharp.Logistics.Tests.Solutions.TSP
{
    /// <summary>
    /// Contains tests for the brute force solver.
    /// </summary>
    [TestFixture]
    public class BruteForceSolverTests
    {
        /// <summary>
        /// Tests the solver name.
        /// </summary>
        [Test]
        public void TestName()
        {
            // create the solver.
            var solver = new BruteForceSolver();

            Assert.AreEqual("BF", solver.Name);
        }

        /// <summary>
        /// Tests the solver.
        /// </summary>
        [Test]
        public void TestSolver()
        {
            // create the problem and make sure 0->1->2->3->4 is the solution.
            var problem = new TSPProblemMock(0, 5, 10, true);
            problem.Weights[0][1] = 1;
            problem.Weights[1][2] = 1;
            problem.Weights[2][3] = 1;
            problem.Weights[3][4] = 1;
            problem.Weights[4][0] = 1;

            // create the solver.
            var solver = new BruteForceSolver();
            var solution = solver.Solve(problem);

            // verify solution.
            Assert.AreEqual(new int[] { 0, 1, 2, 3, 4 }, solution.ToArray());
        }
    }
}
