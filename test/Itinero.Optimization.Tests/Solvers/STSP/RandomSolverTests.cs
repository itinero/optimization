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

using Itinero.Optimization.Solvers.STSP;
using Xunit;

namespace Itinero.Optimization.Tests.Solvers.STSP
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
        /// Tests the solver on a 'open' s-tsp.
        /// </summary>
        [Fact]
        public void RandomSolver_OpenShouldGenerateRandomSequenceUnderMax()
        {
            // create problem.
            var problem = STSPHelper.Create(0, 5, 10, 40);

            for (var i = 0; i < 100; i++)
            {
                // generate solution.
                var candidate = RandomSolver.Default.Search(problem);
                var tour = candidate.Solution;
                var fitness = candidate.Fitness;

                // test contents.
                Assert.Equal(problem.First, tour.First);
                Assert.Null(tour.Last);
                Assert.True(40 >= fitness);
            }
        }

        /// <summary>
        /// Tests the solver on a 'open' but fixed s-tsp.
        /// </summary>
        [Fact]
        public void RandomSolver_OpenFixedShouldGenerateRandomSequenceUnderMax()
        {
            // create problem.
            var problem = STSPHelper.Create(0, 4, 5, 10, 40);

            for (var i = 0; i < 100; i++)
            {
                // generate solution.
                var candidate = RandomSolver.Default.Search(problem);
                var tour = candidate.Solution;
                var fitness = candidate.Fitness;

                // test contents.
                Assert.Equal(problem.First, tour.First);
                Assert.Equal(problem.Last, tour.Last);
                Assert.True(40 >= fitness);
            }
        }

        /// <summary>
        /// Tests the solver on a 'closed' s-tsp.
        /// </summary>
        [Fact]
        public void RandomSolver_ClosedShouldGenerateRandomSequenceUnderMax()
        {
            // create problem.
            var problem = STSPHelper.Create(0, 0, 5, 10, 40);

            for (var i = 0; i < 100; i++)
            {
                // generate solution.
                var candidate = RandomSolver.Default.Search(problem);
                var tour = candidate.Solution;
                var fitness = candidate.Fitness;

                // test contents.
                Assert.Equal(problem.First, tour.First);
                Assert.Equal(problem.Last, tour.Last);
                Assert.True(40 >= fitness);
            }
        }

        /// <summary>
        /// Tests the random solver with a predefined visit list.
        /// </summary>
        [Fact]
        public void RandomSolver_ShouldOnlyUseProvidedVisits()
        {
            var problem = STSPHelper.Create(0, 0, 10, 10, 40,
                new int[] { 0, 2, 4, 6, 8 });

            // solve problem.
            var solution = RandomSolver.Default.Search(problem);
            var tour = solution.Solution;
            var fitness = solution.Fitness;

            // check result.
            Assert.NotNull(tour);
            Assert.Equal(-1, tour.GetIndexOf(1));
            Assert.Equal(-1, tour.GetIndexOf(3));
            Assert.Equal(-1, tour.GetIndexOf(5));
            Assert.Equal(-1, tour.GetIndexOf(7));
            Assert.Equal(-1, tour.GetIndexOf(9));
            Assert.Equal(0, tour.Last);
        }
    }
}