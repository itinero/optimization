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

using System;
using Itinero.Optimization.Solvers.Shared;
using Itinero.Optimization.Solvers.Tours;
using Itinero.Optimization.Solvers.TSP_TW;
using Xunit;

namespace Itinero.Optimization.Tests.Solvers.TSP_TW
{
    public class TSPTWFitnessTests
    {
        [Fact]
        public void TSPTWFitness_ShouldTakeIntoAccountTripToFirst()
        {
            // create problem.
            var problem = new TSPTWProblem(0, 0, WeightMatrixHelpers.Build(5, 10),
                TimeWindowHelpers.Unlimited(5));
            var tour = new Tour(new[] {0, 1, 2, 3, 4}, 0);
            
            Assert.Equal(50, tour.Fitness(problem));
        }

        [Fact]
        public void TSPTWFitness_ShouldNotApplyPenaltiesWhenFeasible()
        {
            var weights = WeightMatrixHelpers.Build(5, 2);
            var windows = new TimeWindow[5];
            windows[2] = new TimeWindow()
            {
                Min = 1,
                Max = 3
            };
            var problem = new TSPTWProblem(0, 0, weights, windows);
            
            // create a feasible route.
            var tour = new Tour(new int[] { 0, 2, 4, 1, 3 }, 0);

            // apply the 1-shift local search.
            var fitness = tour.Fitness(problem);
            Assert.Equal(10, fitness);
        }

        [Fact]
        public void TSPTWFitness_ShouldApplyPenaltiesWhenUnfeasible()
        {
            var weights = WeightMatrixHelpers.Build(5, 2);
            var windows = new TimeWindow[5];
            windows[2] = new TimeWindow()
            {
                Min = 1,
                Max = 3
            };
            var problem = new TSPTWProblem(0, 0, weights, windows);

            // create a route with one shift.
            var tour = new Tour(new int[] { 0, 1, 2, 3, 4 }, 0);

            var fitness = tour.Fitness(problem, timeWindowViolationPenaltyFactor: 1000000);
            Assert.Equal(1000010, fitness);
        }

        [Fact]
        public void TSPTWFitness_LastViolated_ShouldApplyPenalty()
        {
            var weights = WeightMatrixHelpers.Build(5, 2);
            var windows = new TimeWindow[5];
            windows[4] = new TimeWindow()
            {
                Min = 1,
                Max = 3
            };
            var problem = new TSPTWProblem(0, 0, weights, windows);

            // create a route with one shift.
            var tour = new Tour(new int[] { 0, 1, 2, 3, 4 }, 0);

            var fitness = tour.Fitness(problem, timeWindowViolationPenaltyFactor: 1000000);
            Assert.Equal(5000010, fitness);
        }
    }
}