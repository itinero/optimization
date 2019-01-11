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
using Itinero.Optimization.Solvers;
using Itinero.Optimization.Solvers.Tours;
using Itinero.Optimization.Solvers.TSP_TW;
using Itinero.Optimization.Solvers.TSP_TW.Operators;
using Xunit;

namespace Itinero.Optimization.Tests.Solvers.TSP_TW.Operators
{
    public class Random1ShiftPerturberTests
    {
        /// <summary>
        /// Random 1 shift should just use random 1 shift operation, apply the same tests.
        /// </summary>
        [Fact]
        public void Random1ShiftPerturber_ShouldJustUseRandom1ShiftOperation()
        {
            var problem = new TSPTWProblem(0, null, WeightMatrixHelpers.Build(5, 10),
                TimeWindowHelpers.Unlimited(5));
            var tour = new Tour(new[] {0, 1, 2, 3, 4}, 0);
            var count = tour.Count;

            var candidate = new Candidate<TSPTWProblem, Tour>()
            {
                Fitness = tour.Fitness(problem),
                Solution = tour,
                Problem = problem
            };
            for (var i = 0; i < 100; i++)
            {
                var fitness = candidate.Fitness;
                if (Random1ShiftPerturber.Default.Apply(candidate))
                {
                    Assert.True(fitness > candidate.Fitness);
                }
                else
                {
                    Assert.True(fitness <= candidate.Fitness);
                }
                Assert.Equal(count, tour.Count);
                
                var content = new HashSet<int>(tour);
                Assert.Contains(0, content);
                Assert.Contains(1, content);
                Assert.Contains(2, content);
                Assert.Contains(3, content);
                Assert.Contains(4, content);
            }
        }
    }
}