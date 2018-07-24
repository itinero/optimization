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

using Itinero.Optimization.Solvers.CVRP_ND;
using Xunit;

namespace Itinero.Optimization.Tests.Solvers.CVRP_ND
{
    public class CVRPSolutionTests
    {
        [Fact]
        public void CVRPSolution_AddNewShouldAddNewTour()
        {
            var solution = new CVRPNDSolution();

            var t = solution.AddNew(0, null);
            Assert.Equal(1, solution.Count);
            
            var tour = solution.Tour(t);
            Assert.NotNull(tour);
            Assert.Equal(0, tour.First);
            Assert.Null(tour.Last);

            t = solution.AddNew(1, 2);
            Assert.Equal(2, solution.Count);
            
            tour = solution.Tour(t);
            Assert.NotNull(tour);
            Assert.Equal(1, tour.First);
            Assert.Equal(2, tour.Last);
        }
    }
}