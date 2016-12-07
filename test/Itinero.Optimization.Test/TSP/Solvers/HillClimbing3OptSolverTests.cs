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
using System.Linq;

namespace Itinero.Optimization.Test.TSP.Solvers
{
    /// <summary>
    /// Holds tests of the hill-climbing 3-Opt solver.
    /// </summary>
    [TestFixture]
    public class HillClimbing3OptSolverTests
    {
        /// <summary>
        /// Tests 3Opt hillclimbing solver.
        /// </summary>
        [Test]
        public void Test5Closed()
        {
            RandomGeneratorExtensions.GetGetNewRandom = () => new RandomGenerator(4541247);

            // create problem.
            var objective = new TSPObjective();
            var problem = TSPHelper.CreateTSP(0, 0, 5, 10);
            problem.Weights[0][1] = 1;
            problem.Weights[1][2] = 1;
            problem.Weights[2][3] = 1;
            problem.Weights[3][4] = 1;
            problem.Weights[4][0] = 1;

            // solve problem.
            var solver = new HillClimbing3OptSolver();
            var solution = solver.Solve(problem, objective);

            // check result.
            var last = solution.Last();
            Assert.AreEqual(0, solution.Last);
        }
    }
}