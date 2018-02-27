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

using Itinero.Optimization.Solutions.TSP;
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
            for (var x = 0; x < weights.Length; x++)
            {
                for (var y = 0; y < weights[x].Length; y++)
                {
                    Assert.AreEqual(weights[x][y], problem.Weight(x, y));
                }
            }

            // create a closed problem.
            problem = new TSProblem(first, first, weights);
            Assert.AreEqual(first, problem.First);
            Assert.AreEqual(first, problem.Last);
            for (var x = 0; x < weights.Length; x++)
            {
                for (var y = 0; y < weights[x].Length; y++)
                {
                    Assert.AreEqual(weights[x][y], problem.Weight(x, y));
                }
            }

            // create a fixed problem.
            problem = new TSProblem(first, last, weights);
            Assert.AreEqual(first, problem.First);
            Assert.AreEqual(last, problem.Last);
            for (var x = 0; x < weights.Length; x++)
            {
                for (var y = 0; y < weights[x].Length; y++)
                {
                    Assert.AreEqual(weights[x][y], problem.Weight(x, y));
                }
            }
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
            Assert.AreEqual(0, closedProblem.Weight(0, 0));
            Assert.AreEqual(1, closedProblem.Weight(0, 1));
            Assert.AreEqual(2, closedProblem.Weight(0, 2));
            Assert.AreEqual(3, closedProblem.Weight(0, 3));
            Assert.AreEqual(4, closedProblem.Weight(1, 0));
            Assert.AreEqual(0, closedProblem.Weight(1, 1));
            Assert.AreEqual(5, closedProblem.Weight(1, 2));
            Assert.AreEqual(6, closedProblem.Weight(1, 3));
            Assert.AreEqual(7, closedProblem.Weight(2, 0));
            Assert.AreEqual(8, closedProblem.Weight(2, 1));
            Assert.AreEqual(0, closedProblem.Weight(2, 2));
            Assert.AreEqual(9, closedProblem.Weight(2, 3));
            Assert.AreEqual(10, closedProblem.Weight(3, 0));
            Assert.AreEqual(11, closedProblem.Weight(3, 1));
            Assert.AreEqual(12, closedProblem.Weight(3, 2));
            Assert.AreEqual(0, closedProblem.Weight(3, 3));

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
            Assert.AreEqual(0, closedProblem.Weight(0, 0));
            Assert.AreEqual(1, closedProblem.Weight(0, 1));
            Assert.AreEqual(2, closedProblem.Weight(0, 2));
            Assert.AreEqual(3, closedProblem.Weight(0, 3));
            Assert.AreEqual(0, closedProblem.Weight(1, 0)); // all weights to first are zero.
            Assert.AreEqual(0, closedProblem.Weight(1, 1));
            Assert.AreEqual(5, closedProblem.Weight(1, 2));
            Assert.AreEqual(6, closedProblem.Weight(1, 3));
            Assert.AreEqual(0, closedProblem.Weight(2, 0)); // all weights to first are zero.
            Assert.AreEqual(8, closedProblem.Weight(2, 1));
            Assert.AreEqual(0, closedProblem.Weight(2, 2));
            Assert.AreEqual(9, closedProblem.Weight(2, 3));
            Assert.AreEqual(0, closedProblem.Weight(3, 0)); // all weights to first are zero.
            Assert.AreEqual(11, closedProblem.Weight(3, 1));
            Assert.AreEqual(12, closedProblem.Weight(3, 2));
            Assert.AreEqual(0, closedProblem.Weight(3, 3));

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
            //Assert.AreEqual(3, closedProblem.Weight.Length);
            //Assert.AreEqual(3, closedProblem.Weight(0].Length);
            //Assert.AreEqual(3, closedProblem.Weight(1].Length);
            Assert.AreEqual(0, closedProblem.Weight(0, 0));
            Assert.AreEqual(2, closedProblem.Weight(0, 1)); // this has to be old 0->2.
            Assert.AreEqual(3, closedProblem.Weight(0, 2)); // this has to be old 0->3
            Assert.AreEqual(8, closedProblem.Weight(1, 0)); // this has to be old 2->1.
            Assert.AreEqual(0, closedProblem.Weight(1, 1));
            Assert.AreEqual(9, closedProblem.Weight(1, 2)); // this has to old 2->3.
            Assert.AreEqual(11, closedProblem.Weight(2, 0)); // this has to be old 3->1
            Assert.AreEqual(12, closedProblem.Weight(2, 1)); // this has to be old 3->2.
            Assert.AreEqual(0, closedProblem.Weight(2, 2));
        }
    }
}