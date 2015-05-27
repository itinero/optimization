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
using OsmSharp.Logistics.Solutions.TSP.Random;
using System.Collections.Generic;

namespace OsmSharp.Logistics.Tests.Solutions.TSP.Random
{
    /// <summary>
    /// Contains tests for the random TSP solver.
    /// </summary>
    [TestFixture]
    public class RandomSolverTests
    {
        /// <summary>
        /// Tests the solver name.
        /// </summary>
        [Test]
        public void TestName()
        {
            // create the solver.
            var solver = new RandomSolver();

            Assert.AreEqual("RAN", solver.Name);
        }

        /// <summary>
        /// Tests the solver.
        /// </summary>
        [Test]
        public void TestSolutions()
        {
            // create problem.
            var problem = new TSPProblemMock(0, 5, 10, true);

            // create the solver.
            var solver = new RandomSolver();

            for (int i = 0; i < 100; i++)
            { 
                // generate solution.
                double fitness;
                var solution = solver.Solve(problem, out fitness);

                // test contents.
                Assert.AreEqual(50, fitness);
                var solutionList = new List<int>(solution);
                Assert.AreEqual(0, solutionList[0]);
                Assert.IsTrue(solutionList.Remove(0));
                Assert.IsTrue(solutionList.Remove(1));
                Assert.IsTrue(solutionList.Remove(2));
                Assert.IsTrue(solutionList.Remove(3));
                Assert.IsTrue(solutionList.Remove(4));
                Assert.AreEqual(0, solutionList.Count);
            }
        }
    }
}