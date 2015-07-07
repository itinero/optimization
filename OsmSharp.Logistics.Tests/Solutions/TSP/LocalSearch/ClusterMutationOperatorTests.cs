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
using OsmSharp.Logistics.Solutions.TSP.LocalSearch;
using OsmSharp.Logistics.Solutions.TSP.Random;
using OsmSharp.Math.Random;
using System.Linq;

namespace OsmSharp.Logistics.Tests.Solutions.TSP.LocalSearch
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
            StaticRandomGenerator.Set(4541247);

            // create problem.
            var problem = TSPHelper.CreateTSP(0, 0, 5, 10);
            problem.Weights[0][1] = 0;
            problem.Weights[1][2] = 0;
            problem.Weights[2][3] = 0;
            problem.Weights[3][4] = 0;
            problem.Weights[4][0] = 0;

            // mutate a couple of times.
            int count = 10;
            double delta;
            while(count > 0)
            {
                // create random solution problem.
                var solver = new RandomSolver();
                var solution = solver.Solve(problem);

                // apply local search.
                var localSearch = new ClusterMutationOperator();
                localSearch.Apply(problem, solution, out delta);

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
            StaticRandomGenerator.Set(4541247);

            // create problem.
            var problem = TSPHelper.CreateTSP(0, 0, 10, 10);
            problem.Weights[2][3] = 0;
            problem.Weights[5][2] = 0;

            // mutate a couple of times.
            int count = 10;
            double delta;
            while (count > 0)
            {
                // create random solution problem.
                var solver = new RandomSolver();
                var solution = solver.Solve(problem);

                // apply local search.
                var localSearch = new ClusterMutationOperator();
                localSearch.Apply(problem, solution, out delta);

                Assert.AreEqual(new int[] { 3 }, solution.GetNeigbours(2));
                Assert.AreEqual(new int[] { 2 }, solution.GetNeigbours(5));

                count--;
            }
        }
    }
}
