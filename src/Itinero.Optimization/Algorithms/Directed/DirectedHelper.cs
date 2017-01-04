// Itinero.Optimization - Route optimization for .NET
// Copyright (C) 2016 Abelshausen Ben
// 
// This file is part of Itinero.
// 
// Itinero is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// Itinero is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Itinero. If not, see <http://www.gnu.org/licenses/>.

using Itinero.Optimization.Tours;
using System;

namespace Itinero.Optimization.Algorithms.Directed
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
        /// Builds a directed id.
        /// </summary>
        public static int BuildDirectedId(int id, int turn)
        {
            if (turn < 0 || turn >= 4) throw new ArgumentOutOfRangeException("turn");

            return (id * 4) + turn;
        }

        /// <summary>
        /// Extracts the departure id.
        /// </summary>
        public static int ExtractDepartureId(int directedId)
        {
            int arrivalId, departureId, id, turn;
            ExtractAll(directedId, out arrivalId, out departureId, out id, out turn);
            return departureId;
        }

        /// <summary>
        /// Extracts the arrival id.
        /// </summary>
        public static int ExtractArrivalId(int directedId)
        {
            int arrivalId, departureId, id, turn;
            ExtractAll(directedId, out arrivalId, out departureId, out id, out turn);
            return arrivalId;
        }

        /// <summary>
        /// Extracts the original customer id.
        /// </summary>
        public static int ExtractId(int directedId)
        {
            return (directedId - (directedId % 4)) / 4;
        }

        /// <summary>
        /// Extracts the turn.
        /// </summary>
        public static int ExtractTurn(int directedId)
        {
            return directedId % 4;
        }

        /// <summary>
        /// Extracts the turn and id.
        /// </summary>
        public static int ExtractId(int directedId, out int turn)
        {
            turn = (directedId % 4);
            return (directedId - turn) / 4;
        }

        /// <summary>
        /// Builds a turn from an arrival and departure offset.
        /// </summary>
        public static int BuildTurn(int arrivalOffset, int departureOffset)
        {
            return arrivalOffset * 2 + departureOffset;
        }

        /// <summary>
        /// Extracts the arrival and departure offsets.
        /// </summary>
        public static void ExtractOffset(int turn, out int arrivalOffset, out int departureoffset)
        {
            arrivalOffset = 0;
            departureoffset = 0;
            switch (turn)
            {
                case 1:
                    departureoffset = 1;
                    return;
                case 2:
                    arrivalOffset = 1;
                    return;
                case 3:
                    arrivalOffset = 1;
                    departureoffset = 1;
                    return;
            }
        }
        
        /// <summary>
        /// Extracts the arrivalId, departureId, id, and turn.
        /// </summary>
        public static void ExtractAll(int directedId, out int arrivalId, out int departureId, out int id, out int turn)
        {
            turn = directedId % 4;
            var idx = (directedId - turn) / 2;
            id = idx / 2;
            int arrivalOffset, departureOffset;
            ExtractOffset(turn, out arrivalOffset, out departureOffset);
            arrivalId = idx + arrivalOffset;
            departureId = idx + departureOffset;
        }

        /// <summary>
        /// Updates the departure offset.
        /// </summary>
        public static int UpdateDepartureOffset(int directedId, int departureOffset)
        {
            int turn;
            var id = DirectedHelper.ExtractId(directedId, out turn);
            int arrivalOffset, oldDepartureOffset;
            DirectedHelper.ExtractOffset(turn, out arrivalOffset, out oldDepartureOffset);
            return DirectedHelper.BuildDirectedId(id, DirectedHelper.BuildTurn(arrivalOffset, departureOffset));
        }

        /// <summary>
        /// Switches the departure offset.
        /// </summary>
        public static int SwitchDepartureOffset(int directedId)
        {
            int turn;
            var id = DirectedHelper.ExtractId(directedId, out turn);
            int arrivalOffset, departureOffset;
            DirectedHelper.ExtractOffset(turn, out arrivalOffset, out departureOffset);
            return DirectedHelper.BuildDirectedId(id, DirectedHelper.BuildTurn(arrivalOffset, 1 - departureOffset));
        }

        /// <summary>
        /// Updates the arrival offset.
        /// </summary>
        public static int UpdateArrivalOffset(int directedId, int arrivalOffset)
        {
            int turn;
            var id = DirectedHelper.ExtractId(directedId, out turn);
            int oldArrivalOffset, oldDepartureOffset;
            DirectedHelper.ExtractOffset(turn, out oldArrivalOffset, out oldDepartureOffset);
            return DirectedHelper.BuildDirectedId(id, DirectedHelper.BuildTurn(arrivalOffset, oldDepartureOffset));
        }

        /// <summary>
        /// Switches the arrival offset.
        /// </summary>
        public static int SwitchArrivalOffset(int directedId)
        {
            int turn;
            var id = DirectedHelper.ExtractId(directedId, out turn);
            int arrivalOffset, oldDepartureOffset;
            DirectedHelper.ExtractOffset(turn, out arrivalOffset, out oldDepartureOffset);
            return DirectedHelper.BuildDirectedId(id, DirectedHelper.BuildTurn(1 - arrivalOffset, oldDepartureOffset));
        }

        /// <summary>
        /// Gets the minimum weight from customer1 -> customer2.
        /// </summary>
        /// <returns></returns>
        public static float MinWeight(this float[][] weights, int id1, int id2)
        {
            var c1Index = id1 * 2;
            var c2Index = id2 * 2;
            var weight00 = weights[c1Index + 0][c2Index + 0];
            var weight01 = weights[c1Index + 0][c2Index + 1];
            var weight10 = weights[c1Index + 1][c2Index + 0];
            var weight11 = weights[c1Index + 1][c2Index + 1];

            var weight = weight00;
            if (weight > weight01)
            {
                weight = weight01;
            }
            else if(weight > weight10)
            {
                weight = weight10;
            }
            else if (weight > weight11)
            {
                weight = weight11;
            }
            return weight11;
        }

        /// <summary>
        /// Gets the minimum weight from customer1 -> customer3 while inserting customer3 and the best direction and turns to use.
        /// </summary>
        public static float CheapestInsert(this float[][] weights, float[] penalties, int id1, int id2, int id3,
            out int departureOffset1, out int arrivalOffset3, out int turn2)
        {
            var best = float.MaxValue;
            var base1 = id1 * 2;
            var base2 = id2 * 2;
            var base3 = id3 * 2;

            departureOffset1 = Constants.NOT_SET;
            arrivalOffset3 = Constants.NOT_SET;
            turn2 = Constants.NOT_SET;
            for (var do1 = 0; do1 < 2; do1++)
            {
                var d1 = base1 + do1;
                for(var ao3 = 0; ao3 < 2; ao3++)
                {
                    var a3 = base3 + ao3;
                    for (var t = 0; t < 4; t++)
                    {
                        int ao2, do2;
                        DirectedHelper.ExtractOffset(t, out ao2, out do2);

                        var weight = weights[d1][base2 + ao2] +
                            weights[base2 + do2][a3] +
                            penalties[t];
                        if (weight < best)
                        {
                            best = weight;
                            departureOffset1 = do1;
                            arrivalOffset3 = ao3;
                            turn2 = t;
                        }
                    }
                }
            }
            return best;
        }

        /// <summary>
        /// Shifts the directed customer aft er the given directed before with the best possible turns.
        /// </summary>
        public static float ShiftAfterBestTurns(this ITour tour, float[][] weights, float[] penalties, int directedCustomer, int directedBefore)
        {
            var before = DirectedHelper.ExtractId(directedBefore);
            var customer = DirectedHelper.ExtractId(directedCustomer);
            var directedAfter = tour.GetNeigbour(directedBefore);
            if (directedAfter == Constants.NOT_SET)
            { // TODO: figure out if this happens and if so how to calculate turns.
                return 0;
            }
            var after = DirectedHelper.ExtractId(directedAfter);

            // insert directed customer.
            tour.ShiftAfter(directedCustomer, directedBefore);

            // calculate best shift-after.
            int departureOffset1, arrivalOffset3, turn2;
            var cost = DirectedHelper.CheapestInsert(weights, penalties, before, customer, after, out departureOffset1, out arrivalOffset3, out turn2);
            var newDirectedBefore = DirectedHelper.UpdateDepartureOffset(directedBefore, departureOffset1);
            var newDirectedAfter = DirectedHelper.UpdateArrivalOffset(directedAfter, arrivalOffset3);
            var newDirectedCustomer = DirectedHelper.BuildDirectedId(customer, turn2);

            if (directedBefore != newDirectedBefore) { tour.Replace(directedBefore, newDirectedBefore); }
            if (directedCustomer != newDirectedCustomer) { tour.Replace(directedCustomer, newDirectedCustomer); }
            if (directedAfter != newDirectedAfter) { tour.Replace(directedAfter, newDirectedAfter); }

            return cost;
        }

        /// <summary>
        /// Calculates the weights between the two directed id's without the turn weights.
        /// </summary>
        public static float WeightWithoutTurns(this float[][] weights, int directed1, int directed2)
        {
            int arrivalId1, departureId1, id1, turn1, arrivalId2, departureId2, id2, turn2;
            DirectedHelper.ExtractAll(directed1, out arrivalId1, out departureId1, out id1, out turn1);
            DirectedHelper.ExtractAll(directed2, out arrivalId2, out departureId2, out id2, out turn2);

            return weights[departureId1][arrivalId2];
        }

        /// <summary>
        /// Sets the weights between two customers.
        /// </summary>
        public static void SetWeight(this float[][] weights, int id1, int id2,
            float weight)
        {
            weights.SetWeight(id1, id2, weight, weight, weight, weight);
        }

        /// <summary>
        /// Sets the weights between two customers.
        /// </summary>
        public static void SetWeight(this float[][] weights, int id1, int id2,
            float forward1forward2, float forward1backward2, float backward1forward1, float backward1backward2)
        {
            var c1Index = id1 * 2;
            var c2Index = id2 * 2;
            weights[c1Index + 0][c2Index + 0] = forward1forward2;
            weights[c1Index + 0][c2Index + 1] = forward1backward2;
            weights[c1Index + 1][c2Index + 0] = backward1forward1;
            weights[c1Index + 1][c2Index + 1] = backward1backward2;
        }
    }
}