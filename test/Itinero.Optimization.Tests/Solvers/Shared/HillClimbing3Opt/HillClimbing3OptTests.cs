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

using Itinero.Optimization.Solvers;
using Xunit;
using Itinero.Optimization.Solvers.Shared.HillClimbing3Opt;
using Itinero.Optimization.Solvers.Tours;

namespace Itinero.Optimization.Tests.Solvers.Shared.HillClimbing3Opt
{
    public class HillClimbing3OptHelperTests
    {
        [Fact]
        public void HillClimbing3OptHelper_ThisClosedTour1ShouldSolvePerfectly()
        {
            var weights = WeightMatrixHelpers.Build(5, 10);
            weights[0][1] = 1;
            weights[1][2] = 1;
            weights[2][3] = 1;
            weights[3][4] = 1;
            weights[4][0] = 1;
            var tour = new Tour(new [] { 0, 1, 2, 3, 4 }, 0);

            // solve problem.
            var res = tour.Do3Opt((x, y) => weights[x][y], 5);

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