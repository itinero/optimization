// Itinero.Optimization - Route optimization for .NET
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

using Itinero.Optimization.Algorithms.Random;
using Itinero.Optimization.TSP;
using Itinero.Optimization.TSP.Solvers;
using NUnit.Framework;
using System.Collections.Generic;

namespace Itinero.Optimization.Test.TSP.Solvers
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
            var solver = new Optimization.TSP.Directed.Solvers.RandomSolver();

            Assert.AreEqual("RAN", solver.Name);
        }

        /// <summary>
        /// Tests the solver on a 'open' tsp.
        /// </summary>
        [Test]
        public void TestSolutionOpen()
        {
            RandomGeneratorExtensions.GetGetNewRandom = () => new RandomGenerator(4541247);

            // create problem.
            var problem = TSPHelper.CreateTSP(0, 5, 10);

            // create the solver.
            var solver = new RandomSolver();
            var objective = new TSPObjective();

            for (var i = 0; i < 100; i++)
            {
                // generate solution.
                float fitness;
                var solution = solver.Solve(problem, objective, out fitness);

                // test contents.
                Assert.AreEqual(40, fitness);
                Assert.AreEqual(problem.First, solution.First);
                Assert.AreEqual(null, solution.Last);

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

        /// <summary>
        /// Tests the solver on a closed tsp.
        /// </summary>
        [Test]
        public void TestSolutionClosed()
        {
            RandomGeneratorExtensions.GetGetNewRandom = () => new RandomGenerator(4541247);

            // create problem.
            var problem = TSPHelper.CreateTSP(0, 0, 5, 10);

            // create the solver.
            var solver = new RandomSolver();

            for (int i = 0; i < 100; i++)
            {
                // generate solution.
                float fitness;
                var solution = solver.Solve(problem, new TSPObjective(), out fitness);

                // test contents.
                Assert.AreEqual(50, fitness);
                Assert.AreEqual(0, solution.First);
                Assert.AreEqual(0, solution.Last);
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

        /// <summary>
        /// Tests the solver on a closed tsp.
        /// </summary>
        [Test]
        public void TestSolutionFixed()
        {
            RandomGeneratorExtensions.GetGetNewRandom = () => new RandomGenerator(4541247);

            // create problem.
            var problem = TSPHelper.CreateTSP(0, 4, 5, 10);

            // create the solver.
            var solver = new RandomSolver();

            for (int i = 0; i < 100; i++)
            {
                // generate solution.
                float fitness;
                var solution = solver.Solve(problem, new TSPObjective(), out fitness);

                // test contents.
                Assert.AreEqual(40, fitness);
                Assert.AreEqual(0, solution.First);
                Assert.AreEqual(4, solution.Last);
                var solutionList = new List<int>(solution);
                Assert.AreEqual(problem.Last, solutionList[solutionList.Count - 1]);
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