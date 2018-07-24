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
    public class Local2OptOperatorTests
    {
        [Fact]
        public void Local2OptOperator_ShouldExecutePossibleMove()
        {
            var weights = WeightMatrixHelpers.Build(5, 10);
            weights[0][1] = 1;
            weights[1][2] = 1;
            weights[2][3] = 1;
            weights[3][4] = 1;
            weights[4][0] = 1;
            weights[3][1] = 100;
            var windows = new TimeWindow[5];
            var problem = new TSPTWProblem(0, 0, weights, windows);
            
            var tour = new Tour(new int[] { 0, 3, 2, 1, 4 }, 0);

            // apply operator, should detect possible move.
            var fitness = tour.Fitness(problem);
            var candidate = new Candidate<TSPTWProblem, Tour>()
            {
                Problem = problem,
                Solution = tour,
                Fitness = fitness
            };
            Assert.True(Local2OptOperator.Default.Apply(candidate));

            // test result.
            Assert.Equal(5, candidate.Fitness);
            Assert.Equal(new int[] { 0, 1, 2, 3, 4 }, tour);
        }
        
        [Fact]
        public void Local2OptOperator_ShouldDoNothingOnImpossibleMove()
        {
            var weights = WeightMatrixHelpers.Build(5, 10);
            weights[0][1] = 1;
            weights[1][2] = 1;
            weights[2][3] = 1;
            weights[3][4] = 1;
            weights[4][0] = 1;
            weights[3][1] = 100;
            var windows = new TimeWindow[5];
            windows[3] = new TimeWindow()
            {
                Min = 1,
                Max = 2
            };
            windows[2] = new TimeWindow()
            {
                Min = 11,
                Max = 12
            };
            var problem = new TSPTWProblem(0, 0, weights, windows);

            var tour = new Tour(new int[] { 0, 3, 2, 1, 4 }, 0);

            // apply operator, should detect possible move.
            var fitness = tour.Fitness(problem);
            var candidate = new Candidate<TSPTWProblem, Tour>()
            {
                Problem = problem,
                Solution = tour,
                Fitness = fitness
            };
            Assert.False(Local2OptOperator.Default.Apply(candidate));
        }
    }
}