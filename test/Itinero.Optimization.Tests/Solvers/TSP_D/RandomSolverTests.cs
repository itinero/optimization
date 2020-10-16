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
using Itinero.Optimization.Solvers.Shared.Directed;
using Itinero.Optimization.Solvers.TSP_D;
using Itinero.Optimization.Strategies.Random;
using Xunit;

namespace Itinero.Optimization.Tests.Solvers.TSP_D
{
    public class RandomSolverTests
    {
        /// <summary>
        /// Tests the solver name.
        /// </summary>
        [Fact]
        public void RandomSolver_NameShouldBeRAN()
        {
            Assert.Equal("RAN", RandomSolver.Default.Name);
        }

        /// <summary>
        /// Tests the solver on a 'open' tsp.
        /// </summary>
        [Fact]
        public void RandomSolver_ShouldGenerateRandomPermutationOnOpen()
        {
            // create problem.
            var problem = new TSPDProblem(0, null, WeightMatrixHelpers.BuildDirected(5, 10), 2);

            for (var i = 0; i < 100; i++)
            {
                // generate solution.
                var candidate = RandomSolver.Default.Search(problem);
                var tour = candidate.Solution;
                var fitness = candidate.Fitness;

                // test contents.
                Assert.Equal(40, fitness);
                Assert.Equal(problem.First, DirectedHelper.Extract(tour.First).visit);
                Assert.Null(tour.Last);

                var solutionList = new List<int>(tour);
                Assert.Equal(3, solutionList[0]);
                Assert.True(solutionList.Remove(solutionList.Find(x => DirectedHelper.Extract(x).visit == 0)));
                Assert.True(solutionList.Remove(solutionList.Find(x => DirectedHelper.Extract(x).visit == 1)));
                Assert.True(solutionList.Remove(solutionList.Find(x => DirectedHelper.Extract(x).visit == 2)));
                Assert.True(solutionList.Remove(solutionList.Find(x => DirectedHelper.Extract(x).visit == 3)));
                Assert.True(solutionList.Remove(solutionList.Find(x => DirectedHelper.Extract(x).visit == 4)));
                Assert.Empty(solutionList);
            }
        }

        /// <summary>
        /// Tests the solver on a 'closed' tsp.
        /// </summary>
        [Fact]
        public void RandomSolver_ShouldGenerateRandomPermutationOnClosed()
        {
            // create problem.
            var problem = new TSPDProblem(0, 0, WeightMatrixHelpers.BuildDirected(5, 10), 2);

            for (var i = 0; i < 100; i++)
            {
                // generate solution.
                var candidate = RandomSolver.Default.Search(problem);
                var tour = candidate.Solution;
                var fitness = candidate.Fitness;

                // test contents.
                Assert.Equal(50, fitness);
                Assert.Equal(problem.First, DirectedHelper.Extract(tour.First).visit);
                Assert.NotNull(tour.Last);
                Assert.Equal(problem.Last, DirectedHelper.Extract(tour.Last.Value).visit);

                var solutionList = new List<int>(tour);
                Assert.Equal(3, solutionList[0]);
                Assert.True(solutionList.Remove(solutionList.Find(x => DirectedHelper.Extract(x).visit == 0)));
                Assert.True(solutionList.Remove(solutionList.Find(x => DirectedHelper.Extract(x).visit == 1)));
                Assert.True(solutionList.Remove(solutionList.Find(x => DirectedHelper.Extract(x).visit == 2)));
                Assert.True(solutionList.Remove(solutionList.Find(x => DirectedHelper.Extract(x).visit == 3)));
                Assert.True(solutionList.Remove(solutionList.Find(x => DirectedHelper.Extract(x).visit == 4)));
                Assert.Empty(solutionList);
            }
        }

        /// <summary>
        /// Tests the solver on a 'fixed' tsp.
        /// </summary>
        [Fact]
        public void RandomSolver_ShouldGenerateRandomPermutationOnFixed()
        {
            // create problem.
            var problem = new TSPDProblem(0, 4, WeightMatrixHelpers.BuildDirected(5, 10), 2);

            for (var i = 0; i < 100; i++)
            {
                // generate solution.
                var candidate = RandomSolver.Default.Search(problem);
                var tour = candidate.Solution;
                var fitness = candidate.Fitness;

                // test contents.
                Assert.Equal(40, fitness);
                Assert.Equal(problem.First, DirectedHelper.Extract(tour.First).visit);
                Assert.NotNull(tour.Last);
                Assert.Equal(problem.Last, DirectedHelper.Extract(tour.Last.Value).visit);

                var solutionList = new List<int>(tour);
                Assert.Equal(3, solutionList[0]);
                Assert.True(solutionList.Remove(solutionList.Find(x => DirectedHelper.Extract(x).visit == 0)));
                Assert.True(solutionList.Remove(solutionList.Find(x => DirectedHelper.Extract(x).visit == 1)));
                Assert.True(solutionList.Remove(solutionList.Find(x => DirectedHelper.Extract(x).visit == 2)));
                Assert.True(solutionList.Remove(solutionList.Find(x => DirectedHelper.Extract(x).visit == 3)));
                Assert.True(solutionList.Remove(solutionList.Find(x => DirectedHelper.Extract(x).visit == 4)));
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
            var problem = new TSPDProblem(0, 4, WeightMatrixHelpers.BuildDirected(6, 10), 2,
                new int[] { 0, 1, 3, 4, 5 });

            for (var i = 0; i < 100; i++)
            {
                // generate solution.
                var candidate = RandomSolver.Default.Search(problem);
                var tour = candidate.Solution;
                var fitness = candidate.Fitness;

                // test contents.
                Assert.Equal(40, fitness);
                Assert.Equal(problem.First, DirectedHelper.Extract(tour.First).visit);
                Assert.NotNull(tour.Last);
                Assert.Equal(problem.Last, DirectedHelper.Extract(tour.Last.Value).visit);

                var solutionList = new List<int>(tour);
                Assert.Equal(3, solutionList[0]);
                Assert.True(solutionList.Remove(solutionList.Find(x => DirectedHelper.Extract(x).visit == 0)));
                Assert.True(solutionList.Remove(solutionList.Find(x => DirectedHelper.Extract(x).visit == 1)));
//              Assert.True(solutionList.Remove(solutionList.Find(x => DirectedHelper.Extract(x).visit == 2)));
                Assert.True(solutionList.Remove(solutionList.Find(x => DirectedHelper.Extract(x).visit == 3)));
                Assert.True(solutionList.Remove(solutionList.Find(x => DirectedHelper.Extract(x).visit == 4)));
                Assert.True(solutionList.Remove(solutionList.Find(x => DirectedHelper.Extract(x).visit == 5)));
                Assert.Empty(solutionList);
            }
        }
    }
}