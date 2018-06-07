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

using System.Linq;
using Itinero.Optimization.Solvers.TSP;
using Xunit;

namespace Itinero.Optimization.Tests.Solvers.TSP
{
    public class BruteForceSolverTests
    {
        /// <summary>
        /// Tests the solver name.
        /// </summary>
        [Fact]
        public void BruteForceSolver_ShouldHaveBFAsName()
        {
            // create the solver.
            var solver = new BruteForceSolver();

            Assert.Equal("BF", solver.Name);
        }

        /// <summary>
        /// Tests the solver on a very small problem.
        /// </summary>
        [Fact]
        public void BruteForceSolver_ShouldWorkOnSingleSizedProblem()
        {
            // create a problem with only one customer.
            var problem = TSPHelper.CreateTSP(0, 0, 1, 10);

            // create the solver.
            var solver = new BruteForceSolver();
            var solution = solver.Search(problem).Solution;

            // verify solution.
            Assert.Equal(new [] { 0 }, solution.ToArray());
        }

        /// <summary>
        /// Tests the solver on a very small problem.
        /// </summary>
        [Fact]
        public void BruteForceSolver_ShouldWorkOnDoubleSizedProblem()
        {
            // create a problem with only one customer.
            var problem = TSPHelper.CreateTSP(0, 1, 2, 10);

            // create the solver.
            var solver = new BruteForceSolver();
            var solution = solver.Search(problem).Solution;

            // verify solution.
            Assert.Equal(new [] { 0, 1 }, solution.ToArray());
        }
        
        /// <summary>
        /// Tests the solver.
        /// </summary>
        [Fact]
        public void BruteForceSolver_ShouldSolvePerfectlyOnClosed()
        {
            // create the problem and make sure 0->1->2->3->4 is the solution.
            var problem = TSPHelper.CreateTSP(0, 0, 5, 10);
            problem._weights[0][1] = 1;
            problem._weights[1][2] = 1;
            problem._weights[2][3] = 1;
            problem._weights[3][4] = 1;
            problem._weights[4][0] = 1;

            // create the solver.
            var solver = new BruteForceSolver();
            var solution = solver.Search(problem).Solution;

            // verify solution.
            Assert.Equal(new [] { 0, 1, 2, 3, 4 }, solution.ToArray());
        }

        /// <summary>
        /// Tests the solver.
        /// </summary>
        [Fact]
        public void BruteForceSolver_ShouldSolvePerfectlyOnOpen()
        {
            // create the problem and make sure 0->1->2->3->4 is the solution.
            var problem = TSPHelper.CreateTSP(0, 5, 10);
            problem._weights[0][1] = 1;
            problem._weights[1][2] = 1;
            problem._weights[2][3] = 1;
            problem._weights[3][4] = 1;
            problem._weights[4][0] = 1;

            // create the solver.
            var solver = new BruteForceSolver();
            var solution = solver.Search(problem).Solution;

            // verify solution.
            Assert.Equal(new [] { 0, 1, 2, 3, 4 }, solution.ToArray());
        }

        /// <summary>
        /// Tests the solver.
        /// </summary>
        [Fact]
        public void BruteForceSolver_ShouldSolvePerfectlyOnFixed()
        {
            // create the problem and make sure 0->1->2->3->4 is the solution.
            var problem = TSPHelper.CreateTSP(0, 4, 5, 10);
            problem._weights[0][1] = 1;
            problem._weights[1][2] = 1;
            problem._weights[2][3] = 1;
            problem._weights[3][4] = 1;
            problem._weights[4][0] = 1;

            // create the solver.
            var solver = new BruteForceSolver();
            var solution = solver.Search(problem).Solution;

            // verify solution.
            Assert.Equal(new [] { 0, 1, 2, 3, 4 }, solution.ToArray());

            // create the problem and make sure 0->...->3 is the solution.
            problem = TSPHelper.CreateTSP(0, 3, 5, 10);
            problem._weights[0][1] = 1;
            problem._weights[1][2] = 1;
            problem._weights[2][3] = 1;
            problem._weights[3][4] = 1;
            problem._weights[4][0] = 1;

            // create the solver.
            solver = new BruteForceSolver();
            solution = solver.Search(problem).Solution;

            // verify solution.
            var array = solution.ToArray();
            Assert.Equal(0, array[0]);
            Assert.Equal(3, array[4]);
        }
    }
}