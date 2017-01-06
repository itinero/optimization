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
using Itinero.Optimization.Algorithms.Random;
using Itinero.Optimization.TimeWindows;
using Itinero.Optimization.TSP.TimeWindows.Directed;
using Itinero.Optimization.TSP.TimeWindows.Directed.Solvers;
using NUnit.Framework;
using System.Collections.Generic;

namespace Itinero.Optimization.Test.TSP.TimeWindows.Directed.Solvers
{
    /// <summary>
    /// Containts tests for the VNS solver.
    /// </summary>
    [TestFixture]
    public class VNSSolverTests
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
        /// Tests the solver on a problem with only 1 customer.
        /// </summary>
        [Test]
        public void TestSolution1()
        {
            // create problem.
            var problem = TSPTWHelper.CreateDirectedTSPTW(0, 0, 1, 0, 1);
            var objective = new TSPTWObjective();

            // create the solver.
            var solver = new VNSSolver();
            for (int i = 0; i < 10; i++)
            {
                // generate solution.
                float fitness;
                var solution = solver.Solve(problem, objective, out fitness);

                // test contents.
                Assert.IsTrue(fitness < 2);
                var solutionList = new List<int>(solution);
                Assert.AreEqual(0, DirectedHelper.ExtractId(solutionList[0]));
            }
        }

        /// <summary>
        /// Tests the solver.
        /// </summary>
        [Test]
        public void TestSolution5ClosedNotFixed()
        {
            // create problem.
            var problem = TSPTWHelper.CreateDirectedTSPTW(0, 0, 5, 10, 1);
            problem.Times.SetWeight(0, 1, 2);
            problem.Times.SetWeight(1, 2, 2);
            problem.Times.SetWeight(2, 3, 2);
            problem.Times.SetWeight(3, 4, 2);
            problem.Times.SetWeight(4, 0, 2);
            problem.Windows[2] = new TimeWindow()
            {
                Min = 3,
                Max = 5
            };
            problem.Windows[4] = new TimeWindow()
            {
                Min = 7,
                Max = 9
            };
            var objective = new TSPTWObjective();

            // create the solver.
            var solver = new VNSSolver();
            for (int i = 0; i < 10; i++)
            {
                // generate solution.
                float fitness;
                var solution = solver.Solve(problem, objective, out fitness);

                // test contents.
                Assert.AreEqual(10, fitness);
                var solutionList = new List<int>(solution);
                Assert.AreEqual(5, solutionList.Count);
            }
        }

        /// <summary>
        /// Tests the solver.
        /// </summary>
        [Test]
        public void TestSolution5ClosedFixed()
        {
            // create problem.
            var problem = TSPTWHelper.CreateDirectedTSPTW(0, 4, 5, 10, 1);
            problem.Times.SetWeight(0, 1, 2);
            problem.Times.SetWeight(1, 2, 2);
            problem.Times.SetWeight(2, 3, 2);
            problem.Times.SetWeight(3, 4, 2);
            problem.Times.SetWeight(4, 0, 2);
            problem.Windows[2] = new TimeWindow()
            {
                Min = 3,
                Max = 5
            };
            problem.Windows[4] = new TimeWindow()
            {
                Min = 7,
                Max = 9
            };
            var objective = new TSPTWObjective();

            // create the solver.
            var solver = new VNSSolver();
            for (int i = 0; i < 10; i++)
            {
                // generate solution.
                float fitness;
                var solution = solver.Solve(problem, objective, out fitness);

                // test contents.
                Assert.IsTrue(fitness <= 12);
                var solutionList = new List<int>();
                foreach (var directed in solution)
                {
                    solutionList.Add(DirectedHelper.ExtractId(directed));
                }
                Assert.AreEqual(5, solutionList.Count);
                Assert.AreEqual(0, solutionList[0]);
                Assert.AreEqual(4, solutionList[4]);
            }
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
