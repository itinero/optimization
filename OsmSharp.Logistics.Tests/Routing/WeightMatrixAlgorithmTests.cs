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
using OsmSharp.Collections.Tags;
using OsmSharp.Logistics.Routing;
using OsmSharp.Math.Random;
using OsmSharp.Routing.Vehicles;
using System.Collections.Generic;

namespace OsmSharp.Logistics.Tests.Routing
{
    /// <summary>
    /// Contains tests for the weight-matrix algorithm.
    /// </summary>
    [TestFixture]
    public class WeightMatrixAlgorithmTests
    {
        /// <summary>
        /// Tests a simple two-point matrix.
        /// </summary>
        [Test]
        public void Test1TwoPoints()
        {
            StaticRandomGenerator.Set(4541247);

            // build test case.
            var router = new RouterMock();
            var locations = new Math.Geo.GeoCoordinate[] { 
                    new Math.Geo.GeoCoordinate(0, 0),
                    new Math.Geo.GeoCoordinate(1, 1) };
            var matrixAlgorithm = new WeightMatrixAlgorithm(router, Vehicle.Car, locations);

            // run.
            matrixAlgorithm.Run();

            Assert.IsNotNull(matrixAlgorithm);
            Assert.IsTrue(matrixAlgorithm.HasRun);
            Assert.IsTrue(matrixAlgorithm.HasSucceeded);
            Assert.AreEqual(0, matrixAlgorithm.Errors.Count);
            var matrix = matrixAlgorithm.Weights;
            Assert.IsNotNull(matrix);
            Assert.AreEqual(0, matrix[0][0]);
            Assert.AreEqual(locations[0].DistanceReal(locations[1]).Value, matrix[0][1]);
            Assert.AreEqual(locations[1].DistanceReal(locations[0]).Value, matrix[1][0]);
            Assert.AreEqual(0, matrix[1][1]);
            Assert.AreEqual(2, matrixAlgorithm.RouterPoints.Count);
            Assert.AreEqual(0, matrixAlgorithm.IndexOf(0));
            Assert.AreEqual(1, matrixAlgorithm.IndexOf(1));
        }

        /// <summary>
        /// Tests a simple two-point matrix but with one of the points not 'resolvable'.
        /// </summary>
        [Test]
        public void Test2TwoPoints()
        {
            StaticRandomGenerator.Set(4541247);

            // build test case.
            var router = new RouterMock();
            var locations = new Math.Geo.GeoCoordinate[] { 
                    new Math.Geo.GeoCoordinate(0, 0),
                    new Math.Geo.GeoCoordinate(180, 1) };
            var matrixAlgorithm = new WeightMatrixAlgorithm(router, Vehicle.Car, locations);

            // run.
            matrixAlgorithm.Run();

            Assert.IsNotNull(matrixAlgorithm);
            Assert.IsTrue(matrixAlgorithm.HasRun);
            Assert.IsTrue(matrixAlgorithm.HasSucceeded);
            Assert.AreEqual(1, matrixAlgorithm.Errors.Count);
            Assert.IsTrue(matrixAlgorithm.Errors.ContainsKey(1));
            var matrix = matrixAlgorithm.Weights;
            Assert.IsNotNull(matrix);
            Assert.AreEqual(0, matrix[0][0]);
            Assert.AreEqual(1, matrixAlgorithm.RouterPoints.Count);
            Assert.AreEqual(0, matrixAlgorithm.IndexOf(0));
            Assert.AreEqual(0, matrixAlgorithm.LocationIndexOf(0));

            // build test case.
            locations = new Math.Geo.GeoCoordinate[] { 
                    new Math.Geo.GeoCoordinate(180, 0),
                    new Math.Geo.GeoCoordinate(1, 1) };
            matrixAlgorithm = new WeightMatrixAlgorithm(router, Vehicle.Car, locations);

            // run.
            matrixAlgorithm.Run();

            Assert.IsNotNull(matrixAlgorithm);
            Assert.IsTrue(matrixAlgorithm.HasRun);
            Assert.IsTrue(matrixAlgorithm.HasSucceeded);
            Assert.AreEqual(1, matrixAlgorithm.Errors.Count);
            Assert.IsTrue(matrixAlgorithm.Errors.ContainsKey(0));
            matrix = matrixAlgorithm.Weights;
            Assert.IsNotNull(matrix);
            Assert.AreEqual(0, matrix[0][0]);
            Assert.AreEqual(1, matrixAlgorithm.RouterPoints.Count);
            Assert.AreEqual(0, matrixAlgorithm.IndexOf(1));
            Assert.AreEqual(1, matrixAlgorithm.LocationIndexOf(0));
        }

        /// <summary>
        /// Tests a simple two-point matrix but with one of the points not 'routable'.
        /// </summary>
        [Test]
        public void Test3TwoPoints()
        {
            StaticRandomGenerator.Set(4541247);

            // build test case.
            var invalidSet = new HashSet<int>();
            invalidSet.Add(1);
            var router = new RouterMock(invalidSet);
            var locations = new Math.Geo.GeoCoordinate[] { 
                    new Math.Geo.GeoCoordinate(0, 0),
                    new Math.Geo.GeoCoordinate(1, 1) };
            var matrixAlgorithm = new WeightMatrixAlgorithm(router, Vehicle.Car, locations);

            // run.
            matrixAlgorithm.Run();

            Assert.IsNotNull(matrixAlgorithm);
            Assert.IsTrue(matrixAlgorithm.HasRun);
            Assert.IsTrue(matrixAlgorithm.HasSucceeded);
            Assert.AreEqual(1, matrixAlgorithm.Errors.Count);
            Assert.IsTrue(matrixAlgorithm.Errors.ContainsKey(1));
            var matrix = matrixAlgorithm.Weights;
            Assert.IsNotNull(matrix);
            Assert.AreEqual(0, matrix[0][0]);
            Assert.AreEqual(1, matrixAlgorithm.RouterPoints.Count);
            Assert.AreEqual(0, matrixAlgorithm.IndexOf(0));
            Assert.AreEqual(0, matrixAlgorithm.LocationIndexOf(0));

            // build test case.
            invalidSet.Clear();
            invalidSet.Add(0);
            router = new RouterMock(invalidSet);
            locations = new Math.Geo.GeoCoordinate[] { 
                    new Math.Geo.GeoCoordinate(0, 0),
                    new Math.Geo.GeoCoordinate(1, 1) };
            matrixAlgorithm = new WeightMatrixAlgorithm(router, Vehicle.Car, locations);

            // run.
            matrixAlgorithm.Run();

            Assert.IsNotNull(matrixAlgorithm);
            Assert.IsTrue(matrixAlgorithm.HasRun);
            Assert.IsTrue(matrixAlgorithm.HasSucceeded);
            Assert.AreEqual(1, matrixAlgorithm.Errors.Count);
            Assert.IsTrue(matrixAlgorithm.Errors.ContainsKey(0));
            matrix = matrixAlgorithm.Weights;
            Assert.IsNotNull(matrix);
            Assert.AreEqual(0, matrix[0][0]);
            Assert.AreEqual(1, matrixAlgorithm.RouterPoints.Count);
            Assert.AreEqual(0, matrixAlgorithm.IndexOf(1));
            Assert.AreEqual(1, matrixAlgorithm.LocationIndexOf(0));
        }

        /// <summary>
        /// Tests a simple two-point matrix with edge-matcher.
        /// </summary>
        [Test]
        public void Test1TwoPointsWithMatching()
        {
            StaticRandomGenerator.Set(4541247);

            // build test case.
            var router = new RouterMock(new TagsCollection(new Tag("highway", "residential")));
            var locations = new Math.Geo.GeoCoordinate[] { 
                    new Math.Geo.GeoCoordinate(0, 0),
                    new Math.Geo.GeoCoordinate(1, 1) };
            var matrixAlgorithm = new WeightMatrixAlgorithm(router, Vehicle.Car, locations, (tags) =>
            {
                if(tags != null)
                {
                    return tags.ContainsKeyValue("highway", "residential");
                }
                return false;
            });

            // run.
            matrixAlgorithm.Run();

            Assert.IsNotNull(matrixAlgorithm);
            Assert.IsTrue(matrixAlgorithm.HasRun);
            Assert.IsTrue(matrixAlgorithm.HasSucceeded);
            Assert.AreEqual(0, matrixAlgorithm.Errors.Count);
            var matrix = matrixAlgorithm.Weights;
            Assert.IsNotNull(matrix);
            Assert.AreEqual(0, matrix[0][0]);
            Assert.AreEqual(locations[0].DistanceReal(locations[1]).Value, matrix[0][1]);
            Assert.AreEqual(locations[1].DistanceReal(locations[0]).Value, matrix[1][0]);
            Assert.AreEqual(0, matrix[1][1]);
            Assert.AreEqual(2, matrixAlgorithm.RouterPoints.Count);
            Assert.AreEqual(0, matrixAlgorithm.IndexOf(0));
            Assert.AreEqual(1, matrixAlgorithm.IndexOf(1));

            // build test case.
            router = new RouterMock(new TagsCollection(new Tag("highway", "primary")));
            locations = new Math.Geo.GeoCoordinate[] { 
                    new Math.Geo.GeoCoordinate(0, 0),
                    new Math.Geo.GeoCoordinate(1, 1) };
            matrixAlgorithm = new WeightMatrixAlgorithm(router, Vehicle.Car, locations, (tags) =>
            {
                if (tags != null)
                {
                    return tags.ContainsKeyValue("highway", "residential");
                }
                return false;
            });

            // run.
            matrixAlgorithm.Run();

            Assert.IsNotNull(matrixAlgorithm);
            Assert.IsTrue(matrixAlgorithm.HasRun);
            Assert.IsTrue(matrixAlgorithm.HasSucceeded);
            Assert.AreEqual(2, matrixAlgorithm.Errors.Count);
            Assert.AreEqual(LocationErrorCode.NotResolved, matrixAlgorithm.Errors[0].Code);
            Assert.AreEqual(LocationErrorCode.NotResolved, matrixAlgorithm.Errors[1].Code);
        }
    }
}