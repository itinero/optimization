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

using Itinero.Optimization.Algorithms.Directed;
using Itinero.Optimization.Tours;

namespace Itinero.Optimization.Algorithms.CheapestInsertion
{
    /// <summary>
    /// Contains extension methods to do cheapest insertion.
    /// </summary>
    public static class CheapestInsertion
    {
        /// <summary>
        /// Inserts the given customer at the best location.
        /// </summary>
        public static void InsertCheapest(this Tour tour, float[][] weights, int customer)
        {
            Pair bestLocation;
            if (tour.CalculateCheapest(weights, customer, out bestLocation) < float.MaxValue)
            {
                tour.InsertAfter(bestLocation.From, customer);
            }
        }

        /// <summary>
        /// Calculates the best position to insert a given customer.
        /// </summary>
        public static float CalculateCheapest(this Tour tour, float[][] weights, int customer, out Pair location)
        {
            var bestCost = float.MaxValue;
            location = new Pair(int.MaxValue, int.MaxValue);

            if (tour.Count == 1)
            {
                var first = tour.First;
                if (tour.IsClosed())
                {
                    bestCost = weights[first][customer] +
                            weights[customer][first];
                    location = new Pair(first, first);
                }
                else
                {
                    bestCost = weights[first][customer];
                    location = new Pair(first, int.MaxValue);
                }
            }
            else
            {
                foreach (var pair in tour.Pairs())
                {
                    var cost = weights[pair.From][customer] +
                        weights[customer][pair.To] -
                        weights[pair.From][pair.To];
                    if (cost < bestCost)
                    {
                        bestCost = cost;
                        location = pair;
                    }
                }
            }
            return bestCost;
        }

        /// <summary>
        /// Inserts the given customer at the best location.
        /// </summary>
        public static float InsertCheapestDirected(this Tour tour, float[][] weights, float[] turnPenalties, int customer)
        {
            Pair bestLocation;
            int departureOffsetFrom, arrivalOffsetTo, turn;
            var cost = tour.CalculateCheapestDirected(weights, turnPenalties, customer, out bestLocation, out departureOffsetFrom, out arrivalOffsetTo, out turn);
            if (cost < float.MaxValue)
            {
                var directedId = DirectedHelper.BuildDirectedId(customer, turn);
                tour.InsertAfter(bestLocation.From, directedId);

                // update departure offset at from.
                var newFromId = DirectedHelper.UpdateDepartureOffset(bestLocation.From, departureOffsetFrom);
                if (bestLocation.From != newFromId)
                {
                    tour.Replace(bestLocation.From, newFromId);
                }
                var newToId = DirectedHelper.UpdateArrivalOffset(bestLocation.To, arrivalOffsetTo);
                if (bestLocation.To != newToId)
                {
                    tour.Replace(bestLocation.To, newToId);
                }
            }
            return cost;
        }

        /// <summary>
        /// Inserts a new customer at the given location using the given turn and departure and arrival directions.
        /// </summary>
        public static void InsertDirected(this Tour tour, int customer, Pair bestLocation, int departureOffsetFrom, int arrivalOffsetTo, int turn)
        {
            var directedId = DirectedHelper.BuildDirectedId(customer, turn);
            tour.InsertAfter(bestLocation.From, directedId);

            // update departure offset at from.
            var newFromId = DirectedHelper.UpdateDepartureOffset(bestLocation.From, departureOffsetFrom);
            if (bestLocation.From != newFromId)
            {
                tour.Replace(bestLocation.From, newFromId);
            }
            var newToId = DirectedHelper.UpdateArrivalOffset(bestLocation.To, arrivalOffsetTo);
            if (bestLocation.To != newToId)
            {
                tour.Replace(bestLocation.To, newToId);
            }
        }

        /// <summary>
        /// Calculates the best position and direction to insert a given customer.
        /// </summary>
        public static float CalculateCheapestDirected(this Tour tour, float[][] weights, float[] turnPenalties, int id, out Pair location,
            out int departureOffsetFrom, out int arrivalOffsetTo, out int turn)
        {
            var bestCost = float.MaxValue;
            departureOffsetFrom = Constants.NOT_SET;
            arrivalOffsetTo = Constants.NOT_SET;
            turn = Constants.NOT_SET;

            location = new Pair(int.MaxValue, int.MaxValue);
            foreach (var pair in tour.Pairs())
            {
                int departureOffset1, arrivalOffset3, turn2;
                var fromId = DirectedHelper.ExtractId(pair.From);
                var toId = DirectedHelper.ExtractId(pair.To);
                var cost = DirectedHelper.WeightWithoutTurns(weights, pair.From, pair.To);
                var ciCost = DirectedHelper.CheapestInsert(weights, turnPenalties, fromId, id, toId, out departureOffset1,
                        out arrivalOffset3, out turn2);
                cost = ciCost - cost;
                if (cost < bestCost)
                {
                    bestCost = cost;
                    location = pair;
                    departureOffsetFrom = departureOffset1;
                    arrivalOffsetTo = arrivalOffset3;
                    turn = turn2;
                }
            }
            return bestCost;
        }
    }
}
