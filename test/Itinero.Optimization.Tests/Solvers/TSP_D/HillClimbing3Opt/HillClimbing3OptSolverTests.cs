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

using Itinero.Optimization.Solvers.TSP_D;
using Itinero.Optimization.Solvers.TSP_D.HillClimbing3Opt;
using Itinero.Optimization.Solvers.Shared.Directed;
using Xunit;

namespace Itinero.Optimization.Tests.Solvers.TSP_D.HillClimbing3Opt
{
    public class HillClimbing3OptSolverTests
    {
        [Fact]
        public void HillClimbing3OptSolver_ThisClosedTour1ShouldSolvePerfectly()
        {
            var weights = WeightMatrixHelpers.BuildDirected(5, 10);
            weights[DirectionEnum.Forward.WeightId(0)][DirectionEnum.Forward.WeightId(1)] = 1;
            weights[DirectionEnum.Forward.WeightId(1)][DirectionEnum.Forward.WeightId(2)] = 1;
            weights[DirectionEnum.Forward.WeightId(2)][DirectionEnum.Forward.WeightId(3)] = 1;
            weights[DirectionEnum.Forward.WeightId(3)][DirectionEnum.Forward.WeightId(4)] = 1;
            weights[DirectionEnum.Forward.WeightId(4)][DirectionEnum.Forward.WeightId(0)] = 1;
            var problem = new TSPDProblem(0, 0, weights, 2);

            for (var t = 0; t < 100; t++)
            {
                // solve problem.
                var solution = HillClimbing3OptSolver.Default.Search(problem);
                var tour = solution.Solution;

                // check result.
                Assert.NotNull(tour);
                Assert.Equal(5, tour.Count);
                Assert.Equal(0, DirectedHelper.ExtractVisit(tour.GetVisitAt(0)));
                Assert.Equal(1, DirectedHelper.ExtractVisit(tour.GetVisitAt(1)));
                Assert.Equal(2, DirectedHelper.ExtractVisit(tour.GetVisitAt(2)));
                Assert.Equal(3, DirectedHelper.ExtractVisit(tour.GetVisitAt(3)));
                Assert.Equal(4, DirectedHelper.ExtractVisit(tour.GetVisitAt(4)));
                Assert.NotNull(tour.Last);
                Assert.Equal(0, DirectedHelper.ExtractVisit(tour.Last.Value));
            }
        }
        
//        [Fact]
//        public void HillClimbing3OptSolver_ShouldUseOnlyProvidedVisits()
//        {
//            var weights = WeightMatrixHelpers.Build(10, 10);
//            weights[0][2] = 1;
//            weights[2][4] = 1;
//            weights[4][6] = 1;
//            weights[6][8] = 1;
//            weights[8][0] = 1;
//            var problem = new TSProblem(0, 0, weights,
//                new int[] { 0, 2, 4, 6, 8 });
//
//            // solve problem.
//            var solution = HillClimbing3OptSolver.Default.Search(problem);
//            var tour = solution.Solution;
//            var fitness = solution.Fitness;
//
//            // check result.
//            Assert.NotNull(tour);
//            Assert.Equal(5, fitness);
//            Assert.Equal(5, tour.Count);
//            Assert.Equal(0, tour.GetVisitAt(0));
//            Assert.Equal(2, tour.GetVisitAt(1));
//            Assert.Equal(4, tour.GetVisitAt(2));
//            Assert.Equal(6, tour.GetVisitAt(3));
//            Assert.Equal(8, tour.GetVisitAt(4));
//            Assert.Equal(0, tour.Last);
//        }
    }
}