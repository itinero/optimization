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

using Itinero.Optimization.Algorithms.Directed;
using Itinero.Optimization.Algorithms.Random;
using Itinero.Optimization.TimeWindows;
using Itinero.Optimization.TSP.TimeWindows.Directed;
using Itinero.Optimization.TSP.TimeWindows.Directed.Solvers;
using NUnit.Framework;
using System.Collections.Generic;

namespace Itinero.Optimization.Test.TSP.TimeWindows.Directed.Solvers
{
    /// <summary>
    /// Containts tests for the VNS solver.
    /// </summary>
    [TestFixture]
    public class VNSSolverTests
    {
        /// <summary>
        /// Initializes for these tests.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            RandomGeneratorExtensions.GetGetNewRandom = () =>
            {
                return new RandomGenerator(4541247);
            };
        }

        /// <summary>
        /// Tests the solver on a problem with only 1 customer.
        /// </summary>
        [Test]
        public void TestSolution1()
        {
            // create problem.
            var problem = TSPTWHelper.CreateDirectedTSPTW(0, 0, 1, 0, 1);
            var objective = new TSPTWObjective();

            // create the solver.
            var solver = new VNSSolver();
            for (int i = 0; i < 10; i++)
            {
                // generate solution.
                float fitness;
                var solution = solver.Solve(problem, objective, out fitness);

                // test contents.
                Assert.IsTrue(fitness < 2);
                var solutionList = new List<int>(solution);
                Assert.AreEqual(0, DirectedHelper.ExtractId(solutionList[0]));
            }
        }

        /// <summary>
        /// Tests the solver.
        /// </summary>
        [Test]
        public void TestSolution5ClosedNotFixed()
        {
            // create problem.
            var problem = TSPTWHelper.CreateDirectedTSPTW(0, 0, 5, 10, 1);
            problem.Times.SetWeight(0, 1, 2);
            problem.Times.SetWeight(1, 2, 2);
            problem.Times.SetWeight(2, 3, 2);
            problem.Times.SetWeight(3, 4, 2);
            problem.Times.SetWeight(4, 0, 2);
            problem.Windows[2] = new TimeWindow()
            {
                Min = 3,
                Max = 5
            };
            problem.Windows[4] = new TimeWindow()
            {
                Min = 7,
                Max = 9
            };
            var objective = new TSPTWObjective();

            // create the solver.
            var solver = new VNSSolver();
            for (int i = 0; i < 10; i++)
            {
                // generate solution.
                float fitness;
                var solution = solver.Solve(problem, objective, out fitness);

                // test contents.
                Assert.AreEqual(10, fitness);
                var solutionList = new List<int>(solution);
                Assert.AreEqual(5, solutionList.Count);
            }
        }

        /// <summary>
        /// Tests the solver.
        /// </summary>
        [Test]
        public void TestSolution5ClosedFixed()
        {
            // create problem.
            var problem = TSPTWHelper.CreateDirectedTSPTW(0, 4, 5, 10, 1);
            problem.Times.SetWeight(0, 1, 2);
            problem.Times.SetWeight(1, 2, 2);
            problem.Times.SetWeight(2, 3, 2);
            problem.Times.SetWeight(3, 4, 2);
            problem.Times.SetWeight(4, 0, 2);
            problem.Windows[2] = new TimeWindow()
            {
                Min = 3,
                Max = 5
            };
            problem.Windows[4] = new TimeWindow()
            {
                Min = 7,
                Max = 9
            };
            var objective = new TSPTWObjective();

            // create the solver.
            var solver = new VNSSolver();
            for (int i = 0; i < 10; i++)
            {
                // generate solution.
                float fitness;
                var solution = solver.Solve(problem, objective, out fitness);

                // test contents.
                Assert.IsTrue(fitness <= 12);
                var solutionList = new List<int>();
                foreach (var directed in solution)
                {
                    solutionList.Add(DirectedHelper.ExtractId(directed));
                }
                Assert.AreEqual(5, solutionList.Count);
                Assert.AreEqual(0, solutionList[0]);
                Assert.AreEqual(4, solutionList[4]);
            }
        }

        /// <summary>
        /// Cleans up for these tests.
        /// </summary>
        [OneTimeTearDown]
        public void Dispose()
        {
            RandomGeneratorExtensions.Reset();
        }
    }
}
