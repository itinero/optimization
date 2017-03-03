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

using Itinero.Algorithms.Weights;
using Itinero.Optimization.Algorithms.Directed;
using Itinero.Optimization.Tours;
using Itinero.Profiles;

namespace Itinero.Optimization.Algorithms.CheapestInsertion
{
    /// <summary>
    /// Contains extension methods to help with cheapest insertion in a context with directed weights.
    /// </summary>
    public static class CheapestInsertionDirectedHelper
    {
        /// <summary>
        /// Gets the minimum weight from customer1 -> customer3 while inserting customer2 and the best direction and turns to use.
        /// </summary>
        public static float CalculateCheapestInsert(this float[][] weights, float[] penalties, int directedId1, int id2, int directedId3,
            bool includeTurn1, bool includeTurn3, out int departureOffset1, out int arrivalOffset3, out int turn2)
        {
            // extract existing data.
            int id1, turn1, arrivalId1, departureId1, arrivalOffset1, temp;
            DirectedHelper.ExtractAll(directedId1, out arrivalId1, out departureId1, out id1, out turn1);
            DirectedHelper.ExtractOffset(turn1, out arrivalOffset1, out temp);
            int id3, turn3, arrivalId3, departureId3, departureOffset3;
            DirectedHelper.ExtractAll(directedId3, out arrivalId3, out departureId3, out id3, out turn3);
            DirectedHelper.ExtractOffset(turn3, out temp, out departureOffset3);

            // calculate current weight.
            var weightBefore = weights[departureId1][arrivalId3];
            if (includeTurn1)
            {
                weightBefore += penalties[turn1];
            }
            if (includeTurn3)
            {
                weightBefore += penalties[turn3];
            }

            // evaluate all possibilities.
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
                var turn1Weight = 0f;
                if (includeTurn1)
                {
                    turn1 = DirectedHelper.BuildTurn(arrivalOffset1, do1);
                    turn1Weight = penalties[turn1];
                }
                for (var ao3 = 0; ao3 < 2; ao3++)
                {
                    var a3 = base3 + ao3;
                    var turn3Weight = 0f;
                    if (includeTurn3)
                    {
                        turn3 = DirectedHelper.BuildTurn(departureOffset3, ao3);
                        turn3Weight = penalties[turn3];
                    }
                    for (var t = 0; t < 4; t++)
                    {
                        int ao2, do2;
                        DirectedHelper.ExtractOffset(t, out ao2, out do2);

                        var weight = weights[d1][base2 + ao2] +
                            weights[base2 + do2][a3] +
                            penalties[t] + turn1Weight +
                            turn3Weight;

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
            return best - weightBefore;
        }

        /// <summary>
        /// Gets the minimum weight from customer1 -> customer3 while inserting customer2 and the best direction and turns to use.
        /// </summary>
        public static Weight CalculateCheapestInsert(this Weight[][] weights, Weight[] penalties, ProfileMetric metric, 
            int directedId1, int id2, int directedId3, bool includeTurn1, bool includeTurn3, out int departureOffset1, out int arrivalOffset3,
            out int turn2)
        {
            // extract existing data.
            int id1, turn1, arrivalId1, departureId1, arrivalOffset1, temp;
            DirectedHelper.ExtractAll(directedId1, out arrivalId1, out departureId1, out id1, out turn1);
            DirectedHelper.ExtractOffset(turn1, out arrivalOffset1, out temp);
            int id3, turn3, arrivalId3, departureId3, departureOffset3;
            DirectedHelper.ExtractAll(directedId3, out arrivalId3, out departureId3, out id3, out turn3);
            DirectedHelper.ExtractOffset(turn3, out temp, out departureOffset3);

            // calculate current weight.
            var weightBefore = weights[departureId1][arrivalId3];
            if (includeTurn1)
            {
                weightBefore += penalties[turn1];
            }
            if (includeTurn3)
            {
                weightBefore += penalties[turn3];
            }

            // evaluate all possibilities.
            var best = Weight.MaxValue;
            var base1 = id1 * 2;
            var base2 = id2 * 2;
            var base3 = id3 * 2;
            departureOffset1 = Constants.NOT_SET;
            arrivalOffset3 = Constants.NOT_SET;
            turn2 = Constants.NOT_SET;
            for (var do1 = 0; do1 < 2; do1++)
            {
                var d1 = base1 + do1;
                var turn1Weight = Weight.Zero;
                if (includeTurn1)
                {
                    turn1 = DirectedHelper.BuildTurn(arrivalOffset1, do1);
                    turn1Weight = penalties[turn1];
                }
                for (var ao3 = 0; ao3 < 2; ao3++)
                {
                    var a3 = base3 + ao3;
                    var turn3Weight = Weight.Zero;
                    if (includeTurn3)
                    {
                        turn3 = DirectedHelper.BuildTurn(departureOffset3, ao3);
                        turn3Weight = penalties[turn3];
                    }
                    for (var t = 0; t < 4; t++)
                    {
                        int ao2, do2;
                        DirectedHelper.ExtractOffset(t, out ao2, out do2);

                        var weight = weights[d1][base2 + ao2] +
                            weights[base2 + do2][a3] +
                            penalties[t] + turn1Weight +
                            turn3Weight;

                        if (weight.GetForMetric(metric) < best.GetForMetric(metric))
                        {
                            best = weight;
                            departureOffset1 = do1;
                            arrivalOffset3 = ao3;
                            turn2 = t;
                        }
                    }
                }
            }
            return best - weightBefore;
        }

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
                    var fromIsFirst = tour.IsFirst(pair.From);
                    var toIsLast = tour.IsLast(pair.To);
                    var localCost = CheapestInsertionDirectedHelper.CalculateCheapestInsert(weights, turnPenalties, pair.From, customer, pair.To,
                        !fromIsFirst, !toIsLast, out departureOffset1, out arrivalOffset3, out turn2);
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
        
        /// <summary>
        /// Gets the weight for the given metric.
        /// </summary>
        public static float GetForMetric(this Weight weight, ProfileMetric metric)
        {
            switch (metric)
            {
                case ProfileMetric.TimeInSeconds:
                    return weight.Time;
                case ProfileMetric.DistanceInMeters:
                    return weight.Distance;
            }
            return weight.Value;
        }

        /// <summary>
        /// Inserts the given customer at the best location if possible.
        /// </summary>
        /// <param name="tour">The tour to insert into.</param>
        /// <param name="weights">The directed weights.</param>
        /// <param name="turnPenalties">The turn pentalties.</param>
        /// <param name="customer">The customer to insert.</param>
        /// <param name="metric">The metric to use, time, distance or custom.</param>
        /// <param name="max">The maximum allowed cost of the insertion.</param>
        public static Weight InsertCheapestDirected(this Tour tour, Weight[][] weights, Weight[] turnPenalties, ProfileMetric metric, int customer, Weight max)
        {
            if (tour.Count == 1)
            { // there is only one customer, the first one in the route.
                if (tour.First == tour.Last)
                { // there is one customer but it's both the start and the end.
                    var firstId = DirectedHelper.ExtractId(tour.First);

                    var bestCost = new Weight()
                    {
                        Distance = float.MaxValue,
                        Time = float.MaxValue,
                        Value = float.MaxValue
                    };
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

                            if (bestCost.GetForMetric(metric) > weight.GetForMetric(metric))
                            {
                                bestDirected1Id = firstDirectedId;
                                bestDirected2Id = customerDirectedId;
                                bestCost = weight;
                            }
                        }
                    }

                    if (bestCost.GetForMetric(metric) <= max.GetForMetric(metric))
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
                        firstId, customer, metric, out departureOffset1, out arrivalOffset2);
                    if (cost.GetForMetric(metric) <= max.GetForMetric(metric))
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
                var cost = Weight.MaxValue;
                var departureOffsetFrom = Constants.NOT_SET;
                var arrivalOffsetTo = Constants.NOT_SET;
                var turn = Constants.NOT_SET;
                var location = new Pair(int.MaxValue, int.MaxValue);

                foreach (var pair in tour.Pairs())
                {
                    int departureOffset1, arrivalOffset3, turn2;
                    var fromIsFirst = tour.IsFirst(pair.From);
                    var toIsLast = tour.IsLast(pair.To);
                    var localCost = CheapestInsertionDirectedHelper.CalculateCheapestInsert(weights, turnPenalties, metric, pair.From, customer, pair.To,
                        !fromIsFirst, !toIsLast, out departureOffset1, out arrivalOffset3, out turn2);
                    if (localCost.GetForMetric(metric) < cost.GetForMetric(metric))
                    {
                        cost = localCost;
                        location = pair;
                        departureOffsetFrom = departureOffset1;
                        arrivalOffsetTo = arrivalOffset3;
                        turn = turn2;
                    }
                }

                if (cost.GetForMetric(metric) <= max.GetForMetric(metric))
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
            return Weight.Zero; // insert failed, probably costs are too high (above given max parameter).
        }
    }
}