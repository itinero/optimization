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
using OsmSharp.Logistics.Solutions.TSP.Clustering;

namespace OsmSharp.Logistics.Tests.Solutions.TSP.Clustering
{
    /// <summary>
    /// Contains tests for asymmetric clustering.
    /// </summary>
    [TestFixture]
    public class AsymmetricClusteringTests
    {
        /// <summary>
        /// Tests clustering one pair in a matrix of four.
        /// </summary>
        [Test]
        public void TestOnePair4()
        {
            // build a default matrix.
            var defaultWeight = 60;
            var weights = new float[4][];
            for(var x = 0; x < weights.Length; x++)
            {
                weights[x] = new float[weights.Length];
                for(var y = 0; y < weights[x].Length; y++)
                {
                    weights[x][y] = defaultWeight;
                }
                weights[x][x] = 0;
            }

            // set 2 and 3 as close.
            weights[2][3] = 3;
            weights[3][2] = 3;

            // execute the clustering and merge 2 and 3.
            var clustering = new AsymmetricClustering(weights);
            clustering.Run();

            // check result.
            var clusters = clustering.Clusters;
            Assert.AreEqual(3, clusters.Count);
            Assert.AreEqual(1, clusters[0].Count);
            Assert.AreEqual(0, clusters[0][0]);
            Assert.AreEqual(1, clusters[1].Count);
            Assert.AreEqual(1, clusters[1][0]);
            Assert.IsTrue(clusters[2].Contains(2));
            Assert.IsTrue(clusters[2].Contains(3));
            var clustered = clustering.Weights;
            Assert.AreEqual(0, clustered[0][0]);
            Assert.AreEqual(60, clustered[0][1]);
            Assert.AreEqual(60, clustered[0][2]);
            Assert.AreEqual(60, clustered[1][0]);
            Assert.AreEqual(0, clustered[1][1]);
            Assert.AreEqual(60, clustered[1][2]);
            Assert.AreEqual(60, clustered[2][0]);
            Assert.AreEqual(60, clustered[2][1]);
            Assert.AreEqual(0, clustered[2][2]);
        }

        /// <summary>
        /// Tests clustering two pairs in a matrix of four.
        /// </summary>
        [Test]
        public void TestTwoPairs4()
        {
            // build a default matrix.
            var defaultWeight = 60;
            var weights = new float[4][];
            for (var x = 0; x < weights.Length; x++)
            {
                weights[x] = new float[weights.Length];
                for (var y = 0; y < weights[x].Length; y++)
                {
                    weights[x][y] = defaultWeight;
                }
                weights[x][x] = 0;
            }

            // set 2 and 3 and 1 and 2 as close.
            weights[2][3] = 3;
            weights[3][2] = 3;
            weights[1][2] = 3;
            weights[2][1] = 3;
            weights[1][3] = 6;
            weights[3][1] = 6;

            // execute the clustering and merge 2 and 3.
            var clustering = new AsymmetricClustering(weights);
            clustering.Run();

            // check result.
            var clusters = clustering.Clusters;
            Assert.AreEqual(2, clusters.Count);
            Assert.AreEqual(1, clusters[0].Count);
            Assert.AreEqual(0, clusters[0][0]);
            Assert.AreEqual(3, clusters[1].Count);
            Assert.IsTrue(clusters[1].Contains(1));
            Assert.IsTrue(clusters[1].Contains(2));
            Assert.IsTrue(clusters[1].Contains(3));
            var clustered = clustering.Weights;
            Assert.AreEqual(0, clustered[0][0]);
            Assert.AreEqual(60, clustered[0][1]);
            Assert.AreEqual(60, clustered[1][0]);
            Assert.AreEqual(0, clustered[1][1]);
        }

        /// <summary>
        /// Tests clustering of a sequence in a matrix of 10.
        /// </summary>
        [Test]
        public void TestSequence10()
        {
            // build a default matrix.
            var defaultWeight = 60;
            var size = 10;
            var weights = new float[size][];
            for (var x = 0; x < weights.Length; x++)
            {
                weights[x] = new float[weights.Length];
                for (var y = 0; y < weights[x].Length; y++)
                {
                    weights[x][y] = defaultWeight;
                }
                weights[x][x] = 0;
            }

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

            // execute the clustering and merge 2 and 3.
            var clustering = new AsymmetricClustering(weights);
            clustering.Run();

            // check result.
            var clusters = clustering.Clusters;
            Assert.AreEqual(6, clusters.Count);
            Assert.AreEqual(1, clusters[0].Count);
            Assert.AreEqual(0, clusters[0][0]);
            Assert.AreEqual(5, clusters[1].Count);
            Assert.IsTrue(clusters[1].Contains(1));
            Assert.IsTrue(clusters[1].Contains(2));
            Assert.IsTrue(clusters[1].Contains(3));
            Assert.IsTrue(clusters[1].Contains(4));
            Assert.IsTrue(clusters[1].Contains(5));
            Assert.AreEqual(1, clusters[2].Count);
            Assert.AreEqual(6, clusters[2][0]);
            Assert.AreEqual(1, clusters[3].Count);
            Assert.AreEqual(7, clusters[3][0]);
            Assert.AreEqual(1, clusters[4].Count);
            Assert.AreEqual(8, clusters[4][0]);
            Assert.AreEqual(1, clusters[5].Count);
            Assert.AreEqual(9, clusters[5][0]);
            var clustered = clustering.Weights;
            Assert.AreEqual(0, clustered[0][0]);
            Assert.AreEqual(60, clustered[0][1]);
            Assert.AreEqual(60, clustered[0][2]);
            Assert.AreEqual(60, clustered[0][3]);
            Assert.AreEqual(60, clustered[0][4]);
            Assert.AreEqual(60, clustered[0][5]);
            Assert.AreEqual(60, clustered[1][0]);
            Assert.AreEqual(0, clustered[1][1]);
            Assert.AreEqual(60, clustered[1][2]);
            Assert.AreEqual(60, clustered[1][3]);
            Assert.AreEqual(60, clustered[1][4]);
            Assert.AreEqual(60, clustered[1][5]);
            Assert.AreEqual(60, clustered[2][0]);
            Assert.AreEqual(60, clustered[2][1]);
            Assert.AreEqual(0, clustered[2][2]);
            Assert.AreEqual(60, clustered[2][3]);
            Assert.AreEqual(60, clustered[2][4]);
            Assert.AreEqual(60, clustered[2][5]);
            Assert.AreEqual(60, clustered[3][0]);
            Assert.AreEqual(60, clustered[3][1]);
            Assert.AreEqual(60, clustered[3][2]);
            Assert.AreEqual(0, clustered[3][3]);
            Assert.AreEqual(60, clustered[3][4]);
            Assert.AreEqual(60, clustered[3][5]);
            Assert.AreEqual(60, clustered[4][0]);
            Assert.AreEqual(60, clustered[4][1]);
            Assert.AreEqual(60, clustered[4][2]);
            Assert.AreEqual(60, clustered[4][3]);
            Assert.AreEqual(0, clustered[4][4]);
            Assert.AreEqual(60, clustered[4][5]);
            Assert.AreEqual(60, clustered[5][0]);
            Assert.AreEqual(60, clustered[5][1]);
            Assert.AreEqual(60, clustered[5][2]);
            Assert.AreEqual(60, clustered[5][3]);
            Assert.AreEqual(60, clustered[5][4]);
            Assert.AreEqual(0, clustered[5][5]);
        }
    }
}