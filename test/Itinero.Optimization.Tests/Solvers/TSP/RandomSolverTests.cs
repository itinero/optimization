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
using Itinero.Optimization.Strategies.Random;
using Xunit;

namespace Itinero.Optimization.Tests.Solvers.TSP
{
    public class RandomSolverTests
    {
        /// <summary>
        /// Tests the solver name.
        /// </summary>
        [Fact]
        public void RandomSolver_NameShouldBeRAN()
        {
            // create the solver.
            var solver = new RandomSolver();

            Assert.Equal("RAN", solver.Name);
        }

        /// <summary>
        /// Tests the solver on a 'open' tsp.
        /// </summary>
        [Fact]
        public void RandomSolver_ShouldGenerateRandomPermutationOnOpen()
        {
            // create problem.
            var problem = TSPHelper.CreateTSP(0, 5, 10);

            // create the solver.
            var solver = new RandomSolver();

            for (var i = 0; i < 100; i++)
            {
                // generate solution.
                var candidate = solver.Search(problem);
                var tour = candidate.Solution;
                var fitness = candidate.Fitness;

                // test contents.
                Assert.Equal(40, fitness);
                Assert.Equal(problem.First, tour.First);
                Assert.Null(tour.Last);

                var solutionList = new List<int>(tour);
                Assert.Equal(0, solutionList[0]);
                Assert.True(solutionList.Remove(0));
                Assert.True(solutionList.Remove(1));
                Assert.True(solutionList.Remove(2));
                Assert.True(solutionList.Remove(3));
                Assert.True(solutionList.Remove(4));
                Assert.Empty(solutionList);
            }
        }

        /// <summary>
        /// Tests the solver on a closed tsp.
        /// </summary>
        [Fact]
        public void RandomSolver_ShouldGenerateRandomPermutationOnClosed()
        {
            // create problem.
            var problem = TSPHelper.CreateTSP(0, 0, 5, 10);

            // create the solver.
            var solver = new RandomSolver();

            for (var i = 0; i < 100; i++)
            {
                // generate solution.
                var candidate = solver.Search(problem);
                var tour = candidate.Solution;
                var fitness = candidate.Fitness;

                // test contents.
                Assert.Equal(50, fitness);
                Assert.Equal(problem.First, tour.First);
                Assert.Equal(problem.First, tour.Last);

                var solutionList = new List<int>(tour);
                Assert.Equal(0, solutionList[0]);
                Assert.True(solutionList.Remove(0));
                Assert.True(solutionList.Remove(1));
                Assert.True(solutionList.Remove(2));
                Assert.True(solutionList.Remove(3));
                Assert.True(solutionList.Remove(4));
                Assert.Empty(solutionList);
            }
        }

        /// <summary>
        /// Tests the solver on a closed tsp.
        /// </summary>
        [Fact]
        public void RandomSolver_ShouldGenerateRandomPermutationOnFixed()
        {
            // create problem.
            var problem = TSPHelper.CreateTSP(0, 4, 5, 10);

            // create the solver.
            var solver = new RandomSolver();

            for (var i = 0; i < 100; i++)
            {
                // generate solution.
                var candidate = solver.Search(problem);
                var tour = candidate.Solution;
                var fitness = candidate.Fitness;

                // test contents.
                Assert.Equal(40, fitness);
                Assert.Equal(problem.First, tour.First);
                Assert.Equal(problem.Last, tour.Last);

                var solutionList = new List<int>(tour);
                Assert.Equal(0, solutionList[0]);
                Assert.True(solutionList.Remove(0));
                Assert.True(solutionList.Remove(1));
                Assert.True(solutionList.Remove(2));
                Assert.True(solutionList.Remove(3));
                Assert.True(solutionList.Remove(4));
                Assert.Empty(solutionList);
            }
        }

        /// <summary>
        /// Tests the solver on a closed tsp with custom visits.
        /// </summary>
        [Fact]
        public void RandomSolver_ShouldUseOnlyProvidedVisits()
        {
            // create problem.
            var problem = TSPHelper.CreateTSP(0, 8, 10, 10, 
                new [] { 0, 2, 4, 6, 8 });

            for (var i = 0; i < 100; i++)
            {
                // generate solution.
                var candidate = RandomSolver.Default.Search(problem);
                var tour = candidate.Solution;
                var fitness = candidate.Fitness;

                // test contents.
                Assert.Equal(40, fitness);
                Assert.Equal(problem.First, tour.First);
                Assert.Equal(problem.Last, tour.Last);

                var solutionList = new List<int>(tour);
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