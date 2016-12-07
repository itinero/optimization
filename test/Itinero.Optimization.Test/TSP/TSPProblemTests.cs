// Itinero.Optimization - Route optimization for .NET
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

using Itinero.Optimization.TSP;
using NUnit.Framework;

namespace Itinero.Optimization.Test.TSP
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
            var weights = new float[2][];
            weights[0] = new float[] { 0, 1 };
            weights[1] = new float[] { 1, 0 };
            var first = 0;
            var last = 1;

            // create an open problem.
            var problem = new TSProblem(first, weights);
            Assert.AreEqual(first, problem.First);
            Assert.IsNull(problem.Last);
            Assert.AreEqual(weights, problem.Weights);

            // create a closed problem.
            problem = new TSProblem(first, first, weights);
            Assert.AreEqual(first, problem.First);
            Assert.AreEqual(first, problem.Last);
            Assert.AreEqual(weights, problem.Weights);

            // create a fixed problem.
            problem = new TSProblem(first, last, weights);
            Assert.AreEqual(first, problem.First);
            Assert.AreEqual(last, problem.Last);
            Assert.AreEqual(weights, problem.Weights);
        }

        /// <summary>
        /// Tests the to-closed operation on ITSP.
        /// </summary>
        [Test]
        public void TestToClosed()
        {
            var weights = new float[][] {
                new float[] { 0, 1, 2, 3 },
                new float[] { 4, 0, 5, 6 },
                new float[] { 7, 8, 0, 9 },
                new float[] { 10, 11, 12, 0 }
            };

            // create a regular 'closed' problem with last->first and first==last.
            var problem = new TSProblem(0, 0, weights);
            var closedProblem = problem.ToClosed();
            Assert.AreEqual(0, closedProblem.First);
            Assert.IsTrue(closedProblem.Last.HasValue);
            Assert.AreEqual(0, closedProblem.Last.Value);
            Assert.AreEqual(0, closedProblem.Weights[0][0]);
            Assert.AreEqual(1, closedProblem.Weights[0][1]);
            Assert.AreEqual(2, closedProblem.Weights[0][2]);
            Assert.AreEqual(3, closedProblem.Weights[0][3]);
            Assert.AreEqual(4, closedProblem.Weights[1][0]);
            Assert.AreEqual(0, closedProblem.Weights[1][1]);
            Assert.AreEqual(5, closedProblem.Weights[1][2]);
            Assert.AreEqual(6, closedProblem.Weights[1][3]);
            Assert.AreEqual(7, closedProblem.Weights[2][0]);
            Assert.AreEqual(8, closedProblem.Weights[2][1]);
            Assert.AreEqual(0, closedProblem.Weights[2][2]);
            Assert.AreEqual(9, closedProblem.Weights[2][3]);
            Assert.AreEqual(10, closedProblem.Weights[3][0]);
            Assert.AreEqual(11, closedProblem.Weights[3][1]);
            Assert.AreEqual(12, closedProblem.Weights[3][2]);
            Assert.AreEqual(0, closedProblem.Weights[3][3]);

            weights = new float[][] {
                new float[] { 0, 1, 2, 3 },
                new float[] { 4, 0, 5, 6 },
                new float[] { 7, 8, 0, 9 },
                new float[] { 10, 11, 12, 0 }
            };

            // create a regular 'open' problem with last not set and no link between last->first.
            problem = new TSProblem(0, weights);
            closedProblem = problem.ToClosed();
            Assert.AreEqual(0, closedProblem.First);
            Assert.IsTrue(closedProblem.Last.HasValue);
            Assert.AreEqual(0, closedProblem.Last.Value);
            Assert.AreEqual(0, closedProblem.Weights[0][0]);
            Assert.AreEqual(1, closedProblem.Weights[0][1]);
            Assert.AreEqual(2, closedProblem.Weights[0][2]);
            Assert.AreEqual(3, closedProblem.Weights[0][3]);
            Assert.AreEqual(0, closedProblem.Weights[1][0]); // all weights to first are zero.
            Assert.AreEqual(0, closedProblem.Weights[1][1]);
            Assert.AreEqual(5, closedProblem.Weights[1][2]);
            Assert.AreEqual(6, closedProblem.Weights[1][3]);
            Assert.AreEqual(0, closedProblem.Weights[2][0]); // all weights to first are zero.
            Assert.AreEqual(8, closedProblem.Weights[2][1]);
            Assert.AreEqual(0, closedProblem.Weights[2][2]);
            Assert.AreEqual(9, closedProblem.Weights[2][3]);
            Assert.AreEqual(0, closedProblem.Weights[3][0]); // all weights to first are zero.
            Assert.AreEqual(11, closedProblem.Weights[3][1]);
            Assert.AreEqual(12, closedProblem.Weights[3][2]);
            Assert.AreEqual(0, closedProblem.Weights[3][3]);

            weights = new float[][] {
                new float[] { 0, 1, 2, 3 },
                new float[] { 4, 0, 5, 6 },
                new float[] { 7, 8, 0, 9 },
                new float[] { 10, 11, 12, 0 }
            };

            // create a regular 'open' problem with last fixed and no link between last->first.
            problem = new TSProblem(0, 1, weights);
            closedProblem = problem.ToClosed();
            Assert.AreEqual(0, closedProblem.First);
            Assert.IsTrue(closedProblem.Last.HasValue);
            Assert.AreEqual(0, closedProblem.Last.Value);
            Assert.AreEqual(3, closedProblem.Weights.Length);
            Assert.AreEqual(3, closedProblem.Weights[0].Length);
            Assert.AreEqual(3, closedProblem.Weights[1].Length);
            Assert.AreEqual(0, closedProblem.Weights[0][0]);
            Assert.AreEqual(2, closedProblem.Weights[0][1]); // this has to be old 0->2.
            Assert.AreEqual(3, closedProblem.Weights[0][2]); // this has to be old 0->3
            Assert.AreEqual(8, closedProblem.Weights[1][0]); // this has to be old 2->1.
            Assert.AreEqual(0, closedProblem.Weights[1][1]);
            Assert.AreEqual(9, closedProblem.Weights[1][2]); // this has to old 2->3.
            Assert.AreEqual(11, closedProblem.Weights[2][0]); // this has to be old 3->1
            Assert.AreEqual(12, closedProblem.Weights[2][1]); // this has to be old 3->2.
            Assert.AreEqual(0, closedProblem.Weights[2][2]);
        }
    }
}