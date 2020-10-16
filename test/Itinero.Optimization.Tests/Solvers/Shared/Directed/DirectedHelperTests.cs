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

using Itinero.Optimization.Solvers.Shared.Directed;
using Itinero.Optimization.Solvers.Tours;
using Xunit;

namespace Itinero.Optimization.Tests.Solvers.Shared.Directed
{
    public class DirectedHelperTests
    {
        [Fact]
        public void DirectedHelper_WeightIdShouldBeVisitAndDirectionCombined()
        {
            Assert.Equal(21, DirectedHelper.WeightId(10, DirectionEnum.Forward));
            Assert.Equal(20, DirectedHelper.WeightId(10, DirectionEnum.Backward));
        }

        [Fact]
        public void DirectedHelper_ExtractShouldExtractTurnAndVisit()
        {
            var extracted = DirectedHelper.Extract(40);
            Assert.Equal(10, extracted.visit);
            Assert.Equal(TurnEnum.BackwardBackward, extracted.turn);
            extracted = DirectedHelper.Extract(41);
            Assert.Equal(10, extracted.visit);
            Assert.Equal(TurnEnum.BackwardForward, extracted.turn);
            extracted = DirectedHelper.Extract(42);
            Assert.Equal(10, extracted.visit);
            Assert.Equal(TurnEnum.ForwardBackward, extracted.turn);
            extracted = DirectedHelper.Extract(43);
            Assert.Equal(10, extracted.visit);
            Assert.Equal(TurnEnum.ForwardForward, extracted.turn);
        }

        [Fact]
        public void DirectedHelper_WeightDirectedShouldIncludeWeightsAndTurnPenalties()
        {
            var weightsFunc = WeightMatrixHelpers.BuildDirected(5, 10, 20).ToFunc();
            var turnPenaltyFunc = new float[] { 0, 2, 2, 0 }.ToTurnPenaltyFunc();

            var tour = new Tour(new[]
            {
                TurnEnum.ForwardForward.DirectedVisit(0),
                TurnEnum.ForwardForward.DirectedVisit(1),
                TurnEnum.ForwardForward.DirectedVisit(2),
                TurnEnum.ForwardForward.DirectedVisit(3)
            }, TurnEnum.ForwardForward.DirectedVisit(0));
            Assert.Equal(10 + 10 + 10 + 10, tour.WeightDirected(weightsFunc, turnPenaltyFunc));

            // introduce penalty.
            tour = new Tour(new[]
            {
                TurnEnum.ForwardForward.DirectedVisit(0),
                TurnEnum.ForwardForward.DirectedVisit(1),
                TurnEnum.BackwardForward.DirectedVisit(2),
                TurnEnum.ForwardForward.DirectedVisit(3)
            }, TurnEnum.ForwardForward.DirectedVisit(0));
            Assert.Equal(10 + 20 + 2 + 10 + 10, tour.WeightDirected(weightsFunc, turnPenaltyFunc));
        }

        [Fact]
        public void DirectedHelper_ConvertToUndirectedShouldUseMinimumByDefault()
        {
            var directedWeights = WeightMatrixHelpers.BuildDirected(5, 10, 20);
            var undirectedWeights = directedWeights.ConvertToUndirected();
            
            
        }
    }
}