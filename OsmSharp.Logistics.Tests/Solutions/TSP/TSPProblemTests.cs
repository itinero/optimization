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
            var weights = new double[2][];
            weights[0] = new double[] { 0, 1 };
            weights[1] = new double[] { 1, 0 };
            var first = 0;
            var last = 1;

            // create an open problem.
            var problem = new TSPProblem(first, weights);
            Assert.AreEqual(first, problem.First);
            Assert.IsNull(problem.Last);
            Assert.AreEqual(weights, problem.Weights);

            // create a closed problem.
            problem = new TSPProblem(first, first, weights);
            Assert.AreEqual(first, problem.First);
            Assert.AreEqual(first, problem.Last);
            Assert.AreEqual(weights, problem.Weights);

            // create a fixed problem.
            problem = new TSPProblem(first, last, weights);
            Assert.AreEqual(first, problem.First);
            Assert.AreEqual(last, problem.Last);
            Assert.AreEqual(weights, problem.Weights);
        }
    }
}