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
using Itinero.Optimization.Solutions.TSP.Solvers.Operators;
using NUnit.Framework;
using System.Linq;

namespace Itinero.Optimization.Test.TSP.Solvers.Operators
{

    /// <summary>
    /// Holds tests for the EAX operator.
    /// </summary>
    [TestFixture]
    public class EAXOperatorTests
    {
        /// <summary>
        /// Tests the EAX operator on a problem with 5 customers.
        /// </summary>
        [Test]
        public void Test5CrossOverFixed()
        {
            // create problem.
            var problem = TSPHelper.CreateTSP(0, 4, 5, 10);

            // create solutions.
            var solution1 = new Optimization.Tours.Tour(new int[] { 0, 1, 2, 3, 4 }, 4);
            var solution2 = new Optimization.Tours.Tour(new int[] { 0, 1, 3, 2, 4 }, 4);

            // execute crossover.
            var crossover = new EAXOperator();
            float fitness;
            var result = crossover.Apply(problem, new TSPObjective(), solution1, solution2, out fitness);

            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.Count);
        }

        /// <summary>
        /// Tests the EAX operator on a problem with 5 customers.
        /// </summary>
        [Test]
        public void Test5CrossOverClosed()
        {
            // create problem.
            var problem = TSPHelper.CreateTSP(0, 0, 5, 10);

            // create solutions.
            var solution1 = new Optimization.Tours.Tour(new int[] { 0, 1, 2, 3, 4 }, 0);
            var solution2 = new Optimization.Tours.Tour(new int[] { 0, 1, 3, 2, 4 }, 0);

            // execute crossover.
            var crossover = new EAXOperator();
            float fitness;
            var result = crossover.Apply(problem, new TSPObjective(), solution1, solution2, out fitness);

            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.Count);
        }

        /// <summary>
        /// Tests the EAX operator on a problem with 5 customers.
        /// </summary>
        [Test]
        public void Test5CrossOverOpen()
        {
            // create problem.
            var problem = TSPHelper.CreateTSP(0, 5, 10);

            // create solutions.
            var solution1 = new Optimization.Tours.Tour(new int[] { 0, 1, 2, 3, 4 }, null);
            var solution2 = new Optimization.Tours.Tour(new int[] { 0, 1, 3, 2, 4 }, null);

            // execute crossover.
            var crossover = new EAXOperator();
            float fitness;
            var result = crossover.Apply(problem, new TSPObjective(), solution1, solution2, out fitness);

            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.Count);
        }
    }
}