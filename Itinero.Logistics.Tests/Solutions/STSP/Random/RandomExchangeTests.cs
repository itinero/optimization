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

using Itinero.Logistics.Solutions.STSP;
using Itinero.Logistics.Solutions.STSP.Random;
using NUnit.Framework;
using System.Linq;

namespace Itinero.Logistics.Tests.Solutions.STSP.Random
{
    /// <summary>
    /// Holds tests for the random exchange operator.
    /// </summary>
    [TestFixture]
    class RandomExchangeTests
    {
        /// <summary>
        /// Tests a route where only one shift is possible/needed.
        /// </summary>
        [Test]
        public void Test1RandomExchangedClosed()
        {
            // create the problem and a route.
            var problem = TSP.TSPHelper.CreateTSP(0, 0, 10, 10).ToSTSP(100);
            var route = new Logistics.Routes.Route(new int[] { 0, 2, 3, 1, 4 }, 0);

            // apply the 1-shift local search, it should find the customer to replocate.
            var op = new RandomExchange();
            var delta = 0.0;
            var r = op.Apply(problem, new MinimumWeightObjective(), route, out delta);

            // test result.
            if (r)
            {
                Assert.IsTrue(delta > 0);
            }
            else
            {
                Assert.IsTrue(delta < 0);
            }
        }
    }
}
