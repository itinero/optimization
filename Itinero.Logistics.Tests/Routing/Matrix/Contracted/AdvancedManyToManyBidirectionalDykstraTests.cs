// Itinero - OpenStreetMap (OSM) SDK
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

using Itinero.Data.Network;
using Itinero.Logistics.Routing.Matrix.Contracted;
using Itinero.Logistics.Tests.Mocks;
using NUnit.Framework;

namespace Itinero.Logistics.Tests.Routing.Matrix.Contracted
{
    /// <summary>
    /// Contains test for the advanced many-to-many bidirectional dykstra algorithm.
    /// </summary>
    [TestFixture]
    public class AdvancedManyToManyBidirectionalDykstraTests
    {
        /// <summary>
        /// Tests many-to-many path calculations on just one edge.
        /// </summary>
        /// <remarks>
        /// Situation:
        ///  (0)---100m----(1) @ 100km/h
        /// </remarks>
        [Test]
        public void TestOneEdge()
        {
            // build graph.
            var routerDb = new RouterDb();
            routerDb.AddSupportedProfile(MockProfile.CarMock());
            routerDb.Network.AddVertex(0, 0, 0);
            routerDb.Network.AddVertex(1, 0, 0);
            routerDb.Network.AddEdge(0, 1, new Itinero.Data.Network.Edges.EdgeData()
            {
                Distance = 100,
                Profile = 0,
                MetaId = 0
            });
            routerDb.AddContracted(MockProfile.CarMock(), true);

            // create algorithm and run.
            var algorithm = new AdvancedManyToManyBidirectionalDykstra(new Router(routerDb), MockProfile.CarMock(),
                new RouterPoint[] { new RouterPoint(0, 0, 0, 0), new RouterPoint(1, 1, 0, ushort.MaxValue) });
            algorithm.Run();

            // check results.
            Assert.IsTrue(algorithm.HasRun);
            Assert.IsTrue(algorithm.HasSucceeded);

            Assert.IsNotNull(algorithm.Weights);
            Assert.AreEqual(4, algorithm.Weights.Length);
            Assert.AreEqual(4, algorithm.Weights[0].Length);
            Assert.AreEqual(MockProfile.CarMock().Factor(null).Value * 100, algorithm.Weights[0][2], 0.01);
        }

        /// <summary>
        /// Tests many to many calculations between vertices on a triangle.
        /// </summary>
        /// <remarks>
        /// Situation:
        ///  (0)----100m----(1)
        ///   \             /
        ///    \           /           
        ///     \         /
        ///     100m    100m
        ///       \     /
        ///        \   /
        ///         (2)
        /// 
        /// Result:
        /// 
        ///     [  0,100,100]
        ///     [100,  0,100]
        ///     [100,100,  0]
        ///     
        /// </remarks>
        [Test]
        public void TestThreeEdges()
        {
            // build graph.
            var routerDb = new RouterDb();
            routerDb.AddSupportedProfile(MockProfile.CarMock());
            routerDb.Network.AddVertex(0, 0, 0);
            routerDb.Network.AddVertex(1, 1, 1);
            routerDb.Network.AddVertex(2, 2, 2);
            routerDb.Network.AddEdge(0, 1, new Itinero.Data.Network.Edges.EdgeData()
            {
                Distance = 100,
                Profile = 0,
                MetaId = 0
            });
            routerDb.Network.AddEdge(1, 2, new Itinero.Data.Network.Edges.EdgeData()
            {
                Distance = 100,
                Profile = 0,
                MetaId = 0
            });
            routerDb.Network.AddEdge(2, 0, new Itinero.Data.Network.Edges.EdgeData()
            {
                Distance = 100,
                Profile = 0,
                MetaId = 0
            });
            routerDb.AddContracted(MockProfile.CarMock(), true);

            // run algorithm (0, 1, 2)->(0, 1, 2).
            var algorithm = new AdvancedManyToManyBidirectionalDykstra(new Router(routerDb), MockProfile.CarMock(),
                new RouterPoint[] {
                    routerDb.Network.CreateRouterPointForVertex(0),
                    routerDb.Network.CreateRouterPointForVertex(1),
                    routerDb.Network.CreateRouterPointForVertex(2)
                });
            algorithm.Run();

            Assert.IsTrue(algorithm.HasRun);
            Assert.IsTrue(algorithm.HasSucceeded);

            var weights = algorithm.Weights;
            Assert.IsNotNull(weights);
            Assert.AreEqual(6, weights.Length);

            Assert.AreEqual(6, weights[0].Length);
            Assert.AreEqual(0, weights[0][0], 0.001);
            Assert.AreEqual(300 * MockProfile.CarMock().Factor(null).Value, weights[0][1], 0.1);
            Assert.AreEqual(100 * MockProfile.CarMock().Factor(null).Value, weights[0][2], 0.1);
            Assert.AreEqual(200 * MockProfile.CarMock().Factor(null).Value, weights[0][3], 0.1);
            Assert.AreEqual(300 * MockProfile.CarMock().Factor(null).Value, weights[0][4], 0.1);
            Assert.AreEqual(100 * MockProfile.CarMock().Factor(null).Value, weights[0][5], 0.1);

            Assert.AreEqual(6, weights[1].Length);
            Assert.AreEqual(300 * MockProfile.CarMock().Factor(null).Value, weights[1][0], 0.1);
            Assert.AreEqual(200 * MockProfile.CarMock().Factor(null).Value, weights[1][1], 0.1);
            Assert.AreEqual(400 * MockProfile.CarMock().Factor(null).Value, weights[1][2], 0.1);
            Assert.AreEqual(100 * MockProfile.CarMock().Factor(null).Value, weights[1][3], 0.1);
            Assert.AreEqual(200 * MockProfile.CarMock().Factor(null).Value, weights[1][4], 0.1);
            Assert.AreEqual(200 * MockProfile.CarMock().Factor(null).Value, weights[1][5], 0.1);

            Assert.AreEqual(6, weights[2].Length);
            Assert.AreEqual(100 * MockProfile.CarMock().Factor(null).Value, weights[2][0], 0.1);
            Assert.AreEqual(400 * MockProfile.CarMock().Factor(null).Value, weights[2][1], 0.1);
            Assert.AreEqual(200 * MockProfile.CarMock().Factor(null).Value, weights[2][2], 0.1);
            Assert.AreEqual(300 * MockProfile.CarMock().Factor(null).Value, weights[2][3], 0.1);
            Assert.AreEqual(400 * MockProfile.CarMock().Factor(null).Value, weights[2][4], 0.1);
            Assert.AreEqual(200 * MockProfile.CarMock().Factor(null).Value, weights[2][5], 0.1);
        }
    }
}