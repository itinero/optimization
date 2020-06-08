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
using Itinero.Optimization.Solvers;
using Itinero.Optimization.Solvers.Shared;
using Itinero.Optimization.Solvers.Tours;
using Itinero.Optimization.Solvers.TSP_TW;
using Itinero.Optimization.Solvers.TSP_TW.Operators;
using Xunit;

namespace Itinero.Optimization.Tests.Solvers.TSP_TW.Operators
{
    public class Local1ShiftOperatorTests
    {
        [Fact]
        public void Local1ShiftOperator_ShouldDoNothingWhenBest()
        {
            var weights = WeightMatrixHelpers.Build(5, 2);
            var windows = new TimeWindow[5];
            windows[2] = new TimeWindow()
            {
                Times = new[] {1f, 3f}
            };
            var problem = new TSPTWProblem(0, 0, weights, windows);
            
            // create a feasible route.
            var tour = new Tour(new int[] { 0, 2, 4, 1, 3 }, 0);

            // apply the 1-shift local search.
            var fitness = tour.Fitness(problem);
            var candidate = new Candidate<TSPTWProblem, Tour>()
            {
                Problem = problem,
                Solution = tour,
                Fitness = fitness
            };
            Assert.False(Local1ShiftOperator.Default.Apply(candidate));
            
            // nothing should have happened, everything should be the same.
            Assert.Equal(fitness, candidate.Fitness);
            Assert.Equal(new int[] { 0, 2, 4, 1, 3 }, candidate.Solution);
        }
        
        [Fact]
        public void Local1ShiftOperator_ShouldDetectMoveToMakeTourFeasible()
        {
            var weights = WeightMatrixHelpers.Build(5, 2);
            var windows = new TimeWindow[5];
            windows[2] = new TimeWindow()
            {
                Times = new[] {1f, 3f}
            };
            var problem = new TSPTWProblem(0, 0, weights, windows);

            // create a route with one shift.
            var tour = new Tour(new int[] { 0, 1, 2, 3, 4 }, 0);

            // apply the 1-shift local search, it should find the customer to replocate.
            var fitness = tour.Fitness(problem);
            var candidate = new Candidate<TSPTWProblem, Tour>()
            {
                Problem = problem,
                Solution = tour,
                Fitness = fitness
            };
            Assert.True(Local1ShiftOperator.Default.Apply(candidate));

            // test result.
            Assert.Equal(new int[] { 0, 2, 1, 3, 4 }, candidate.Solution);
            Assert.Equal(10, candidate.Fitness);
        }
    }
}