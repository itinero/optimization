﻿/*
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
using Itinero.Optimization.Solvers.Tours;

namespace Itinero.Optimization.Solvers.Shared.Directed
{
    /// <summary>
    /// An helper class to handle directed-aware id's with direction-aware weights.
    /// </summary>
    /// <remarks>
    /// 
    /// - id            = The customer id, indepedent of turns or directions.
    /// - directedId    = What happens at a customer, do we u-turn at the customer, go straight forward, backward or u-turn in the other direction.
    /// - departureId   = How we depart from a customer, do we depart 'left' or 'right'.
    /// - arrivalId     = How we arrive at a customer, do we arrive from 'left' or 'right'.
    /// - turn          = A turn:
    ///                     - 0: ------X------- forward
    ///                     - 1: ------X        u-turn
    ///                     - 2:       X------- other u-turn
    ///                     - 3: ------X------- backward
    ///                     
    ///  A directedId contains:
    ///  - id           : the regular customer id.
    ///  - turn         : the turn taken at that customer, the turn defines:
    ///         - arrivalId    : the arrival id at that customer.
    ///         - departureId  : the departure id at that customer.
    ///         
    /// </remarks>
    internal static class DirectedHelper
    {
        /// <summary>
        /// Builds the weight id for the given visit and given direction.
        /// </summary>
        /// <param name="visit">The visit.</param>
        /// <param name="direction">The direction.</param>
        /// <returns>The weight id.</returns>
        public static int WeightId(int visit, DirectionEnum direction)
        {
            return (visit * 2) + (byte) direction;
        }
        
        /// <summary>
        /// Builds the weight id for the given directed visit and given direction.
        /// </summary>
        /// <param name="directedVisit">The directed visit.</param>
        /// <returns>The weight id.</returns>
        public static int WeightIdArrival(int directedVisit)
        {
            var extracted = Extract(directedVisit);
            return WeightId(extracted.visit, extracted.turn.Arrival());
        }
        
        /// <summary>
        /// Builds the weight id for the given directed visit and given direction.
        /// </summary>
        /// <param name="directedVisit">The directed visit.</param>
        /// <returns>The weight id.</returns>
        public static int WeightIdDeparture(int directedVisit)
        {
            var extracted = Extract(directedVisit);
            return WeightId(extracted.visit, extracted.turn.Departure());
        }
        
        /// <summary>
        /// Extracts the original visit.
        /// </summary>
        /// <param name="directedVisit">The directed visit.</param>
        /// <returns>The original visit id.</returns>
        public static int ExtractVisit(int directedVisit)
        {
            return (directedVisit - (directedVisit % 4)) / 4;
        }
        
        /// <summary>
        /// Extracts the turn.
        /// </summary>
        /// <param name="directedVisit">The directed visit.</param>
        /// <returns>The turn taken at the given visit.</returns>
        public static TurnEnum ExtractTurn(int directedVisit)
        {
            return (TurnEnum)(directedVisit % 4);
        }
        
        /// <summary>
        /// Extracts the turn and id.
        /// </summary>
        /// <param name="directedVisit">The directed visit.</param>
        /// <returns></returns>
        public static (int visit, TurnEnum turn) Extract(int directedVisit)
        {
            var turn = (directedVisit % 4);
            return ((directedVisit - turn) / 4, (TurnEnum)turn);
        }

        /// <summary>
        /// Extracts the arrival weight id.
        /// </summary>
        /// <param name="directedVisit">The directed visit.</param>
        /// <returns>The arrival weight id.</returns>
        public static int ExtractArrivalWeightId(int directedVisit)
        {
            var turn = (TurnEnum)(directedVisit % 4);
            return (directedVisit / 2) + (int)turn.Arrival();
        }

        /// <summary>
        /// Extracts the departure weight id.
        /// </summary>
        /// <param name="directedVisit">The directed visit.</param>
        /// <returns>The departure weight id.</returns>
        public static int ExtractDepartureWeightId(int directedVisit)
        {
            var turn = (TurnEnum)(directedVisit % 4);
            return (directedVisit / 2) + (int)turn.Departure();
        }
        
        /// <summary>
        /// Calculates the total weight.
        /// </summary>
        /// <param name="tour">The tour.</param>
        /// <param name="weightsFunc">The weight function.</param>
        /// <returns>The total weight.</returns>
        internal static float WeightDirected(this Tour tour, Func<int, int, float> weightsFunc, Func<TurnEnum, float> turnPenaltyFunc)
        {
            var weight = 0f;

            (int visit, TurnEnum turn) previous1 = (-1, TurnEnum.ForwardForward);
            (int visit, TurnEnum turn) previous2 = (-1, TurnEnum.ForwardForward);
            foreach (var directedVisit in tour)
            {
                var current = Extract(directedVisit);
                if (previous1.visit == -1)
                {
                    previous1 = current;
                    continue;
                }

                // add travel weight.
                weight += weightsFunc(previous1.turn.Departure().WeightId(previous1.visit),
                    current.turn.Arrival().WeightId(current.visit));

                // add turn cost for the previous(-2) if any.
                if (previous2.visit != -1)
                {
                    weight += turnPenaltyFunc(previous2.turn);
                }
                
                // prepare for next iteration.
                previous1 = current;
                previous2 = previous1;
            }

            if (tour.IsClosedDirected() &&
                previous1.visit != -1)
            {
                var current = Extract(tour.First);
                weight += weightsFunc(previous1.turn.Departure().WeightId(previous1.visit),
                    current.turn.Arrival().WeightId(current.visit));
            }

            // add last turn cost for the previous(-2) if any.
            if (previous2.visit != -1)
            {
                weight += turnPenaltyFunc(previous2.turn);
            }
            
            return weight;
        }
        
//
//        /// <summary>
//        /// Builds a turn from an arrival and departure offset.
//        /// </summary>
//        public static int BuildTurn(int arrivalOffset, int departureOffset)
//        {
//            return arrivalOffset * 2 + departureOffset;
//        }
//
//        /// <summary>
//        /// Extracts the arrival and departure offsets.
//        /// </summary>
//        public static void ExtractOffset(int turn, out int arrivalOffset, out int departureoffset)
//        {
//            arrivalOffset = 0;
//            departureoffset = 0;
//            switch (turn)
//            {
//                case 1:
//                    departureoffset = 1;
//                    return;
//                case 2:
//                    arrivalOffset = 1;
//                    return;
//                case 3:
//                    arrivalOffset = 1;
//                    departureoffset = 1;
//                    return;
//            }
//        }
//        
//        /// <summary>
//        /// Extracts the arrivalId, departureId, id, and turn.
//        /// </summary>
//        public static void ExtractAll(int directedId, out int arrivalId, out int departureId, out int id, out int turn)
//        {
//            turn = directedId % 4;
//            var idx = (directedId - turn) / 2;
//            id = idx / 2;
//            int arrivalOffset, departureOffset;
//            ExtractOffset(turn, out arrivalOffset, out departureOffset);
//            arrivalId = idx + arrivalOffset;
//            departureId = idx + departureOffset;
//        }
//
//        /// <summary>
//        /// Updates the departure offset.
//        /// </summary>
//        public static int UpdateDepartureOffset(int directedId, int departureOffset)
//        {
//            int turn;
//            var id = DirectedHelper.ExtractId(directedId, out turn);
//            int arrivalOffset, oldDepartureOffset;
//            DirectedHelper.ExtractOffset(turn, out arrivalOffset, out oldDepartureOffset);
//            return DirectedHelper.BuildDirectedId(id, DirectedHelper.BuildTurn(arrivalOffset, departureOffset));
//        }
//
//        /// <summary>
//        /// Switches the departure offset.
//        /// </summary>
//        public static int SwitchDepartureOffset(int directedId)
//        {
//            int turn;
//            var id = DirectedHelper.ExtractId(directedId, out turn);
//            int arrivalOffset, departureOffset;
//            DirectedHelper.ExtractOffset(turn, out arrivalOffset, out departureOffset);
//            return DirectedHelper.BuildDirectedId(id, DirectedHelper.BuildTurn(arrivalOffset, 1 - departureOffset));
//        }
//
//        /// <summary>
//        /// Updates the arrival offset.
//        /// </summary>
//        public static int UpdateArrivalOffset(int directedId, int arrivalOffset)
//        {
//            int turn;
//            var id = DirectedHelper.ExtractId(directedId, out turn);
//            int oldArrivalOffset, oldDepartureOffset;
//            DirectedHelper.ExtractOffset(turn, out oldArrivalOffset, out oldDepartureOffset);
//            return DirectedHelper.BuildDirectedId(id, DirectedHelper.BuildTurn(arrivalOffset, oldDepartureOffset));
//        }
//
//        /// <summary>
//        /// Switches the arrival offset.
//        /// </summary>
//        public static int SwitchArrivalOffset(int directedId)
//        {
//            int turn;
//            var id = DirectedHelper.ExtractId(directedId, out turn);
//            int arrivalOffset, oldDepartureOffset;
//            DirectedHelper.ExtractOffset(turn, out arrivalOffset, out oldDepartureOffset);
//            return DirectedHelper.BuildDirectedId(id, DirectedHelper.BuildTurn(1 - arrivalOffset, oldDepartureOffset));
//        }
//
//        /// <summary>
//        /// Gets the minimum weight from customer1 -> customer2.
//        /// </summary>
//        /// <returns></returns>
//        public static float MinWeight(this float[][] weights, int id1, int id2)
//        {
//            var c1Index = id1 * 2;
//            var c2Index = id2 * 2;
//            var weight00 = weights[c1Index + 0][c2Index + 0];
//            var weight01 = weights[c1Index + 0][c2Index + 1];
//            var weight10 = weights[c1Index + 1][c2Index + 0];
//            var weight11 = weights[c1Index + 1][c2Index + 1];
//
//            var weight = weight00;
//            if (weight > weight01)
//            {
//                weight = weight01;
//            }
//            else if(weight > weight10)
//            {
//                weight = weight10;
//            }
//            else if (weight > weight11)
//            {
//                weight = weight11;
//            }
//            return weight11;
//        }
//
//        /// <summary>
//        /// Gets the average weight from customer1 -> customer2.
//        /// </summary>
//        /// <returns></returns>
//        public static float AverageWeight(this float[][] weights, int id1, int id2)
//        {
//            var c1Index = id1 * 2;
//            var c2Index = id2 * 2;
//            var weight00 = weights[c1Index + 0][c2Index + 0];
//            var weight01 = weights[c1Index + 0][c2Index + 1];
//            var weight10 = weights[c1Index + 1][c2Index + 0];
//            var weight11 = weights[c1Index + 1][c2Index + 1];
//
//            var samples = 0;
//            var weight = 0f;
//            if (weight01 != float.MaxValue)
//            {
//                weight += weight01;
//                samples++;
//            }
//            if (weight10 != float.MaxValue)
//            {
//                weight += weight10;
//                samples++;
//            }
//            if (weight11 != float.MaxValue)
//            {
//                weight += weight11;
//                samples++;
//            }
//            if (weight00 != float.MaxValue)
//            {
//                weight += weight00;
//                samples++;
//            }
//            if (samples == 0)
//            {
//                return float.MaxValue;
//            }
//            return weight / samples;
//        }
//
//        /// <summary>
//        /// Gets the minimum weight from customer1 -> customer2 while inserting customer2. Outputs the best departure and arrival offsets.
//        /// </summary>
//        public static float CheapestInsert(this float[][] weights, float[] penalties, int id1, int id2,
//            out int departureOffset1, out int arrivalOffset2)
//        {
//            var best = float.MaxValue;
//            var base1 = id1 * 2;
//            var base2 = id2 * 2;
//
//            departureOffset1 = -1;
//            arrivalOffset2 = -1;
//
//            for (var do1 = 0; do1 < 2; do1++)
//            {
//                var departureId1 = base1 + do1;
//                for (var ao2 = 0; ao2 < 2; ao2++)
//                {
//                    var arrivalId2 = base2 + ao2;
//
//                    var weight = weights[departureId1][arrivalId2];
//
//                    if (weight < best)
//                    {
//                        departureOffset1 = do1;
//                        arrivalOffset2 = ao2;
//
//                        best = weight;
//                    }
//                }
//            }
//            return best;
//        }
//
//        /// <summary>
//        /// Gets the minimum weight from customer1 -> customer2 while inserting customer2. Outputs the best departure and arrival offsets.
//        /// </summary>
//        public static Weight CheapestInsert(this Weight[][] weights, Weight[] penalties, int id1, int id2, ProfileMetric metric,
//            out int departureOffset1, out int arrivalOffset2)
//        {
//            var best = Itinero.Algorithms.Weights.Weight.MaxValue;
//            var base1 = id1 * 2;
//            var base2 = id2 * 2;
//
//            departureOffset1 = -1;
//            arrivalOffset2 = -1;
//
//            for (var do1 = 0; do1 < 2; do1++)
//            {
//                var departureId1 = base1 + do1;
//                for (var ao2 = 0; ao2 < 2; ao2++)
//                {
//                    var arrivalId2 = base2 + ao2;
//
//                    var weight = weights[departureId1][arrivalId2];
//
//                    if (weight.GetForMetric(metric) < best.GetForMetric(metric))
//                    {
//                        departureOffset1 = do1;
//                        arrivalOffset2 = ao2;
//
//                        best = weight;
//                    }
//                }
//            }
//            return best;
//        }
//
//        /// <summary>
//        /// Shifts the directed customer aft er the given directed before with the best possible turns.
//        /// </summary>
//        public static float ShiftAfterBestTurns(this Tour tour, float[][] weights, float[] penalties, int directedCustomer, int directedBefore)
//        {
//            var customer = DirectedHelper.ExtractId(directedCustomer);
//            var directedAfter = tour.GetNeigbour(directedCustomer);
//            if (directedAfter == Constants.NOT_SET)
//            { // TODO: figure out if this happens and if so how to calculate turns.
//                return 0;
//            }
//
//            // insert directed customer.
//            tour.ShiftAfter(directedCustomer, directedBefore);
//
//            // calculate best shift-after.
//            int departureOffset1, arrivalOffset3, turn2;
//            var cost = CheapestInsertion.CheapestInsertionDirectedHelper.CalculateCheapestInsert(weights, penalties, directedBefore, customer, directedAfter, 
//                !tour.IsFirst(directedBefore), !tour.IsLast(directedAfter), out departureOffset1, out arrivalOffset3, out turn2);
//            var newDirectedBefore = DirectedHelper.UpdateDepartureOffset(directedBefore, departureOffset1);
//            var newDirectedAfter = DirectedHelper.UpdateArrivalOffset(directedAfter, arrivalOffset3);
//            var newDirectedCustomer = DirectedHelper.BuildDirectedId(customer, turn2);
//
//            if (directedBefore != newDirectedBefore) { tour.Replace(directedBefore, newDirectedBefore); }
//            if (directedCustomer != newDirectedCustomer) { tour.Replace(directedCustomer, newDirectedCustomer); }
//            if (directedAfter != newDirectedAfter) { tour.Replace(directedAfter, newDirectedAfter); }
//
//            return cost;
//        }
//
//        /// <summary>
//        /// Calculates the weights between the two directed id's without the turn weights.
//        /// </summary>
//        public static float WeightWithoutTurns(this float[][] weights, int directed1, int directed2)
//        {
//            int arrivalId1, departureId1, id1, turn1, arrivalId2, departureId2, id2, turn2;
//            DirectedHelper.ExtractAll(directed1, out arrivalId1, out departureId1, out id1, out turn1);
//            DirectedHelper.ExtractAll(directed2, out arrivalId2, out departureId2, out id2, out turn2);
//
//            return weights[departureId1][arrivalId2];
//        }
//
//        /// <summary>
//        /// Calculates the weight of the given tour.
//        /// </summary>
//        public static float Weight(this Tour tour, float[][] weights, float[] turnPenalties)
//        {
//            var weight = 0f;
//            var previousFrom = int.MaxValue;
//            var firstTo = int.MaxValue;
//            var lastTurn = int.MaxValue;
//            foreach (var directedId in tour)
//            {
//                // extract turns and stuff from directed id.
//                int arrivalId, departureId, id, turn;
//                DirectedHelper.ExtractAll(directedId, out arrivalId, out departureId, out id, out turn);
//
//                // add the weight from the previous customer to the current one.
//                if (previousFrom != int.MaxValue)
//                {
//                    weight = weight + weights[previousFrom][arrivalId];
//                }
//                else
//                {
//                    firstTo = arrivalId;
//                }
//
//                // add turn penalty.
//                if ((tour.First != directedId && 
//                     tour.Last != directedId) ||
//                    tour.Last == tour.First)
//                { // never count turn @ first customer.
//                    // don't count turn @ last customer if it's different from first.
//                    weight += turnPenalties[turn];
//                }
//
//                previousFrom = departureId;
//                lastTurn = turn;
//            }
//
//            // remove the last turn penalty if it's an open non-fixed tour.
//            if (tour.Last == null)
//            {
//                weight -= turnPenalties[lastTurn];
//            }
//
//            // add the weight between last and first.
//            if (previousFrom != int.MaxValue &&
//                tour.First == tour.Last)
//            {
//                weight = weight + weights[previousFrom][firstTo];
//            }
//            return weight;
//        }
//
//        /// <summary>
//        /// Sets the weights between two customers.
//        /// </summary>
//        public static void SetWeight(this float[][] weights, int id1, int id2,
//            float weight)
//        {
//            weights.SetWeight(id1, id2, weight, weight, weight, weight);
//        }
//
//        /// <summary>
//        /// Sets the weights between two customers.
//        /// </summary>
//        public static void SetWeight(this float[][] weights, int id1, int id2,
//            float forward1forward2, float forward1backward2, float backward1forward1, float backward1backward2)
//        {
//            var c1Index = id1 * 2;
//            var c2Index = id2 * 2;
//            weights[c1Index + 0][c2Index + 0] = forward1forward2;
//            weights[c1Index + 0][c2Index + 1] = forward1backward2;
//            weights[c1Index + 1][c2Index + 0] = backward1forward1;
//            weights[c1Index + 1][c2Index + 1] = backward1backward2;
//        }
//
//        /// <summary>
//        /// Switches the given directed id to the best turn given the before and after directed id's.
//        /// </summary>
//        public static bool SwitchToBestTurn(int directedId, int beforeDirectedId, int afterDirectedId, float[][] weights, float[] turnPenalties,
//            out int betterDirectedId, out bool departureIdSwitched, out bool arrivalIdSwitched, out float delta)
//        {
//            int arrivalId, departureId, turn, id, beforeDepartureOffset, afterArrivalOffset, temp;
//            DirectedHelper.ExtractOffset(DirectedHelper.ExtractTurn(beforeDirectedId), out temp, out beforeDepartureOffset);
//            DirectedHelper.ExtractOffset(DirectedHelper.ExtractTurn(afterDirectedId), out afterArrivalOffset, out temp);
//            DirectedHelper.ExtractAll(directedId, out arrivalId, out departureId, out id, out turn);
//
//            var bestWeight = float.MaxValue;
//            var bestDirectedId = -1;
//            var previousWeight = float.MaxValue;
//            departureIdSwitched = false;
//            arrivalIdSwitched = false;
//            for (var newBeforeDepartureOffset = 0; newBeforeDepartureOffset < 2; newBeforeDepartureOffset++)
//            {
//                for (var newAfterArrivalOffset = 0; newAfterArrivalOffset < 2; newAfterArrivalOffset++)
//                {
//                    for (var i = 0; i < 4; i++)
//                    {
//                        var newDirectedId = DirectedHelper.BuildDirectedId(id, i);
//                        int localTurn;
//                        DirectedHelper.ExtractAll(newDirectedId, out arrivalId, out departureId, out id, out localTurn);
//                        var newBeforeDirectedId = DirectedHelper.UpdateDepartureOffset(beforeDirectedId, newBeforeDepartureOffset);
//                        var newAfterDirectedId = DirectedHelper.UpdateArrivalOffset(afterDirectedId, newAfterArrivalOffset);
//                        var newBeforeDepartureId = DirectedHelper.ExtractDepartureId(newBeforeDirectedId);
//                        var newAfterArrivalId = DirectedHelper.ExtractArrivalId(newAfterDirectedId);
//                        var newBeforeTurn = DirectedHelper.ExtractTurn(newBeforeDirectedId);
//                        var newAfterTurn = DirectedHelper.ExtractTurn(newAfterDirectedId);
//
//                        var newWeight = turnPenalties[localTurn];
//                        newWeight += weights[newBeforeDepartureId][arrivalId];
//                        newWeight += weights[departureId][newAfterArrivalId];
//                        newWeight += turnPenalties[newBeforeTurn];
//                        newWeight += turnPenalties[newAfterTurn];
//
//                        if (i == turn && newBeforeDepartureOffset == beforeDepartureOffset &&
//                            newAfterArrivalOffset == afterArrivalOffset)
//                        {
//                            previousWeight = newWeight;
//                        }
//
//                        if (bestWeight > newWeight)
//                        {
//                            bestWeight = newWeight;
//                            bestDirectedId = newDirectedId;
//                            departureIdSwitched = newBeforeDepartureOffset != beforeDepartureOffset;
//                            arrivalIdSwitched = newAfterArrivalOffset != afterArrivalOffset;
//                        }
//                    }
//                }
//            }
//            delta = previousWeight - bestWeight;
//            betterDirectedId = bestDirectedId;
//            return betterDirectedId != directedId;
//        }
    }
}