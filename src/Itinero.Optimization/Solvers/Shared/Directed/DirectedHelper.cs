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
    public static class DirectedHelper
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
            return ((directedVisit - (int)turn) / 2) + (int)(turn.Arrival());
        }

        /// <summary>
        /// Extracts the departure weight id.
        /// </summary>
        /// <param name="directedVisit">The directed visit.</param>
        /// <returns>The departure weight id.</returns>
        public static int ExtractDepartureWeightId(int directedVisit)
        {
            var turn = (TurnEnum)(directedVisit % 4);
            return ((directedVisit - (int)turn) / 2) + (int)(turn.Departure());
        }
        
        /// <summary>
        /// Calculates the total weight.
        /// </summary>
        /// <param name="tour">The tour.</param>
        /// <param name="weightsFunc">The weight function.</param>
        /// <param name="turnPenaltyFunc">The turn penalty function.</param>
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
                var travelWeight = weightsFunc(previous1.turn.Departure().WeightId(previous1.visit),
                    current.turn.Arrival().WeightId(current.visit));
                if (travelWeight >= float.MaxValue)
                { // tour is impossible.
                    return float.MaxValue;
                }
                weight += travelWeight;

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
                var travelWeight = weightsFunc(previous1.turn.Departure().WeightId(previous1.visit),
                    current.turn.Arrival().WeightId(current.visit));
                if (travelWeight >= float.MaxValue)
                { // tour is impossible.
                    return float.MaxValue;
                }
                weight += travelWeight;
            }

            // add last turn cost for the previous(-2) if any.
            if (previous2.visit != -1)
            {
                weight += turnPenaltyFunc(previous2.turn);
            }
            
            return weight;
        }
    }
}