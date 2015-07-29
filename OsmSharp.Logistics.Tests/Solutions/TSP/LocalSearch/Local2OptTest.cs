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
using OsmSharp.Logistics.Solutions.TSP;
using OsmSharp.Logistics.Solutions.TSP.LocalSearch;
using System.Linq;

namespace OsmSharp.Logistics.Tests.Solutions.TSP.LocalSearch
{
    /// <summary>
    /// Contains tests for the local 2-Opt search operator.
    /// </summary>
    [TestFixture]
    public class Local2OptTests
    {
        /// <summary>
        /// Tests a route where only one shift is possible/needed.
        /// </summary>
        [Test]
        public void Test1OneSwitchClosed()
        {
            // create the problem and make sure 0->1->2->3->4 is the solution.
            var problem = TSPHelper.CreateTSP(0, 0, 5, 10);
            problem.Weights[0][1] = 1;
            problem.Weights[1][2] = 1;
            problem.Weights[2][3] = 1;
            problem.Weights[3][4] = 1;
            problem.Weights[4][0] = 1;

            // create a route with one shift.
            var route = new Route(new int[] { 0, 3, 2, 1, 4}, 0);

            // apply the 1-shift local search, it should find the customer to replocate.
            var localSearch = new Local2Opt();
            var delta = 0.0;
            localSearch.Apply(problem, new MinimumWeightObjective(), route, out delta);

            // test result.
            Assert.AreEqual(36, delta);
            Assert.AreEqual(new int[] { 0, 1, 2, 3, 4 }, route.ToArray());
        }
    }
}