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
using OsmSharp.Logistics.Solutions.TSP;

namespace OsmSharp.Logistics.Tests.Solutions.TSP
{
    /// <summary>
    /// Contains tests for the TSP problem.
    /// </summary>
    [TestFixture]
    public class TSPProblemTests
    {
        /// <summary>
        /// Tests a TSP problem instance.
        /// </summary>
        [Test]
        public void Test()
        {
            var weights = new double[1][];
            var first = 0;

            // create an open problem.
            var problem = new TSPProblem(first, weights, false);
            Assert.AreEqual(first, problem.First);
            Assert.AreEqual(weights, problem.Weights);
            Assert.IsFalse(problem.IsClosed);

            // create a closed problem.
            problem = new TSPProblem(first, weights, true);
            Assert.AreEqual(first, problem.First);
            Assert.AreEqual(weights, problem.Weights);
            Assert.IsTrue(problem.IsClosed);
        }
    }
}