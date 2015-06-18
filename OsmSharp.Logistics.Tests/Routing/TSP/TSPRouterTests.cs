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
using OsmSharp.Logistics.Routes;
using OsmSharp.Logistics.Routing.TSP;
using OsmSharp.Logistics.Tests.Solutions.TSP;
using OsmSharp.Math.Random;
using OsmSharp.Routing.Vehicles;

namespace OsmSharp.Logistics.Tests.Routing.TSP
{
    /// <summary>
    /// Contains tests for the TSP-router.
    /// </summary>
    [TestFixture]
    public class TSPRouterTests
    {
        /// <summary>
        /// Tests a simple TSP.
        /// </summary>
        [Test]
        public void Test1TwoPoints()
        {
            StaticRandomGenerator.Set(4541247);

            // build test case.
            var router = new RouterMock();
            var tspSolver = new TSPSolverMock(Route.CreateFrom(new int[] { 0, 1 }), 10);
            var tspRouter = new TSPRouter(router, Vehicle.Car,
                new Math.Geo.GeoCoordinate[] { 
                    new Math.Geo.GeoCoordinate(0, 0),
                    new Math.Geo.GeoCoordinate(1, 1) }, true);

            // run.
            tspRouter.Run();

            Assert.IsNotNull(tspRouter);
            Assert.IsTrue(tspRouter.HasRun);
            Assert.IsTrue(tspRouter.HasSucceeded);
            Assert.AreEqual(0, tspRouter.Errors.Count);
            var route = tspRouter.BuildRoute();
            Assert.AreEqual(3, route.Segments.Length);
            Assert.AreEqual(0, route.Segments[0].Latitude);
            Assert.AreEqual(0, route.Segments[0].Longitude);
            Assert.AreEqual(1, route.Segments[1].Latitude);
            Assert.AreEqual(1, route.Segments[1].Longitude);
            Assert.AreEqual(0, route.Segments[0].Latitude);
            Assert.AreEqual(0, route.Segments[0].Longitude);
        }
    }
}