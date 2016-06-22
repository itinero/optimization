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
using Itinero.Logistics.Solutions.STSP.VNS;
using Itinero.Logistics.Weights;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Itinero.Logistics.Tests.Solutions.STSP.Random
{
    /// <summary>
    /// Contains tests for VNS construction STSP solver.
    /// </summary>
    [TestFixture]
    public class VNSConstructionSolverTests
    {
        /// <summary>
        /// Tests the solver name.
        /// </summary>
        [Test]
        public void TestName()
        {
            // create the solver.
            var solver = new RandomSolver<float>();

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
            problem.Weights[0][1] = 1;
            problem.Weights[1][2] = 1;
            problem.Weights[2][3] = 1;
            problem.Weights[3][4] = 1;
            problem.Weights[4][5] = 1;
            problem.Weights[5][6] = 1;
            problem.Weights[6][7] = 1;
            problem.Weights[7][8] = 1;
            problem.Weights[8][9] = 1;
            problem.Weights[9][0] = 1;

            // create the solver.
            var solver = new VNSConstructionSolver<float>();
            var objective = new MinimumWeightObjective<float>(new DefaultWeightHandler());

            for (var i = 0; i < 100; i++)
            {
                // generate solution.
                float fitness;
                var solution = solver.Solve(problem, objective, out fitness);

                // test contents.
                Assert.IsTrue(fitness <= problem.Max * (problem.Weights.Length - solution.Count));
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
            problem.Weights[0][1] = 1;
            problem.Weights[1][2] = 1;
            problem.Weights[2][3] = 1;
            problem.Weights[3][4] = 1;
            problem.Weights[4][5] = 1;
            problem.Weights[5][6] = 1;
            problem.Weights[6][7] = 1;
            problem.Weights[7][8] = 1;
            problem.Weights[8][9] = 1;
            problem.Weights[9][0] = 1;

            // create the solver.
            var solver = new VNSConstructionSolver<float>();

            for (int i = 0; i < 100; i++)
            {
                // generate solution.
                float fitness;
                var solution = solver.Solve(problem, new MinimumWeightObjective<float>(new DefaultWeightHandler()), out fitness);

                // test contents.
                Assert.IsTrue(fitness <= problem.Max * (problem.Weights.Length - solution.Count));
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
            problem.Weights[0][1] = 1;
            problem.Weights[1][2] = 1;
            problem.Weights[2][3] = 1;
            problem.Weights[3][4] = 1;
            problem.Weights[4][5] = 1;
            problem.Weights[5][6] = 1;
            problem.Weights[6][7] = 1;
            problem.Weights[7][8] = 1;
            problem.Weights[8][9] = 1;
            problem.Weights[9][0] = 1;

            //create the solver.
            var solver = new RandomSolver<float>();

            for (int i = 0; i < 100; i++)
            {
                // generate solution.
                float fitness;
                var solution = solver.Solve(problem, new MinimumWeightObjective<float>(new DefaultWeightHandler()), out fitness);

                // test contents.
                Assert.IsTrue(fitness <= problem.Max * (problem.Weights.Length - solution.Count));
                Assert.AreEqual(problem.First, solution.First);
                Assert.AreEqual(problem.Last, solution.Last);
            }
        }
    }
}
