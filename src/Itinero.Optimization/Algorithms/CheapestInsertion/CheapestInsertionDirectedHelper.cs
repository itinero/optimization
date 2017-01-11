// Itinero.Optimization - Route optimization for .NET
// Copyright (C) 2017 Abelshausen Ben
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
    /// Contains extension methods to help with cheapest insertion in a context with directed weights.
    /// </summary>
    public static class CheapestInsertionDirectedHelper
    {
        /// <summary>
        /// Inserts the given customer at the best location if possible.
        /// </summary>
        /// <param name="tour">The tour to insert into.</param>
        /// <param name="weights">The directed weights.</param>
        /// <param name="turnPenalties">The turn pentalties.</param>
        /// <param name="customer">The customer to insert.</param>
        /// <param name="max">The maximum allowed cost of the insertion.</param>
        public static float InsertCheapestDirected(this Tour tour, float[][] weights, float[] turnPenalties, int customer, float max = float.MaxValue)
        {
            if (tour.Count == 1)
            { // there is only one customer, the first one in the route.
                if (tour.First == tour.Last)
                { // there is one customer but it's both the start and the end.
                    var firstId = DirectedHelper.ExtractId(tour.First);

                    var bestCost = float.MaxValue;
                    var bestDirected1Id = int.MaxValue;
                    var bestDirected2Id = int.MaxValue;
                    for (var turn1 = 0; turn1 < 4; turn1++)
                    {
                        var firstDirectedId = DirectedHelper.BuildDirectedId(firstId, turn1);
                        var firstArrivalId = DirectedHelper.ExtractArrivalId(firstDirectedId);
                        var firstDepartureId = DirectedHelper.ExtractDepartureId(firstDirectedId);
                        for (var turn2 = 0; turn2 < 4; turn2++)
                        {
                            var customerDirectedId = DirectedHelper.BuildDirectedId(customer, turn2);
                            var customerArrivalId = DirectedHelper.ExtractArrivalId(customerDirectedId);
                            var customerDepartureId = DirectedHelper.ExtractDepartureId(customerDirectedId);

                            var weight = turnPenalties[turn1];
                            weight += turnPenalties[turn2];
                            weight += weights[firstDepartureId][customerArrivalId];
                            weight += weights[customerDepartureId][firstArrivalId];

                            if (bestCost > weight)
                            {
                                bestDirected1Id = firstDirectedId;
                                bestDirected2Id = customerDirectedId;
                                bestCost = weight;
                            }
                        }
                    }

                    if (bestCost <= max)
                    {
                        tour.Replace(tour.First, bestDirected1Id);
                        tour.InsertAfter(tour.First, bestDirected2Id);

                        return bestCost;
                    }
                }
                else
                { // there is one customer, the last one is not set.
                    var firstId = DirectedHelper.ExtractId(tour.First);

                    int departureOffset1, arrivalOffset2;
                    var cost = DirectedHelper.CheapestInsert(weights, turnPenalties,
                        firstId, customer, out departureOffset1, out arrivalOffset2);
                    if (cost <= max)
                    {
                        var newFirst = DirectedHelper.UpdateDepartureOffset(
                            DirectedHelper.BuildDirectedId(firstId, 0), departureOffset1);
                        var customerDirectedId = DirectedHelper.UpdateArrivalOffset(
                            DirectedHelper.BuildDirectedId(customer, 0), arrivalOffset2);

                        tour.Replace(tour.First, newFirst);
                        tour.InsertAfter(tour.First, customerDirectedId);

                        return cost;
                    }
                }
            }
            else
            { // at least 2 customers already exist, insert a new one in between.
                var cost = float.MaxValue;
                var departureOffsetFrom = Constants.NOT_SET;
                var arrivalOffsetTo = Constants.NOT_SET;
                var turn = Constants.NOT_SET;
                var location = new Pair(int.MaxValue, int.MaxValue);

                foreach (var pair in tour.Pairs())
                {
                    int departureOffset1, arrivalOffset3, turn2;
                    var fromId = DirectedHelper.ExtractId(pair.From);
                    var toId = DirectedHelper.ExtractId(pair.To);
                    var localCost = DirectedHelper.WeightWithoutTurns(weights, pair.From, pair.To);
                    var ciCost = DirectedHelper.CheapestInsert(weights, turnPenalties, fromId, customer, toId, out departureOffset1,
                            out arrivalOffset3, out turn2);
                    localCost = ciCost - localCost;
                    if (localCost < cost)
                    {
                        cost = localCost;
                        location = pair;
                        departureOffsetFrom = departureOffset1;
                        arrivalOffsetTo = arrivalOffset3;
                        turn = turn2;
                    }
                }

                if (cost <= max)
                {
                    var directedId = DirectedHelper.BuildDirectedId(customer, turn);
                    tour.InsertAfter(location.From, directedId);

                    // update departure offset at from.
                    var newFromId = DirectedHelper.UpdateDepartureOffset(location.From, departureOffsetFrom);
                    if (location.From != newFromId)
                    {
                        tour.Replace(location.From, newFromId);
                    }
                    var newToId = DirectedHelper.UpdateArrivalOffset(location.To, arrivalOffsetTo);
                    if (location.To != newToId)
                    {
                        tour.Replace(location.To, newToId);
                    }

                    return cost;
                }
            }
            return 0; // insert failed, probably costs are too high (above given max parameter).
        }
    }
}