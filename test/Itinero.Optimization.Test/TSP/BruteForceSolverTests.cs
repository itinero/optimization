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

using Itinero.Optimization.Abstract.Solvers.TSP;
using Itinero.Optimization.Abstract.Solvers.TSP.Solvers;
using NUnit.Framework;
using System.Linq;

namespace Itinero.Optimization.Test.TSP
{
    /// <summary>
    /// Contains tests for the brute force solver.
    /// </summary>
    [TestFixture]
    public class BruteForceSolverTests
    {
        /// <summary>
        /// Tests the solver name.
        /// </summary>
        [Test]
        public void TestName()
        {
            // create the solver.
            var solver = new BruteForceSolver();

            Assert.AreEqual("BF", solver.Name);
        }

        /// <summary>
        /// Tests the solver on a very small problem.
        /// </summary>
        [Test]
        public void TestSolver1()
        {
            // create a problem with only one customer.
            var problem = TSPHelper.CreateTSP(0, 0, 1, 10);

            // create the solver.
            var solver = new BruteForceSolver();
            var solution = solver.Solve(problem, new TSPObjective());

            // verify solution.
            Assert.AreEqual(new int[] { 0 }, solution.ToArray());
        }

        /// <summary>
        /// Tests the solver on a very small problem.
        /// </summary>
        [Test]
        public void TestSolver2()
        {
            // create a problem with only one customer.
            var problem = TSPHelper.CreateTSP(0, 1, 2, 10);

            // create the solver.
            var solver = new BruteForceSolver();
            var solution = solver.Solve(problem, new TSPObjective());

            // verify solution.
            Assert.AreEqual(new int[] { 0, 1 }, solution.ToArray());
        }

        /// <summary>
        /// Tests the solver.
        /// </summary>
        [Test]
        public void TestSolverClosed()
        {
            // create the problem and make sure 0->1->2->3->4 is the solution.
            var problem = TSPHelper.CreateTSP(0, 0, 5, 10);
            problem.Weights[0][1] = 1;
            problem.Weights[1][2] = 1;
            problem.Weights[2][3] = 1;
            problem.Weights[3][4] = 1;
            problem.Weights[4][0] = 1;

            // create the solver.
            var solver = new BruteForceSolver();
            var solution = solver.Solve(problem, new TSPObjective());

            // verify solution.
            Assert.AreEqual(new int[] { 0, 1, 2, 3, 4 }, solution.ToArray());
        }

        /// <summary>
        /// Tests the solver.
        /// </summary>
        [Test]
        public void TestSolverOpen()
        {
            // create the problem and make sure 0->1->2->3->4 is the solution.
            var problem = TSPHelper.CreateTSP(0, 5, 10);
            problem.Weights[0][1] = 1;
            problem.Weights[1][2] = 1;
            problem.Weights[2][3] = 1;
            problem.Weights[3][4] = 1;
            problem.Weights[4][0] = 1;

            // create the solver.
            var solver = new BruteForceSolver();
            var solution = solver.Solve(problem, new TSPObjective());

            // verify solution.
            Assert.AreEqual(new int[] { 0, 1, 2, 3, 4 }, solution.ToArray());
        }

        /// <summary>
        /// Tests the solver.
        /// </summary>
        [Test]
        public void TestSolverFixed()
        {
            // create the problem and make sure 0->1->2->3->4 is the solution.
            var problem = TSPHelper.CreateTSP(0, 4, 5, 10);
            problem.Weights[0][1] = 1;
            problem.Weights[1][2] = 1;
            problem.Weights[2][3] = 1;
            problem.Weights[3][4] = 1;
            problem.Weights[4][0] = 1;

            // create the solver.
            var solver = new BruteForceSolver();
            var solution = solver.Solve(problem, new TSPObjective());

            // verify solution.
            Assert.AreEqual(new int[] { 0, 1, 2, 3, 4 }, solution.ToArray());

            // create the problem and make sure 0->...->3 is the solution.
            problem = TSPHelper.CreateTSP(0, 3, 5, 10);
            problem.Weights[0][1] = 1;
            problem.Weights[1][2] = 1;
            problem.Weights[2][3] = 1;
            problem.Weights[3][4] = 1;
            problem.Weights[4][0] = 1;

            // create the solver.
            solver = new BruteForceSolver();
            solution = solver.Solve(problem, new TSPObjective());

            // verify solution.
            var array = solution.ToArray();
            Assert.AreEqual(0, array[0]);
            Assert.AreEqual(3, array[4]);
        }
    }
}