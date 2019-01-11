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

using System.Runtime.CompilerServices;
using Itinero.Optimization.Solvers.Shared.Directed;
using Itinero.Optimization.Solvers.Shared.Directed.CheapestInsertion;
using Itinero.Optimization.Solvers.Tours;
using Xunit;

namespace Itinero.Optimization.Tests.Solvers.Shared.Directed.CheapestInsertion
{
    public class CheapestInsertionHelperTests
    {
        [Fact]
        public void CheapestInsertionHelper_ShouldUseCheapestTurnCombinations()
        {
            var location = new Pair(TurnEnum.ForwardBackward.DirectedVisit(0),
                TurnEnum.BackwardForward.DirectedVisit(2));
            var insertion = location.CalculateCheapestDirected(1, (x, y) => 0, (turn) =>
            {
                switch (turn)
                {
                    case TurnEnum.BackwardForward:
                    case TurnEnum.ForwardBackward:
                        return 100;
                }
                return 0;
            });
            
            Assert.Equal(TurnEnum.ForwardForward, insertion.turn);
            Assert.Equal(DirectionEnum.Forward, insertion.fromDepartureDirection);
            Assert.Equal(DirectionEnum.Forward, insertion.toArrivalDirection);
            Assert.Equal(0, insertion.cost);
        } 

        [Fact]
        public void CheapestInsertionHelper_ShouldChooseCheapestEvenWithTurns()
        {
            var location = new Pair(TurnEnum.ForwardBackward.DirectedVisit(0),
                TurnEnum.BackwardForward.DirectedVisit(2));
            var insertion = location.CalculateCheapestDirected(1, (x, y) =>
            { // force another turn with a penalty by changing weights.
                if (x == DirectionEnum.Backward.WeightId(0) &&
                    y == DirectionEnum.Forward.WeightId(1))
                {
                    return 0;
                }

                if (x == DirectionEnum.Backward.WeightId(1) &&
                    y == DirectionEnum.Backward.WeightId(2))
                {
                    return 0;
                }
                return 1000;
            }, (turn) =>
            {
                switch (turn)
                {
                    case TurnEnum.BackwardForward:
                    case TurnEnum.ForwardBackward:
                        return 100;
                }
                return 0;
            });
            
            Assert.Equal(TurnEnum.ForwardBackward, insertion.turn);
            Assert.Equal(DirectionEnum.Backward, insertion.fromDepartureDirection);
            Assert.Equal(DirectionEnum.Backward, insertion.toArrivalDirection);
            Assert.Equal(300, insertion.cost);
        }
        
        [Fact]
        public void CheapestInsertionHelper_ShouldTolerateAPairWithoutTo()
        {
            var location = new Pair(TurnEnum.ForwardBackward.DirectedVisit(0), -1);
            var insertion = location.CalculateCheapestDirected(1, (x, y) =>
            { // force 0F -> F1 to have the lowest weight.
                if (x == DirectionEnum.Forward.WeightId(0) &&
                    y == DirectionEnum.Forward.WeightId(1))
                {
                    return 0;
                }
                return 1000;
            }, (turn) =>
            {
                switch (turn)
                {
                    case TurnEnum.BackwardForward:
                    case TurnEnum.ForwardBackward:
                        return 100;
                }
                return 0;
            });
            
            Assert.Equal(TurnEnum.ForwardForward, insertion.turn);
            Assert.Equal(DirectionEnum.Forward, insertion.fromDepartureDirection);
            Assert.Equal(0, insertion.cost);
        }
        
        [Fact]
        public void CheapestInsertionHelper_ShouldUseLowestWeightOnAPairWithoutTo()
        {
            var location = new Pair(TurnEnum.ForwardForward.DirectedVisit(0), -1);
            var insertion = location.CalculateCheapestDirected(1, (x, y) =>
            { // force 0F -> F1 to have the lowest weight.
                if (x == DirectionEnum.Backward.WeightId(0) &&
                    y == DirectionEnum.Backward.WeightId(1))
                {
                    return 0;
                }
                return 1000;
            }, (turn) =>
            {
                switch (turn)
                {
                    case TurnEnum.BackwardForward:
                    case TurnEnum.ForwardBackward:
                        return 100;
                }
                return 0;
            });
            
            Assert.Equal(TurnEnum.BackwardForward, insertion.turn);
            Assert.Equal(DirectionEnum.Backward, insertion.fromDepartureDirection);
            Assert.Equal(0, insertion.cost);
        }
        
        [Fact]
        public void CheapestInsertionHelper_ShouldChooseCheapestSecondInOpenTour()
        {
            var weightsFunc = WeightMatrixHelpers.BuildDirected(5, 10).ToFunc();
            var turnPenaltyFunc = new float[] { 0, 2, 2, 0 }.ToTurnPenaltyFunc();

            var tour = new Tour(new int[] { TurnEnum.ForwardForward.DirectedVisit(0) }, null);
            var insertion = tour.CalculateCheapestDirected(new [] { 1, 2, 3, 4 }, weightsFunc, turnPenaltyFunc);
            Assert.Equal(10, insertion.cost);
            Assert.Equal(tour.First, insertion.location.From);
            Assert.Equal(1, insertion.visit);
            Assert.Equal(DirectionEnum.Forward, insertion.fromDepartureDirection);
            
            tour = new Tour(new int[] { TurnEnum.ForwardBackward.DirectedVisit(0) }, null);
            insertion = tour.CalculateCheapestDirected(new [] { 1, 2, 3, 4 }, weightsFunc, turnPenaltyFunc);
            Assert.Equal(10, insertion.cost);
            Assert.Equal(tour.First, insertion.location.From);
            Assert.Equal(1, insertion.visit);
            Assert.Equal(DirectionEnum.Forward, insertion.fromDepartureDirection);

            tour = new Tour(new int[] { TurnEnum.BackwardForward.DirectedVisit(0) }, null);
            insertion = tour.CalculateCheapestDirected(new [] { 1, 2, 3, 4 }, weightsFunc, turnPenaltyFunc);
            Assert.Equal(10, insertion.cost);
            Assert.Equal(tour.First, insertion.location.From);
            Assert.Equal(1, insertion.visit);
            Assert.Equal(DirectionEnum.Forward, insertion.fromDepartureDirection);
            
            tour = new Tour(new int[] { TurnEnum.BackwardBackward.DirectedVisit(0) }, null);
            insertion = tour.CalculateCheapestDirected(new [] { 1, 2, 3, 4 }, weightsFunc, turnPenaltyFunc);
            Assert.Equal(10, insertion.cost);
            Assert.Equal(tour.First, insertion.location.From);
            Assert.Equal(1, insertion.visit);
            Assert.Equal(DirectionEnum.Forward, insertion.fromDepartureDirection);
        }
        
        [Fact]
        public void CheapestInsertionHelper_ShouldChooseCheapestTurnSecondInClosedTour()
        {
            var weightsFunc = WeightMatrixHelpers.BuildDirected(5, 10, 20).ToFunc();
            var turnPenaltyFunc = new float[] { 0, 2, 2, 0 }.ToTurnPenaltyFunc();

            var tour = new Tour(new int[] { TurnEnum.ForwardForward.DirectedVisit(0) }, TurnEnum.ForwardForward.DirectedVisit(0));
            var insertion = tour.CalculateCheapestDirected(new [] { 1, 2, 3, 4 }, weightsFunc, turnPenaltyFunc);
            Assert.Equal(20, insertion.cost);
            Assert.Equal(tour.First, insertion.location.From);
            Assert.Equal(tour.Last, insertion.location.To);
            Assert.Equal(1, insertion.visit);
            Assert.Equal(DirectionEnum.Forward, insertion.fromDepartureDirection);
            Assert.Equal(DirectionEnum.Forward, insertion.toArrivalDirection);
            
            tour = new Tour(new int[] { TurnEnum.ForwardForward.DirectedVisit(0), TurnEnum.ForwardBackward.DirectedVisit(0) }, TurnEnum.ForwardBackward.DirectedVisit(0));
            insertion = tour.CalculateCheapestDirected(new [] { 1, 2, 3, 4 }, weightsFunc, turnPenaltyFunc);
            Assert.Equal(22, insertion.cost);
            Assert.Equal(tour.First, insertion.location.From);
            Assert.Equal(tour.Last, insertion.location.To);
            Assert.Equal(1, insertion.visit);
            Assert.Equal(DirectionEnum.Forward, insertion.fromDepartureDirection);
            Assert.Equal(DirectionEnum.Forward, insertion.toArrivalDirection);

            tour = new Tour(new int[] { TurnEnum.BackwardBackward.DirectedVisit(0), TurnEnum.ForwardForward.DirectedVisit(0) }, TurnEnum.ForwardForward.DirectedVisit(0));
            insertion = tour.CalculateCheapestDirected(new [] { 1, 2, 3, 4 }, weightsFunc, turnPenaltyFunc);
            Assert.Equal(22, insertion.cost);
            Assert.Equal(tour.First, insertion.location.From);
            Assert.Equal(tour.Last, insertion.location.To);
            Assert.Equal(1, insertion.visit);
            Assert.Equal(DirectionEnum.Forward, insertion.fromDepartureDirection);
            Assert.Equal(DirectionEnum.Forward, insertion.toArrivalDirection);
            
            tour = new Tour(new int[] { TurnEnum.BackwardBackward.DirectedVisit(0), TurnEnum.BackwardBackward.DirectedVisit(0) }, TurnEnum.BackwardBackward.DirectedVisit(0));
            insertion = tour.CalculateCheapestDirected(new [] { 1, 2, 3, 4 }, weightsFunc, turnPenaltyFunc);
            Assert.Equal(20, insertion.cost);
            Assert.Equal(tour.First, insertion.location.From);
            Assert.Equal(tour.Last, insertion.location.To);
            Assert.Equal(1, insertion.visit);
            Assert.Equal(DirectionEnum.Backward, insertion.fromDepartureDirection);
            Assert.Equal(DirectionEnum.Backward, insertion.toArrivalDirection);
        }
    }
}