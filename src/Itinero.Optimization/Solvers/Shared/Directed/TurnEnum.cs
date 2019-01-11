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

namespace Itinero.Optimization.Solvers.Shared.Directed
{
    /// <summary>
    /// Enumerates possible directions at a visit.
    /// </summary>
    public enum TurnEnum : byte
    {
        /// <summary>
        /// Arrival forward, departure forward.
        /// </summary>
        ForwardForward = 0,
        /// <summary>
        /// Arrival forward, departure backward, this is a turn.
        /// </summary>
        ForwardBackward = 1,
        /// <summary>
        /// Arrival backward, departure forward, this is a turn.
        /// </summary>
        BackwardForward = 2,
        /// <summary>
        /// Arrival backward, departure backward.
        /// </summary>
        BackwardBackward = 3
    }

    /// <summary>
    /// Contains extension methods for the turn enum.
    /// </summary>
    public static class TurnEnumExtensions
    {
        /// <summary>
        /// Returns a directed visit for the given turn and visit.
        /// </summary>
        /// <param name="turn">The turn.</param>
        /// <param name="visit">The visit.</param>
        /// <returns>A directed visit.</returns>
        public static int DirectedVisit(this TurnEnum turn, int visit)
        {
            return visit * 4 + (byte) turn;
        }
        
        /// <summary>
        /// Gets the departure direction.
        /// </summary>
        /// <param name="turn">The turn.</param>
        /// <returns>The departure direction.</returns>
        public static DirectionEnum Departure(this TurnEnum turn)
        {
            switch (turn)
            {
                case TurnEnum.BackwardBackward:
                case TurnEnum.ForwardBackward:
                    return DirectionEnum.Backward;
                default:
                    return DirectionEnum.Forward;
            }
        }
        
        /// <summary>
        /// Gets the arrival direction.
        /// </summary>
        /// <param name="turn">The turn.</param>
        /// <returns>The arrival direction.</returns>
        public static DirectionEnum Arrival(this TurnEnum turn)
        {
            switch (turn)
            {
                case TurnEnum.BackwardBackward:
                case TurnEnum.BackwardForward:
                    return DirectionEnum.Backward;
                default:
                    return DirectionEnum.Forward;
            }
        }
        
        /// <summary>
        /// Sets the departure direction.
        /// </summary>
        /// <param name="turn">The turn.</param>
        /// <param name="departure">The departure.</param>
        /// <returns>The adjusted turn.</returns>
        public static TurnEnum ApplyDeparture(this TurnEnum turn, DirectionEnum departure)
        {
            switch (departure)
            {
                case DirectionEnum.Forward:
                    switch (turn)
                    {
                        case TurnEnum.BackwardBackward:
                            return TurnEnum.BackwardForward;
                        case TurnEnum.ForwardBackward:
                            return TurnEnum.ForwardForward;
                    }
                    break;
                case DirectionEnum.Backward:
                    switch (turn)
                    {
                        case TurnEnum.BackwardForward:
                            return TurnEnum.BackwardBackward;
                        case TurnEnum.ForwardForward:
                            return TurnEnum.ForwardBackward;
                    }
                    break;

            }

            return turn;
        }
        /// <summary>
        /// Sets the arrival direction.
        /// </summary>
        /// <param name="turn">The turn.</param>
        /// <param name="arrival">The arrival.</param>
        /// <returns>The adjusted turn.</returns>
        public static TurnEnum ApplyArrival(this TurnEnum turn, DirectionEnum arrival)
        {
            switch (arrival)
            {
                case DirectionEnum.Forward:
                    switch (turn)
                    {
                        case TurnEnum.BackwardBackward:
                            return TurnEnum.ForwardBackward;
                        case TurnEnum.BackwardForward:
                            return TurnEnum.ForwardForward;
                    }
                    break;
                case DirectionEnum.Backward:
                    switch (turn)
                    {
                        case TurnEnum.ForwardBackward:
                            return TurnEnum.BackwardBackward;
                        case TurnEnum.ForwardForward:
                            return TurnEnum.BackwardForward;
                    }
                    break;
            }

            return turn;
        }
    }
}