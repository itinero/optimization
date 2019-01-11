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
            Assert.Equal("BF", BruteForceSolver.Default.Name);
        }

        /// <summary>
        /// Tests the solver on a very small problem.
        /// </summary>
        [Fact]
        public void BruteForceSolver_ShouldWorkOnSingleSizedProblem()
        {
            // create a problem with only one customer.
            var problem = new TSProblem(0, 0, WeightMatrixHelpers.Build(1, 10));

            // create the solver.
            var solution = BruteForceSolver.Default.Search(problem).Solution;

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
            var problem = new TSProblem(0, 1, WeightMatrixHelpers.Build(2, 10));

            // create the solver.
            var solution = BruteForceSolver.Default.Search(problem).Solution;

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
            var weights = WeightMatrixHelpers.Build(5, 10);
            weights[0][1] = 1;
            weights[1][2] = 1;
            weights[2][3] = 1;
            weights[3][4] = 1;
            weights[4][0] = 1;
            var problem = new TSProblem(0, 0, weights);
            
            // create the solver.
            var solution = BruteForceSolver.Default.Search(problem).Solution;

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
            var weights = WeightMatrixHelpers.Build(5, 10);
            weights[0][1] = 1;
            weights[1][2] = 1;
            weights[2][3] = 1;
            weights[3][4] = 1;
            weights[4][0] = 1;
            var problem = new TSProblem(0, null, weights);

            // create the solver.
            var solution = BruteForceSolver.Default.Search(problem).Solution;

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
            var weights = WeightMatrixHelpers.Build(5, 10);
            weights[0][1] = 1;
            weights[1][2] = 1;
            weights[2][3] = 1;
            weights[3][4] = 1;
            weights[4][0] = 1;
            var problem = new TSProblem(0, 4, weights);

            // create the solver.
            var solution = BruteForceSolver.Default.Search(problem).Solution;

            // verify solution.
            Assert.Equal(new [] { 0, 1, 2, 3, 4 }, solution.ToArray());
            
            // create the problem and make sure 0->1->2->3->4 is the solution.
            weights = WeightMatrixHelpers.Build(5, 10);
            weights[0][1] = 1;
            weights[1][2] = 1;
            weights[2][3] = 1;
            weights[3][4] = 1;
            weights[4][0] = 1;
            problem = new TSProblem(0, 3, weights);

            // create the solver.
            solution = BruteForceSolver.Default.Search(problem).Solution;

            // verify solution.
            var array = solution.ToArray();
            Assert.Equal(0, array[0]);
            Assert.Equal(3, array[4]);
        }

        /// <summary>
        /// Tests the solver.
        /// </summary>
        [Fact]
        public void BruteForceSolver_ShouldUseOnlyProvidedVisits()
        {
            // create the problem and make sure 0->2->4->6->8 is the solution.
            var weights = WeightMatrixHelpers.Build(10, 10);
            weights[0][2] = 1;
            weights[2][4] = 1;
            weights[4][6] = 1;
            weights[6][8] = 1;
            weights[8][0] = 1;
            var problem = new TSProblem(0, null, weights,
                new int[] { 0, 2, 4, 6, 8});
            
            // create the solver.
            var solution = BruteForceSolver.Default.Search(problem).Solution;

            // verify solution.
            Assert.Equal(new [] { 0, 2, 4, 6, 8 }, solution.ToArray());
        }
    }
}