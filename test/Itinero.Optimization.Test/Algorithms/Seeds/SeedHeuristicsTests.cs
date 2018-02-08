/*
 *  Licensed to SharpSoftware under one or more contributor
 *  license agreements. See the NOTICE file distributed with this work for 
 *  additional information regarding copyright ownership.
 * 
 *  SharpSoftware licenses this file to you under the Apache License, 
 *  Version 2.0 (the "License"); you may not use this file except in 
 *  compliance with the License. You may obtain a copy of the License at
 * 
 *       http://www.apache.org/licenses/LICENSE-2.0
 * 
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */

using System.Collections.Generic;
using Itinero.LocalGeo;
using Itinero.Optimization.Test.Staging;
using Itinero.Optimization.Algorithms.Seeds;
using NUnit.Framework;
using System.Linq;

namespace Itinero.Optimization.Test.Algorithms.Seeds
{
    /// <summary>
    /// Contains tests for the seed heuristics.
    /// </summary>
    [TestFixture]
    public class SeedHeuristicsTests
    {
        /// <summary>
        /// Tests getting the fartest seed.
        /// </summary>
        [Test]
        public void TestGetSeedFarthest()
        {
            List<Coordinate> locations;
            var weights = StagingHelpers.GetFeatureCollection("data.geometric.problem1.geojson").BuildMatrix(out locations);
            var visits = Enumerable.Range(0, weights.Length);

            foreach(var visit in visits)
            {
                Assert.AreEqual(GetFarthest(visit, locations), 
                    SeedHeuristics.GetSeedFarthest(weights, visits, visit));
            }
        }

        private static int GetFarthest(int l, List<Coordinate> locations)
        {
            var location = locations[l];
            var distance = -1f;
            var best = -1;
            for (var i = 0; i < locations.Count; i++)
            {
                var localDistance = Coordinate.DistanceEstimateInMeter(location, locations[i]);
                if (localDistance > distance)
                {
                    distance = localDistance;
                    best = i;
                }
            }
            return best;
        }

        /// <summary>
        /// Tests getting the closest seed.
        /// </summary>
        [Test]
        public void TestGetSeedClosest()
        {
            List<Coordinate> locations;
            var weights = StagingHelpers.GetFeatureCollection("data.geometric.problem1.geojson").BuildMatrix(out locations);
            var visits = Enumerable.Range(0, weights.Length);

            foreach(var visit in visits)
            {
                Assert.AreEqual(GetClosest(visit, locations), 
                    SeedHeuristics.GetSeedClosest(weights, visits, visit));
            }
        }

        private static int GetClosest(int l, List<Coordinate> locations)
        {
            var location = locations[l];
            var distance = float.MaxValue;
            var best = -1;
            for (var i = 0; i < locations.Count; i++)
            {
                var localDistance = Coordinate.DistanceEstimateInMeter(location, locations[i]);
                if (localDistance < distance)
                {
                    distance = localDistance;
                    best = i;
                }
            }
            return best;
        }

        /// <summary>
        /// Tests getting the seed with the closest neighbours.
        /// </summary>
        [Test]
        public void TestGetSeedWithCloseNeighbours()
        {
            List<Coordinate> locations;
            var weights = StagingHelpers.GetFeatureCollection("data.geometric.problem1.geojson").BuildMatrix(out locations);
            var visits = Enumerable.Range(0, weights.Length).ToList();

            Assert.AreEqual(GetSeedWithCloseNeighbours(locations), 
                SeedHeuristics.GetSeedWithCloseNeighbours(weights, visits));
        }

        private static int GetSeedWithCloseNeighbours(List<Coordinate> locations)
        {
            var distance = float.MaxValue;
            var best = -1;
            for (var i = 0; i < locations.Count; i++)
            {
                var averageDistance = 0f;
                for (var j = 0; j < locations.Count; j++)
                {
                    averageDistance += Coordinate.DistanceEstimateInMeter(locations[j], locations[i]);
                }
                if (averageDistance < distance)
                {
                    distance = averageDistance;
                    best = i;
                }
            }
            return best;
        }
    }
}