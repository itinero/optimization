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

using Itinero.Optimization.Solvers.TSP.HillClimbing3Opt;
using Xunit;

namespace Itinero.Optimization.Tests.Solvers.TSP.HillClimbing3Opt
{
    public class HillClimbing3OptSolverTests
    {
        [Fact]
        public void HillClimbing3OptSolver_ThisClosedTour1ShouldSolverPerfectly()
        {
            var problem = TSPHelper.CreateTSP(0, 0, 5, 10);
            problem._weights[0][1] = 1;
            problem._weights[1][2] = 1;
            problem._weights[2][3] = 1;
            problem._weights[3][4] = 1;
            problem._weights[4][0] = 1;

            // solve problem.
            var solver = new HillClimbing3OptSolver();
            var solution = solver.Search(problem);
            var tour = solution.Solution;

            // check result.
            Assert.NotNull(tour);
            Assert.Equal(5, tour.Count);
            Assert.Equal(0, tour.GetVisitAt(0));
            Assert.Equal(1, tour.GetVisitAt(1));
            Assert.Equal(2, tour.GetVisitAt(2));
            Assert.Equal(3, tour.GetVisitAt(3));
            Assert.Equal(4, tour.GetVisitAt(4));
            Assert.Equal(0, tour.Last);
        }
    }
}