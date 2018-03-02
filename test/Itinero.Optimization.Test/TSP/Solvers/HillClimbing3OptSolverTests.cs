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

using Itinero.Optimization.Algorithms.Random;
using Itinero.Optimization.Abstract.Solvers.TSP;
using Itinero.Optimization.Abstract.Solvers.TSP.Solvers;
using NUnit.Framework;
using System.Linq;

namespace Itinero.Optimization.Test.TSP.Solvers
{
    /// <summary>
    /// Holds tests of the hill-climbing 3-Opt solver.
    /// </summary>
    [TestFixture]
    public class HillClimbing3OptSolverTests
    {
        /// <summary>
        /// Tests 3Opt hillclimbing solver.
        /// </summary>
        [Test]
        public void Test5Closed()
        {
            RandomGeneratorExtensions.GetGetNewRandom = () => new RandomGenerator(4541247);

            // create problem.
            var objective = new TSPObjective();
            var problem = TSPHelper.CreateTSP(0, 0, 5, 10);
            problem.Weights[0][1] = 1;
            problem.Weights[1][2] = 1;
            problem.Weights[2][3] = 1;
            problem.Weights[3][4] = 1;
            problem.Weights[4][0] = 1;

            // solve problem.
            var solver = new HillClimbing3OptSolver();
            var solution = solver.Solve(problem, objective);

            // check result.
            var last = solution.Last();
            Assert.AreEqual(0, solution.Last);
        }
    }
}