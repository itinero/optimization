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
using Itinero.Optimization.TSP.TimeWindows.Directed.Solvers.Operators;
using NUnit.Framework;
using System.Collections.Generic;

namespace Itinero.Optimization.Test.TSP.TimeWindows.Directed.Solvers.Operators
{
    /// <summary>
    /// Containts tests for the VND operator.
    /// </summary>
    [TestFixture]
    public class VNDOperatorTests
    {
        /// <summary>
        /// Initializes for these tests.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            RandomGeneratorExtensions.GetGetNewRandom = () =>
            {
                return new RandomGenerator(45841647);
            };
        }

        /// <summary>
        /// Tests the solver.
        /// </summary>
        [Test]
        public void TestSolutions5ClosedNotFixed()
        {
            RandomGeneratorExtensions.GetGetNewRandom = () =>
            {
                return new RandomGenerator(45841647);
            };

            // create problem.
            var problem = TSPTWHelper.CreateDirectedTSPTW(0, 0, 5, 10, 1);
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

            // create the solver.
            var solver = new VNDOperator();
            for (int i = 0; i < 10; i++)
            {
                // generate solution.
                float fitness;
                var solution = solver.Solve(problem, new TSPTWObjective(), out fitness);

                // test contents.
                Assert.AreEqual(50, fitness, string.Format("Solution was: {0} with fitness {1}.", solution.ToInvariantString(), fitness));
                var solutionList = new List<int>();
                foreach (var directedId in solution)
                {
                    solutionList.Add(DirectedHelper.ExtractId(directedId));
                }
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
        /// Cleans up for these tests.
        /// </summary>
        [OneTimeTearDown]
        public void Dispose()
        {
            RandomGeneratorExtensions.Reset();
        }
    }
}
