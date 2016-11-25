// Itinero.Logistics - Route optimization for .NET
// Copyright (C) 2015 Abelshausen Ben
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

using NUnit.Framework;
using Itinero.Logistics.Solutions.TSP;
using Itinero.Logistics.Solutions.TSP.LocalSearch;
using Itinero.Logistics.Solutions.TSP.Random;
using System.Linq;
using Itinero.Logistics.Algorithms;

namespace Itinero.Logistics.Tests.Solutions.TSP.LocalSearch
{
    /// <summary>
    /// Holds tests for the cluster mutation operator.
    /// </summary>
    [TestFixture]
    public class ClusterMutationOperatorTests
    {
        /// <summary>
        /// Tests cluster mutation operator.
        /// </summary>
        [Test]
        public void Test5AndAll()
        {
            RandomGeneratorExtensions.GetGetNewRandom = () => new RandomGenerator(4541247);

            // create problem.
            var objective = new MinimumWeightObjective<float>();
            var problem = TSPHelper.CreateTSP(0, 0, 5, 10);
            problem.Weights[0][1] = 0;
            problem.Weights[1][2] = 0;
            problem.Weights[2][3] = 0;
            problem.Weights[3][4] = 0;
            problem.Weights[4][0] = 0;

            // mutate a couple of times.
            int count = 10;
            float delta;
            while(count > 0)
            {
                // create random solution problem.
                var solver = new RandomSolver<float>();
                var solution = solver.Solve(problem, objective);

                // apply local search.
                var localSearch = new ClusterMutationOperator<float>();
                localSearch.Apply(problem, new MinimumWeightObjective<float>(), solution, out delta);

                Assert.AreEqual(new int[] { 0, 1, 2, 3, 4 }, solution.ToArray());

                count--;
            }
        }

        /// <summary>
        /// Tests cluster mutation operator.
        /// </summary>
        [Test]
        public void Test10And1()
        {
            RandomGeneratorExtensions.GetGetNewRandom = () => new RandomGenerator(4541247);

            // create problem.
            var problem = TSPHelper.CreateTSP(0, 0, 10, 10);
            problem.Weights[2][3] = 0;
            problem.Weights[5][2] = 0;

            // mutate a couple of times.
            int count = 10;
            float delta;
            while (count > 0)
            {
                // create random solution problem.
                var solver = new RandomSolver<float>();
                var solution = solver.Solve(problem, new MinimumWeightObjective<float>());

                // apply local search.
                var localSearch = new ClusterMutationOperator<float>();
                localSearch.Apply(problem, new MinimumWeightObjective<float>(), solution, out delta);

                Assert.AreEqual(3, solution.GetNeigbour(2));
                Assert.AreEqual(2, solution.GetNeigbour(5));

                count--;
            }
        }
    }
}
