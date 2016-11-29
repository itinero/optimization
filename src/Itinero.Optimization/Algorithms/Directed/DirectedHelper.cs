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
        /// Extracts the arrivalId, departureId, id, and turn.
        /// </summary>
        public static void ExtractAll(int directedId, out int arrivalId, out int departureId, out int id, out int turn)
        {
            turn = directedId % 4;
            var idx = (directedId - turn) / 2;
            id = idx / 2;
            switch (turn)
            {
                case 0:
                    arrivalId = idx + 0;
                    departureId = idx + 0;
                    return;
                case 1:
                    arrivalId = idx + 0;
                    departureId = idx + 1;
                    return;
                case 2:
                    arrivalId = idx + 1;
                    departureId = idx + 0;
                    return;
                case 3:
                    arrivalId = idx + 1;
                    departureId = idx + 1;
                    return;
            }
            arrivalId = int.MaxValue;
            departureId = int.MaxValue;
            id = int.MaxValue;
            turn = int.MaxValue;
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
        /// Calculates the weights between the two directed id's without the turn weights.
        /// </summary>
        public static float WeightWithoutTurns(this float[][] weights, int directed1, int directed2)
        {
            int arrivalId1, departureId1, id1, turn1, arrivalId2, departureId2, id2, turn2;
            DirectedHelper.ExtractAll(directed1, out arrivalId1, out departureId1, out id1, out turn1);
            DirectedHelper.ExtractAll(directed2, out arrivalId2, out departureId2, out id2, out turn2);

            return weights[departureId1][arrivalId2];
        }
    }
}