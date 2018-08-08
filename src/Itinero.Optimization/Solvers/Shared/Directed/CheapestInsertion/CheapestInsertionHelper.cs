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
using System.Collections.Generic;
using System.Diagnostics;
using Itinero.Algorithms.Weights;
using Itinero.Optimization.Solvers.Tours;

namespace Itinero.Optimization.Solvers.Shared.Directed.CheapestInsertion
{
    /// <summary>
    /// Contains helper methods to do cheapest inserction in the directed case. 
    /// </summary>
    internal static class CheapestInsertionHelper
    {
        /// <summary>
        /// Calculates the best location to insert the given visit taking into account directed weights.
        /// </summary>
        /// <param name="tour">The tour to insert into.</param>
        /// <param name="weightFunc">The weight function.</param>
        /// <param name="insertionCostHeuristic">The insertion cost heuristic.</param>
        /// <param name="canPlace">The can place function, use this to check constraints.</param>
        /// <param name="turnPenalty">The turn penalty function.</param>
        /// <param name="visits">The visits to place.</param>
        /// <returns>The data about the cheapest insert.</returns>
        public static (float cost, int visit, TurnEnum turn, Pair location, DirectionEnum fromDepartureDirection, DirectionEnum
            toArrivalDirection) CalculateCheapestDirected(this Tour tour, IEnumerable<int> visits,
                Func<int, int, float> weightFunc, Func<TurnEnum, float> turnPenalty,
                Func<int, float> insertionCostHeuristic = null, Func<float, int, bool> canPlace = null)
        {
            (float cost, int visit, TurnEnum turn, Pair location, DirectionEnum fromDepartureDirection, DirectionEnum toArrivalDirection) best
                = (float.MaxValue, int.MaxValue, TurnEnum.ForwardForward, new Pair(), DirectionEnum.Forward, DirectionEnum.Forward);
            
            foreach (var visit in visits)
            {
                var current = tour.CalculateCheapestDirected(visit, weightFunc, turnPenalty, insertionCostHeuristic, canPlace);
                if (current.cost < best.cost)
                {
                    best = (current.cost, visit, current.turn, current.location, current.fromDepartureDirection,
                        current.toArrivalDirection);
                }
            }
 
            return best;
        }

        /// <summary>
        /// Calculates the best location to insert the given visit taking into account directed weights.
        /// </summary>
        /// <param name="tour">The tour to insert into.</param>
        /// <param name="weightFunc">The weight function.</param>
        /// <param name="insertionCostHeuristic">The insertion cost heuristic.</param>
        /// <param name="canPlace">The can place function, use this to check constraints.</param>
        /// <param name="turnPenalty">The turn penalty function.</param>
        /// <param name="visit">The visit to place.</param>
        /// <returns>The data about the cheapest insert.</returns>
        public static (float cost, TurnEnum turn, Pair location, DirectionEnum fromDepartureDirection, DirectionEnum toArrivalDirection) CalculateCheapestDirected(this Tour tour, int visit, 
            Func<int, int, float> weightFunc, Func<TurnEnum, float> turnPenalty,
            Func<int, float> insertionCostHeuristic = null, Func<float, int, bool> canPlace = null)
        {
            if (tour.Count == 1)
            {
                if (tour.IsClosedDirected())
                { // this is a closed loop.
                    var location = new Pair(tour.First, tour.Last.Value);
                    var result = location.CalculateCheapestDirected(visit, weightFunc, turnPenalty);
                    return (result.cost, result.turn, location, result.fromDepartureDirection,
                        result.toArrivalDirection);
                }
                else
                { // this is an open loop with just one visit.
                    var location = new Pair(tour.First, -1);
                    var result = location.CalculateCheapestDirected(visit, weightFunc, turnPenalty);
                    return (result.cost, result.turn, location, result.fromDepartureDirection,
                        result.toArrivalDirection);
                }
            }
            else
            {
                (float cost, TurnEnum turn, Pair location, DirectionEnum fromDepartureDirection, DirectionEnum toArrivalDirection) best
                    = (float.MaxValue, TurnEnum.ForwardForward, new Pair(), DirectionEnum.Forward, DirectionEnum.Forward);
                foreach (var pair in tour.Pairs())
                {
                    var result = pair.CalculateCheapestDirected(visit, weightFunc, turnPenalty);
                    if (best.cost > result.cost)
                    {
                        best = (result.cost, result.turn, pair, result.fromDepartureDirection,
                            result.toArrivalDirection);
                    }
                }
                return best;
            }
        }

        /// <summary>
        /// Calculates the cheapest way to insert the given visit between the two given visits.
        /// </summary>
        /// <param name="pair">The pair.</param>
        /// <param name="visit">The visit.</param>
        /// <param name="weightFunc">The weight function.</param>
        /// <param name="turnPenaltyFunc">The turn penalty function.</param>
        /// <returns>The data on the cheapest insert found.</returns>
        public static (float cost, TurnEnum turn, DirectionEnum fromDepartureDirection, DirectionEnum toArrivalDirection) CalculateCheapestDirected(
                this Pair pair, int visit, Func<int, int, float> weightFunc, Func<TurnEnum, float> turnPenaltyFunc)
        {
            Debug.Assert(pair.From >= 0);

            if (pair.To == -1)
            {
                // don't care about 'to', cheapest insert after 'from'.
                var from = DirectedHelper.Extract(pair.From);

                (float cost, TurnEnum turn, DirectionEnum fromDepartureDirection, DirectionEnum toArrivalDirection)
                    current = (float.MaxValue, TurnEnum.ForwardForward, DirectionEnum.Forward, DirectionEnum.Forward);
                var best = current;
                for (var dd = 0; dd < 2; dd++)
                for (var ad = 0; ad < 2; ad++)
                {
                    current.fromDepartureDirection = (DirectionEnum) dd;
                    var turnArrival = (DirectionEnum) ad;
                    current.turn = current.turn.ApplyArrival(turnArrival);

                    // use the cost between 'from -> visit'.
                    current.cost = weightFunc(DirectedHelper.WeightId(from.visit, current.fromDepartureDirection),
                        DirectedHelper.WeightId(visit, turnArrival));

                    if (current.cost < best.cost)
                    {
                        best = current;
                    }
                }

                return best;
            }
            else
            { // cheapest insert between the two.
                var from = DirectedHelper.Extract(pair.From);
                var to = DirectedHelper.Extract(pair.To);
            
                (float cost, TurnEnum turn, DirectionEnum fromDepartureDirection, DirectionEnum toArrivalDirection) current = 
                    (float.MaxValue, TurnEnum.ForwardForward, DirectionEnum.Forward, DirectionEnum.Forward);
                var best = current;
                for (var t = 0; t < 4; t++)
                {
                    current.turn = (TurnEnum) t;
                    for (var dd = 0; dd < 2; dd++)
                    for (var ad = 0; ad < 2; ad++)
                    {
                        current.fromDepartureDirection = (DirectionEnum) dd;
                        current.toArrivalDirection = (DirectionEnum) ad;

                        current.cost = 0;
                        // add turn penalty of turn at 'from'.
                        current.cost = turnPenaltyFunc(from.turn.ApplyDeparture(current.fromDepartureDirection));
                        // add travel weight 'from -> visit'.
                        current.cost += weightFunc(DirectedHelper.WeightId(from.visit, current.fromDepartureDirection),
                            DirectedHelper.WeightId(visit, current.turn.Arrival()));
                        // add turning cost at 'visit'.
                        current.cost += turnPenaltyFunc(current.turn);
                        // add travel weight 'visit -> 'to'.
                        current.cost += weightFunc(DirectedHelper.WeightId(visit, current.turn.Departure()),
                            DirectedHelper.WeightId(to.visit, current.toArrivalDirection));
                        // add turning cost at 'to'.
                        current.cost += turnPenaltyFunc(to.turn.ApplyArrival(current.toArrivalDirection));

                        if (current.cost < best.cost)
                        {
                            best = current;
                        }
                    }
                }

                return best;
            }
        }
    }
}