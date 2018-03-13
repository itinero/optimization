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
using Itinero.Optimization.Abstract.Solvers.STSP;
using Itinero.Optimization.Abstract.Solvers.STSP.Solvers;
using NUnit.Framework;
using System.Collections.Generic;

namespace Itinero.Optimization.Test.STSP.Solvers
{
    /// <summary>
    /// Contains tests for the random TSP solver.
    /// </summary>
    [TestFixture]
    public class RandomSolverTests
    {
        /// <summary>
        /// Tests the solver on a 'open' s-tsp.
        /// </summary>
        [Test]
        public void TestSolutionOpen()
        {
            RandomGeneratorExtensions.GetGetNewRandom = () => new RandomGenerator(4541247);

            // create problem.
            var problem = STSPHelper.CreateSTSP(0, 5, 10, 40);

            // create the solver.
            var solver = new RandomSolver();
            var objective = new STSPObjective();

            for (var i = 0; i < 100; i++)
            {
                // generate solution.
                STSPFitness fitness;
                var solution = solver.Solve(problem, objective, out fitness);

                // test contents.
                Assert.AreEqual(problem.First, solution.First);
                Assert.AreEqual(null, solution.Last);
                Assert.AreEqual(solution.Count, fitness.Customers);
                Assert.IsTrue(40 >= fitness.Weight);
            }
        }

        /// <summary>
        /// Tests the solver on a 'open' but fixed s-tsp.
        /// </summary>
        [Test]
        public void TestSolutionOpenFixed()
        {
            RandomGeneratorExtensions.GetGetNewRandom = () => new RandomGenerator(4541247);

            // create problem.
            var problem = STSPHelper.CreateSTSP(0, 4, 5, 10, 40);

            // create the solver.
            var solver = new RandomSolver();
            var objective = new STSPObjective();

            for (var i = 0; i < 100; i++)
            {
                // generate solution.
                STSPFitness fitness;
                var solution = solver.Solve(problem, objective, out fitness);

                // test contents.
                Assert.AreEqual(problem.First, solution.First);
                Assert.AreEqual(problem.Last, solution.Last);
                Assert.AreEqual(solution.Count, fitness.Customers);
                Assert.IsTrue(40 >= fitness.Weight);
            }
        }

        /// <summary>
        /// Tests the solver on a 'closed' s-tsp.
        /// </summary>
        [Test]
        public void TestSolutionClosed()
        {
            RandomGeneratorExtensions.GetGetNewRandom = () => new RandomGenerator(4541247);

            // create problem.
            var problem = STSPHelper.CreateSTSP(0, 0, 5, 10, 40);

            // create the solver.
            var solver = new RandomSolver();
            var objective = new STSPObjective();

            for (var i = 0; i < 100; i++)
            {
                // generate solution.
                STSPFitness fitness;
                var solution = solver.Solve(problem, objective, out fitness);

                // test contents.
                Assert.AreEqual(problem.First, solution.First);
                Assert.AreEqual(problem.Last, solution.Last);
                Assert.AreEqual(solution.Count, fitness.Customers);
                Assert.IsTrue(40 >= fitness.Weight);
            }
        }
    }
}