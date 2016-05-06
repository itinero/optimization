// Itinero.Logistics - Route optimization for .NET
// Copyright (C) 2016 Abelshausen Ben
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

using Itinero.Logistics.Algorithms;
using Itinero.Logistics.Solutions.STSP;
using Itinero.Logistics.Solutions.STSP.Random;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Itinero.Logistics.Tests.Solutions.STSP.Random
{
    /// <summary>
    /// Contains tests for the random STSP solver.
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
        /// Tests the solver on a 'open' stsp.
        /// </summary>
        [Test]
        public void TestSolutionOpen()
        {
            RandomGeneratorExtensions.GetGetNewRandom = () => new RandomGenerator(4541247);

            // create problem.
            var problem = TSP.TSPHelper.CreateTSP(0, 10, 10).ToSTSP(60);

            // create the solver.
            var solver = new RandomSolver();
            var objective = new MinimumWeightObjective();

            for (var i = 0; i < 100; i++)
            {
                // generate solution.
                double fitness;
                var solution = solver.Solve(problem, objective, out fitness);

                // test contents.
                Assert.IsTrue(fitness < problem.Max);
                Assert.AreEqual(problem.First, solution.First);
                Assert.AreEqual(null, solution.Last);
            }
        }

        /// <summary>
        /// Tests the solver on a closed tsp.
        /// </summary>
        [Test]
        public void TestSolutionClosed()
        {
            RandomGeneratorExtensions.GetGetNewRandom = () => new RandomGenerator(4541247);

            // create problem.
            var problem = TSP.TSPHelper.CreateTSP(0, 0, 10, 10).ToSTSP(60);

            // create the solver.
            var solver = new RandomSolver();

            for (int i = 0; i < 100; i++)
            {
                // generate solution.
                double fitness;
                var solution = solver.Solve(problem, new MinimumWeightObjective(), out fitness);

                // test contents.
                Assert.IsTrue(fitness < problem.Max);
                Assert.AreEqual(problem.First, solution.First);
                Assert.AreEqual(problem.First, solution.Last);
            }
        }

        /// <summary>
        /// Tests the solver on a closed stsp.
        /// </summary>
        [Test]
        public void TestSolutionFixed()
        {
            RandomGeneratorExtensions.GetGetNewRandom = () => new RandomGenerator(4541247);

            //create problem.
            var problem = TSP.TSPHelper.CreateTSP(0, 9, 10, 10).ToSTSP(60);

            //create the solver.
            var solver = new RandomSolver();

            for (int i = 0; i < 100; i++)
            {
                // generate solution.
                double fitness;
                var solution = solver.Solve(problem, new MinimumWeightObjective(), out fitness);

                // test contents.
                Assert.IsTrue(fitness < problem.Max);
                Assert.AreEqual(problem.First, solution.First);
                Assert.AreEqual(problem.Last, solution.Last);
            }
        }
    }
}
