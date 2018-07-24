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
using Itinero.Optimization.Solvers.Shared.TimeWindows.Local2Opt;
using Itinero.Optimization.Solvers.Tours;
using Xunit;

namespace Itinero.Optimization.Tests.Solvers.Shared.TimeWindows.Local2Opt
{
    public class Local2OptOperationTests
    {

        [Fact]
        public void Local2OptOperation_ShouldExecutePossibleMove()
        {
            var weights = WeightMatrixHelpers.Build(5, 10);
            weights[0][1] = 1;
            weights[1][2] = 1;
            weights[2][3] = 1;
            weights[3][4] = 1;
            weights[4][0] = 1;
            weights[3][1] = 100;
            var windows = new TimeWindow[5];
            
            var tour = new Tour(new int[] { 0, 3, 2, 1, 4 }, 0);

            // apply operator, should detect possible move.
            Assert.True(tour.Do2Opt((x, y) => weights[x][y], windows));

            // test result.
            Assert.Equal(new [] { 0, 1, 2, 3, 4 }, tour);
        }
        
        [Fact]
        public void Local2OptOperation_ShouldDoNothingOnImpossibleMove()
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
            
            var tour = new Tour(new [] { 0, 3, 2, 1, 4 }, 0);

            // apply operator, should detect possible move.
            Assert.False(tour.Do2Opt((x, y) => weights[x][y], windows));

            // test result.
            Assert.Equal(new [] { 0, 3, 2, 1, 4 }, tour);
        }
    }
}