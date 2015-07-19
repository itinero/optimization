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
using System.Linq;
using OsmSharp.Logistics.Solutions.TSP;
using OsmSharp.Logistics.Solutions.TSP.Clustering;
using OsmSharp.Logistics.Solutions.TSP.GA.EAX;
using OsmSharp.Logistics.Solvers.GA;

namespace OsmSharp.Logistics.Tests.Solutions.TSP.Clustering
{
    /// <summary>
    /// Contains tests for asymmetric clustering solver.
    /// </summary>
    [TestFixture]
    public class AsymmetricClusteringSolverTests
    {
        /// <summary>
        /// Tests clustering of a sequence in a matrix of 10.
        /// </summary>
        [Test]
        public void TestSequence10()
        {
            // build a default matrix.
            var defaultWeight = 60;
            var size = 10;
            var weights = new double[size][];
            for (var x = 0; x < weights.Length; x++)
            {
                weights[x] = new double[weights.Length];
                for (var y = 0; y < weights[x].Length; y++)
                {
                    weights[x][y] = defaultWeight;
                }
                weights[x][x] = 0;
            }

            weights[0][1] = 10;

            // set 2 and 3 and 1 and 2 as close.
            weights[1][2] = 1;
            weights[2][1] = 1;
            weights[2][3] = 1;
            weights[3][2] = 1;
            weights[4][3] = 1;
            weights[3][4] = 1;
            weights[4][5] = 1;
            weights[5][4] = 1;

            weights[1][3] = 2;
            weights[3][1] = 2;
            weights[2][4] = 2;
            weights[4][2] = 2;
            weights[5][3] = 2;
            weights[3][5] = 2;

            weights[1][4] = 3;
            weights[4][1] = 3;
            weights[2][5] = 3;
            weights[5][2] = 3;

            weights[1][5] = 4;
            weights[5][1] = 4;

            weights[5][6] = 10;
            weights[6][7] = 10;
            weights[7][8] = 10;
            weights[8][9] = 10;

            // execute the clustering and merge 2 and 3.
            var clustering = new AsymmetricClusteringSolver(new EAXSolver(new GASettings()
            {
                CrossOverPercentage = 10,
                ElitismPercentage = 1,
                PopulationSize = 100,
                MaxGenerations = 100000,
                MutationPercentage = 0,
                StagnationCount = 100
            }));
            double fitness;
            var route = clustering.Solve(TSPHelper.CreateTSP(0, weights), new MinimumWeightObjective(), out fitness);

            // verify route.
            Assert.AreEqual(54, fitness);
            Assert.AreEqual(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, route.ToArray());
        }
    }
}