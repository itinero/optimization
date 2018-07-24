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

using System.Collections.Generic;
using Itinero.Optimization.Solvers.TSP;
using Itinero.Optimization.Solvers.TSP.EAX;
using Xunit;

namespace Itinero.Optimization.Tests.Solvers.TSP.EAX
{
    public class EAXSolverTests
    {
        /// <summary>
        /// Tests the solver with only 1 customer.
        /// </summary>
        [Fact]
        public void EAXSolver_SolvingSize1ProblemShouldHaveSize1Result()
        {
            // create problem.
            var problem = new TSProblem(0, 0, WeightMatrixHelpers.Build(1, 0));

            for (var i = 0; i < 10; i++)
            {
                // generate solution.
                var candidate = EAXSolver.Default.Search(problem);

                // test contents.
                Assert.Equal(0, candidate.Fitness);
                var solutionList = new List<int>(candidate.Solution);
                Assert.Equal(0, solutionList[0]);
                Assert.True(solutionList.Remove(0));
                Assert.Empty(solutionList);
            }
        }

        /// <summary>
        /// Tests the solver with only 2 customers.
        /// </summary>
        [Fact]
        public void EAXSolver_SolvingSize2ProblemShouldBePerfect()
        {
            // create problem.
            var weights = WeightMatrixHelpers.Build(2, 10);
            weights[0][1] = 1;
            weights[1][0] = 1;
            var problem = new TSProblem(0, 1, weights);

            for (var i = 0; i < 10; i++)
            {
                // generate solution.
                var candidate = EAXSolver.Default.Search(problem);

                // test contents.
                Assert.Equal(1, candidate.Fitness);
                var solutionList = new List<int>(candidate.Solution);
                Assert.Equal(0, solutionList[0]);
                Assert.True(solutionList.Remove(0));
                Assert.True(solutionList.Remove(1));
                Assert.Empty(solutionList);
            }
        }
        
        /// <summary>
        /// Tests the solver on open test problem 'BR17'.
        /// </summary>
        [Fact]
        public void EAXSolver_ShouldSolveBR17OpenToPerfection()
        {
            var weights = new float[][] {
                new float[] {9999,   3,   5,  48,  48,   8,   8,   5,   5,   3,   3,   0,   3,   5,   8,   8,   5},
                new float[] {   3,9999,   3,  48,  48,   8,   8,   5,   5,   0,   0,   3,   0,   3,   8,   8,   5},
                new float[] {   5,   3,9999,  72,  72,  48,  48,  24,  24,   3,   3,   5,   3,   0,  48,  48,  24},
                new float[] {  48,  48,  74,9999,   0,   6,   6,  12,  12,  48,  48,  48,  48,  74,   6,   6,  12},
                new float[] {  48,  48,  74,   0,9999,   6,   6,  12,  12,  48,  48,  48,  48,  74,   6,   6,  12},
                new float[] {   8,   8,  50,   6,   6,9999,   0,   8,   8,   8,   8,   8,   8,  50,   0,   0,   8},
                new float[] {   8,   8,  50,   6,   6,   0,9999,   8,   8,   8,   8,   8,   8,  50,   0,   0,   8},
                new float[] {   5,   5,  26,  12,  12,   8,   8,9999,   0,   5,   5,   5,   5,  26,   8,   8,   0},
                new float[] {   5,   5,  26,  12,  12,   8,   8,   0,9999,   5,   5,   5,   5,  26,   8,   8,   0},
                new float[] {   3,   0,   3,  48,  48,   8,   8,   5,   5,9999,   0,   3,   0,   3,   8,   8,   5},
                new float[] {   3,   0,   3,  48,  48,   8,   8,   5,   5,   0,9999,   3,   0,   3,   8,   8,   5},
                new float[] {   0,   3,   5,  48,  48,   8,   8,   5,   5,   3,   3,9999,   3,   5,   8,   8,   5},
                new float[] {   3,   0,   3,  48,  48,   8,   8,   5,   5,   0,   0,   3,9999,   3,   8,   8,   5},
                new float[] {   5,   3,   0,  72,  72,  48,  48,  24,  24,   3,   3,   5,   3,9999,  48,  48,  24},
                new float[] {   8,   8,  50,   6,   6,   0,   0,   8,   8,   8,   8,   8,   8,  50,9999,   0,   8},
                new float[] {   8,   8,  50,   6,   6,   0,   0,   8,   8,   8,   8,   8,   8,  50,   0,9999,   8},
                new float[] {   5,   5,  26,  12,  12,   8,   8,   0,   0,   5,   5,   5,   5,  26,   8,   8,9999}};

            // create problem.
            var problem = new TSProblem(0, 16, weights);

            for (var i = 0; i < 10; i++)
            {
                // generate solution.
                var candidate = EAXSolver.Default.Search(problem);

                // test contents.
                Assert.Equal(0, candidate.Solution.First);
                Assert.Equal(16, candidate.Solution.Last);
                Assert.Equal(34, candidate.Fitness);
                var solutionList = new List<int>(candidate.Solution);
                Assert.Equal(0, solutionList[0]);
                Assert.True(solutionList.Remove(0));
                Assert.True(solutionList.Remove(1));
                Assert.True(solutionList.Remove(2));
                Assert.True(solutionList.Remove(3));
                Assert.True(solutionList.Remove(4));
                Assert.True(solutionList.Remove(5));
                Assert.True(solutionList.Remove(6));
                Assert.True(solutionList.Remove(7));
                Assert.True(solutionList.Remove(8));
                Assert.True(solutionList.Remove(9));
                Assert.True(solutionList.Remove(10));
                Assert.True(solutionList.Remove(11));
                Assert.True(solutionList.Remove(12));
                Assert.True(solutionList.Remove(13));
                Assert.True(solutionList.Remove(14));
                Assert.True(solutionList.Remove(15));
                Assert.True(solutionList.Remove(16));
                Assert.Empty(solutionList);
            }
        }
        
        /// <summary>
        /// Tests the solver on closed test problem 'BR17'.
        /// </summary>
        [Fact]
        public void EAXSolver_ShouldSolveBR17ClosedToPerfection()
        {
            var weights = new float[][] {
                new float[] {9999,   3,   5,  48,  48,   8,   8,   5,   5,   3,   3,   0,   3,   5,   8,   8,   5},
                new float[] {   3,9999,   3,  48,  48,   8,   8,   5,   5,   0,   0,   3,   0,   3,   8,   8,   5},
                new float[] {   5,   3,9999,  72,  72,  48,  48,  24,  24,   3,   3,   5,   3,   0,  48,  48,  24},
                new float[] {  48,  48,  74,9999,   0,   6,   6,  12,  12,  48,  48,  48,  48,  74,   6,   6,  12},
                new float[] {  48,  48,  74,   0,9999,   6,   6,  12,  12,  48,  48,  48,  48,  74,   6,   6,  12},
                new float[] {   8,   8,  50,   6,   6,9999,   0,   8,   8,   8,   8,   8,   8,  50,   0,   0,   8},
                new float[] {   8,   8,  50,   6,   6,   0,9999,   8,   8,   8,   8,   8,   8,  50,   0,   0,   8},
                new float[] {   5,   5,  26,  12,  12,   8,   8,9999,   0,   5,   5,   5,   5,  26,   8,   8,   0},
                new float[] {   5,   5,  26,  12,  12,   8,   8,   0,9999,   5,   5,   5,   5,  26,   8,   8,   0},
                new float[] {   3,   0,   3,  48,  48,   8,   8,   5,   5,9999,   0,   3,   0,   3,   8,   8,   5},
                new float[] {   3,   0,   3,  48,  48,   8,   8,   5,   5,   0,9999,   3,   0,   3,   8,   8,   5},
                new float[] {   0,   3,   5,  48,  48,   8,   8,   5,   5,   3,   3,9999,   3,   5,   8,   8,   5},
                new float[] {   3,   0,   3,  48,  48,   8,   8,   5,   5,   0,   0,   3,9999,   3,   8,   8,   5},
                new float[] {   5,   3,   0,  72,  72,  48,  48,  24,  24,   3,   3,   5,   3,9999,  48,  48,  24},
                new float[] {   8,   8,  50,   6,   6,   0,   0,   8,   8,   8,   8,   8,   8,  50,9999,   0,   8},
                new float[] {   8,   8,  50,   6,   6,   0,   0,   8,   8,   8,   8,   8,   8,  50,   0,9999,   8},
                new float[] {   5,   5,  26,  12,  12,   8,   8,   0,   0,   5,   5,   5,   5,  26,   8,   8,9999}};

            // create problem.
            var problem = new TSProblem(0, 0, weights);

            for (var i = 0; i < 10; i++)
            {
                // generate solution.
                var candidate = EAXSolver.Default.Search(problem);

                // test contents.
                Assert.Equal(0, candidate.Solution.First);
                Assert.Equal(0, candidate.Solution.Last);
                Assert.Equal(39, candidate.Fitness);
                var solutionList = new List<int>(candidate.Solution);
                Assert.Equal(0, solutionList[0]);
                Assert.True(solutionList.Remove(0));
                Assert.True(solutionList.Remove(1));
                Assert.True(solutionList.Remove(2));
                Assert.True(solutionList.Remove(3));
                Assert.True(solutionList.Remove(4));
                Assert.True(solutionList.Remove(5));
                Assert.True(solutionList.Remove(6));
                Assert.True(solutionList.Remove(7));
                Assert.True(solutionList.Remove(8));
                Assert.True(solutionList.Remove(9));
                Assert.True(solutionList.Remove(10));
                Assert.True(solutionList.Remove(11));
                Assert.True(solutionList.Remove(12));
                Assert.True(solutionList.Remove(13));
                Assert.True(solutionList.Remove(14));
                Assert.True(solutionList.Remove(15));
                Assert.True(solutionList.Remove(16));
                Assert.Empty(solutionList);
            }
        }

        /// <summary>
        /// Tests the solver.
        /// </summary>
        [Fact]
        public void EAXSolver_ShouldUseOnlyProvidedVisits()
        {
            // create the problem and make sure 0->2->4->6->8 is the solution.
            var weights = WeightMatrixHelpers.Build(10, 10);
            weights[0][2] = 1;
            weights[2][4] = 1;
            weights[4][6] = 1;
            weights[6][8] = 1;
            weights[8][0] = 1;
            var problem = new TSProblem(0, null, weights,
                new [] { 0, 2, 4, 6, 8 });

            for (var i = 0; i < 10; i++)
            {
                // generate solution.
                var candidate = EAXSolver.Default.Search(problem);

                // test contents.
                Assert.Equal(4, candidate.Fitness);
                var solutionList = new List<int>(candidate.Solution);
                Assert.Equal(0, solutionList[0]);
                Assert.True(solutionList.Remove(0));
                Assert.True(solutionList.Remove(2));
                Assert.True(solutionList.Remove(4));
                Assert.True(solutionList.Remove(6));
                Assert.True(solutionList.Remove(8));
                Assert.Empty(solutionList);
            }
        }
    }
}