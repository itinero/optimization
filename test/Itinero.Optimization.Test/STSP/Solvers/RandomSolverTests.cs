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
using Itinero.Optimization.STSP;
using Itinero.Optimization.STSP.Solvers;
using NUnit.Framework;
using System.Collections.Generic;

namespace Itinero.Optimization.Test.STSP.Solvers
{
    /// <summary>
    /// Contains tests for the random TSP solver.
    /// </summary>
    [TestFixture]
    public class RandomSolverTests
    {
        /// <summary>
        /// Tests the solver on a 'open' s-tsp.
        /// </summary>
        [Test]
        public void TestSolutionOpen()
        {
            RandomGeneratorExtensions.GetGetNewRandom = () => new RandomGenerator(4541247);

            // create problem.
            var problem = STSPHelper.CreateSTSP(0, 5, 10, 40);

            // create the solver.
            var solver = new RandomSolver();
            var objective = new STSPObjective();

            for (var i = 0; i < 100; i++)
            {
                // generate solution.
                STSPFitness fitness;
                var solution = solver.Solve(problem, objective, out fitness);

                // test contents.
                Assert.AreEqual(problem.First, solution.First);
                Assert.AreEqual(null, solution.Last);
                Assert.AreEqual(solution.Count, fitness.Customers);
                Assert.IsTrue(40 >= fitness.Weight);
            }
        }

        /// <summary>
        /// Tests the solver on a 'open' but fixed s-tsp.
        /// </summary>
        [Test]
        public void TestSolutionOpenFixed()
        {
            RandomGeneratorExtensions.GetGetNewRandom = () => new RandomGenerator(4541247);

            // create problem.
            var problem = STSPHelper.CreateSTSP(0, 4, 5, 10, 40);

            // create the solver.
            var solver = new RandomSolver();
            var objective = new STSPObjective();

            for (var i = 0; i < 100; i++)
            {
                // generate solution.
                STSPFitness fitness;
                var solution = solver.Solve(problem, objective, out fitness);

                // test contents.
                Assert.AreEqual(problem.First, solution.First);
                Assert.AreEqual(problem.Last, solution.Last);
                Assert.AreEqual(solution.Count, fitness.Customers);
                Assert.IsTrue(40 >= fitness.Weight);
            }
        }

        /// <summary>
        /// Tests the solver on a 'closed' s-tsp.
        /// </summary>
        [Test]
        public void TestSolutionClosed()
        {
            RandomGeneratorExtensions.GetGetNewRandom = () => new RandomGenerator(4541247);

            // create problem.
            var problem = STSPHelper.CreateSTSP(0, 0, 5, 10, 40);

            // create the solver.
            var solver = new RandomSolver();
            var objective = new STSPObjective();

            for (var i = 0; i < 100; i++)
            {
                // generate solution.
                STSPFitness fitness;
                var solution = solver.Solve(problem, objective, out fitness);

                // test contents.
                Assert.AreEqual(problem.First, solution.First);
                Assert.AreEqual(problem.Last, solution.Last);
                Assert.AreEqual(solution.Count, fitness.Customers);
                Assert.IsTrue(40 >= fitness.Weight);
            }
        }
    }
}