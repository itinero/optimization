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

using Itinero.Optimization.Solvers.CVRP;
using Xunit;

namespace Itinero.Optimization.Tests.Solvers.CVRP
{
    public class CVRProblemTests
    {
        [Fact]
        public void CVRProblem_DepartureAndArrivalShouldBeProperlySet()
        {
            var problem = new CVRProblem(1, null, new float[0][], null);
            
            Assert.Equal(1, problem.Departure);
            Assert.Null(problem.Arrival);
            
            problem = new CVRProblem(1, 2, new float[0][], null);
            
            Assert.Equal(1, problem.Departure);
            Assert.Equal(2, problem.Arrival);
        }
        
        [Fact]
        public void CVRProblem_ShouldReturnTravelWeightsFromGivenMatrix()
        {
            var problem = new CVRProblem(1, null, new []
            {
                new [] { 0f, 1f, 2f }, 
                new [] { 3f, 4f, 5f }, 
                new [] { 6f, 7f, 8f }
            }, null);
            
            Assert.Equal(0, problem.TravelWeight(0, 0));
            Assert.Equal(1, problem.TravelWeight(0, 1));
            Assert.Equal(2, problem.TravelWeight(0, 2));
            Assert.Equal(3, problem.TravelWeight(1, 0));
            Assert.Equal(4, problem.TravelWeight(1, 1));
            Assert.Equal(5, problem.TravelWeight(1, 2));
            Assert.Equal(6, problem.TravelWeight(2, 0));
            Assert.Equal(7, problem.TravelWeight(2, 1));
            Assert.Equal(8, problem.TravelWeight(2, 2));
        }
    }
}