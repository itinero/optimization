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

using Itinero.Optimization.Solutions.TSP.Directed.Solvers;
using Itinero.Optimization.Solutions.TSP.Directed.Solvers.Operators;
using NUnit.Framework;

namespace Itinero.Optimization.Test.TSP.Directed.Solvers.Operators
{
    /// <summary>
    /// Contains tests for the direction local search operator.
    /// </summary>
    [TestFixture]
    public class DirectionLocalSearchTest
    {
        /// <summary>
        /// Tests on an open route.
        /// </summary>
        [Test]
        public void TestOpen()
        {
            var problem = TSPHelper.CreateDirectedTSP(0, 10, 100, 5);
            var random = new RandomSolver();
            var randomRoute = random.Solve(problem, new Optimization.Solutions.TSP.Directed.TSPObjective());

            var op = new DirectionLocalSearch();
            float delta;
            var optimized = op.Apply(problem, new Optimization.Solutions.TSP.Directed.TSPObjective(), randomRoute, out delta);


        }
    }
}