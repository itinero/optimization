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

using Itinero.Optimization.Solvers.Shared;
using Itinero.Optimization.Solvers.Shared.TimeWindows.Local1Shift;
using Itinero.Optimization.Solvers.Tours;
using Xunit;

namespace Itinero.Optimization.Tests.Solvers.Shared.TimeWindows.Local1Shift
{
    public class Local1ShiftOperationTests
    {
        /// <summary>
        /// Tests an infeasible route where a violated customer can be moved backwards.
        /// </summary>
        [Fact]
        public void Local1ShiftOperation_ShouldMoveViolatedBackward()
        {
            var weights = WeightMatrixHelpers.Build(5, 2);
            var windows = new TimeWindow[5];
            windows[2] = new TimeWindow()
            {
                Times = new[] {1f, 3f}
            };

            // create a route with one shift.
            var tour = new Tour(new int[] { 0, 1, 2, 3, 4 }, 0);

            // apply the 1-shift local search, it should find the customer to replocate.
            var result = tour.MoveViolatedBackward((x, y) => weights[x][y], windows);

            // test result.
            Assert.True(result.success);
            Assert.Equal(new int[] { 0, 2, 1, 3, 4 }, tour);

            // create a route with one shift.
            tour = new Tour(new int[] { 0, 4, 1, 3, 2 }, 0);

            // apply the 1-shift local search, it should find the customer to replocate.
            result = tour.MoveViolatedBackward((x, y) => weights[x][y], windows); // shifts 2 after 0

            // test result.
            Assert.True(result.success);
            Assert.Equal(new int[] { 0, 2, 4, 1, 3 }, tour);
        }

        [Fact]
        public void Local1ShiftOperation_ShouldDoNothingWhenBest()
        {
            var weights = WeightMatrixHelpers.Build(5, 2);
            var windows = new TimeWindow[5];
            windows[2] = new TimeWindow()
            {
                Times = new[] {1f, 3f}
            };
            
            // create a feasible route.
            var tour = new Tour(new int[] { 0, 2, 4, 1, 3 }, 0);

            // apply the 1-shift local search, it should find the customer to replocate.
            var result = tour.TryShift((x, y) => weights[x][y], windows);
            Assert.False(result.success);
        }

        /// <summary>
        /// Tests an infeasible route where a non-violated customer can be moved forward.
        /// </summary>
        [Fact]
        public void Local1ShiftOperation_ShouldShiftNonViolatedForward()
        {
            var weights = WeightMatrixHelpers.Build(5, 2);
            var windows = new TimeWindow[5];
            windows[2] = new TimeWindow()
            {
                Times = new[] {1f, 3f}
            };

            // create a route with one shift.
            var tour = new Tour(new int[] { 0, 1, 2, 3, 4 }, 0);

            // apply the 1-shift local search, it should find the customer to replocate.
            var result = tour.MoveNonViolatedForward((x, y) => weights[x][y], windows);

            // test result.
            Assert.True(result.success);
            Assert.Equal(new int[] { 0, 2, 1, 3, 4 }, tour);
        }

        /// <summary>
        /// Tests an infeasible route where a non-violated customer can be moved backward.
        /// </summary>
        [Fact]
        public void Local1ShiftOperation_ShouldShiftNonViolatedBackward()
        {
            var weights = WeightMatrixHelpers.Build(5, 10);
            weights[0][3] = 1;
            weights[3][1] = 1;
            var windows = new TimeWindow[5];
            windows[2] = new TimeWindow()
            {
                Times = new[] {1f, 3f}
            };

            // create a route with one shift.
            var tour = new Tour(new int[] { 0, 1, 2, 3, 4 }, 0);

            // apply the 1-shift local search, it should find the customer to relocate.
            var result = tour.MoveNonViolatedBackward((x, y) => weights[x][y], windows);

            // test result.
            Assert.True(result.success);
            //Assert.AreEqual(7, delta);
            Assert.Equal(new int[] { 0, 3, 1, 2, 4 }, tour);
        }

        /// <summary>
        /// Tests an infeasible route where a violated customer can be moved forward.
        /// </summary>
        [Fact]
        public void Local1ShiftOperation_ShouldShiftViolatedForward()
        {
            var weights = WeightMatrixHelpers.Build(5, 10);
            weights[0][3] = 1;
            weights[3][1] = 1;
            var windows = new TimeWindow[5];
            windows[1] = new TimeWindow()
            {
                Times = new[] {1f, 3f}
            };
            windows[3] = new TimeWindow()
            {
                Times = new[] {0f, 2f}
            };

            // create a route with one shift.
            var tour = new Tour(new int[] { 0, 1, 3, 2, 4 }, 0);

            // apply the 1-shift local search, it should find the customer to replocate.
            var result = tour.MoveViolatedForward((x, y) => weights[x][y], windows);

            // test result.
            Assert.True(result.success);
            //Assert.AreEqual(25, delta);
            Assert.Equal(new int[] { 0, 3, 1, 2, 4 }, tour);
        }

        /// <summary>
        /// Tests the local1shift on an infeasible route where the last violated customer cannot be moved.
        /// </summary>
        [Fact]
        public void Local1ShiftOperation_ShouldShiftValidForwardWhenInvalidFixed()
        {
            var weights = WeightMatrixHelpers.Build(5, 10);
            weights[0][1] = 2;
            weights[1][2] = 2;
            weights[2][3] = 2;
            weights[3][4] = 2;
            weights[4][0] = 2;
            var windows = new TimeWindow[5];
            windows[4] = new TimeWindow()
            {
                Times = new[] {7f, 9f}
            };

            // create a route with one shift.
            var tour = new Tour(new int[] { 0, 1, 3, 2, 4 }, 4);

            // apply the 1-shift local search, it should find the customer to replocate.
            var result = tour.TryShift((x, y) => weights[x][y], windows);
        }

        /// <summary>
        /// Tests the local1shift on an infeasible route where the last violated customer cannot be moved.
        /// </summary>
        [Fact]
        public void Local1ShiftOperation_ShouldShiftValidBackwardWhenInvalidFixed()
        {
            var weights = WeightMatrixHelpers.Build(5, 10);
            weights[0][1] = 2;
            weights[1][2] = 2;
            weights[2][3] = 2;
            weights[3][4] = 2;
            weights[4][0] = 2;
            var windows = new TimeWindow[5];
            windows[4] = new TimeWindow()
            {
                Times = new[] {7f, 9f}
            };

            // create a route with one shift.
            var tour = new Tour(new int[] { 0, 1, 3, 2, 4 }, 4);

            // apply the 1-shift local search, it should find the customer to replocate.
            var result = tour.MoveNonViolatedBackward((x, y) => weights[x][y], windows);

            // test result.
            Assert.True(result.success);
            // Assert.AreEqual(23, delta);
            Assert.Equal(new int[] { 0, 1, 2, 3, 4 }, tour);
        }
    }
}