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
using Itinero.Optimization.Abstract.Solvers.TSP.Directed;
using Itinero.Optimization.Abstract.Solvers.TSP.Directed.Solvers;
using NUnit.Framework;
using System.Collections.Generic;

namespace Itinero.Optimization.Test.TSP.Directed.Solvers
{
    [TestFixture]
    public class HillClimbing3OptSolverTests
    {
        /// <summary>
        /// Tests the solver with only 1 customer.
        /// </summary>
        [Test]
        public void TestSolution1()
        {
            RandomGeneratorExtensions.GetGetNewRandom = () => new RandomGenerator(4541247);

            // create problem.
            var problem = TSPHelper.CreateDirectedTSP(0, 0, 1, 0, 1);

            // create the solver.
            var solver = new HillClimbing3OptSolver();

            for (int i = 0; i < 10; i++)
            {
                // generate solution.
                float fitness;
                var solution = solver.Solve(problem, new TSPObjective(), out fitness);

                // test contents.
                Assert.AreEqual(0, fitness);
                var solutionList = new List<int>(solution);
                Assert.AreEqual(0, DirectedHelper.ExtractId(solutionList[0]));
                Assert.AreEqual(1, solutionList.Count);
            }
        }

        /// <summary>
        /// Tests the solver with only 2 customers.
        /// </summary>
        [Test]
        public void TestSolution2()
        {
            RandomGeneratorExtensions.GetGetNewRandom = () => new RandomGenerator(4541247);

            // create problem.
            var problem = TSPHelper.CreateDirectedTSP(0, 2, 10, 1);
            problem.Weights.SetWeight(0, 1, 1);
            problem.Weights.SetWeight(1, 0, 1);

            // create the solver.
            var solver = new HillClimbing3OptSolver();

            for (int i = 0; i < 10; i++)
            {
                // generate solution.
                float fitness;
                var solution = solver.Solve(problem, new TSPObjective(), out fitness);

                // test contents.
                Assert.AreEqual(1, fitness);
                var solutionList = new List<int>(solution);
                Assert.AreEqual(0, DirectedHelper.ExtractId(solutionList[0]));
                Assert.AreEqual(1, DirectedHelper.ExtractId(solutionList[1]));
                Assert.AreEqual(2, solutionList.Count);
            }
        }

        /// <summary>
        /// Tests the solver with a non-fixed ending.
        /// </summary>
        [Test]
        public void TestSolutions5NotClosedNotFixed()
        {
            RandomGeneratorExtensions.GetGetNewRandom = () => new RandomGenerator(4541247);

            // create problem.
            var problem = TSPHelper.CreateDirectedTSP(0, 5, 10, 1);
            problem.Weights.SetWeight(0, 1, 1);
            problem.Weights.SetWeight(1, 2, 1);
            problem.Weights.SetWeight(2, 3, 1);
            problem.Weights.SetWeight(3, 4, 1);
            problem.Weights.SetWeight(4, 0, 1);

            // create the solver.
            var solver = new HillClimbing3OptSolver();

            for (int i = 0; i < 10; i++)
            {
                // generate solution.
                float fitness;
                var solution = solver.Solve(problem, new TSPObjective(), out fitness);

                // test contents.
                Assert.IsTrue(fitness <= 7);
                var solutionList = new List<int>(solution);
                Assert.AreEqual(0, DirectedHelper.ExtractId(solutionList[0]));
                Assert.AreEqual(5, solutionList.Count);
            }
        }

        // TODO: fix this option in this solver or implement another solver.
        ///// <summary>
        ///// Tests the solver on an 'open' solution but with a fixed end.
        ///// </summary>
        //[Test]
        //public void TestSolutions5NotClosedFixed()
        //{
        //    RandomGeneratorExtensions.GetGetNewRandom = () => new RandomGenerator(4541247);

        //    // create problem.
        //    var problem = TSPHelper.CreateDirectedTSP(0, 4, 5, 10);
        //    problem.Weights[0][1] = 1;
        //    problem.Weights[1][2] = 1;
        //    problem.Weights[2][3] = 1;
        //    problem.Weights[3][4] = 1;
        //    problem.Weights[4][0] = 1;

        //    // create the solver.
        //    var solver = new HillClimbing3OptSolver();

        //    for (int i = 0; i < 10; i++)
        //    {
        //        // generate solution.
        //        float fitness;
        //        var solution = solver.Solve(problem, new TSPObjective(), out fitness);

        //        // test contents.
        //        Assert.AreEqual(4, fitness);
        //        var solutionList = new List<int>(solution);
        //        Assert.AreEqual(0, solutionList[0]);
        //        Assert.AreEqual(4, solutionList[4]);
        //        Assert.IsTrue(solutionList.Remove(0));
        //        Assert.IsTrue(solutionList.Remove(1));
        //        Assert.IsTrue(solutionList.Remove(2));
        //        Assert.IsTrue(solutionList.Remove(3));
        //        Assert.IsTrue(solutionList.Remove(4));
        //        Assert.AreEqual(0, solutionList.Count);
        //    }
        //}


        /// <summary>
        /// Tests the solver.
        /// </summary>
        [Test]
        public void TestSolutions5ClosedNotFixed()
        {
            RandomGeneratorExtensions.GetGetNewRandom = () => new RandomGenerator(4541247);

            // create problem.
            var problem = TSPHelper.CreateDirectedTSP(0, 0, 5, 10, 1);
            problem.Weights.SetWeight(0, 1, 1);
            problem.Weights.SetWeight(1, 2, 1);
            problem.Weights.SetWeight(2, 3, 1);
            problem.Weights.SetWeight(3, 4, 1);
            problem.Weights.SetWeight(4, 0, 1);

            // create the solver.
            var solver = new HillClimbing3OptSolver();

            for (int i = 0; i < 10; i++)
            {
                // generate solution.
                float fitness;
                var solution = solver.Solve(problem, new TSPObjective(), out fitness);

                // test contents.
                Assert.IsTrue(fitness <= 10);
                var solutionList = new List<int>(solution);
                Assert.AreEqual(0, DirectedHelper.ExtractId(solutionList[0]));
                Assert.IsNotNull(solution.Last);
                Assert.AreEqual(0, DirectedHelper.ExtractId(solution.Last.Value));
                Assert.AreEqual(5, solutionList.Count);
            }
        }

    }
}
