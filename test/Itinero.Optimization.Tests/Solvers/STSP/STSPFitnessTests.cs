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
using Itinero.Optimization.Solvers.Tours;
using Xunit;

namespace Itinero.Optimization.Tests.Solvers.STSP
{
    public class STSPFitnessTests
    {
        [Fact]
        public void STSPFitness_ShouldTakeIntoAccountUnplacedVisits()
        {
            // create problem.
            var problem = STSPHelper.Create(0, 0, 5, 10, 40);
            var tour = new Tour(new[] {0, 1}, 0);

            Assert.Equal(40 * 5 - 40 * 2 + 20, tour.Fitness(problem));
        }
        
        [Fact]
        public void STSPFitness_ShouldEqualTravelWeightWhenAllPlaced()
        {
            // create problem.
            var problem = STSPHelper.Create(0, 0, 5, 10, 50);
            var tour = new Tour(new[] {0, 1, 2, 3, 4}, 0);

            Assert.Equal(50, tour.Fitness(problem));
        }
    }
}