// Itinero.Logistics - Route optimization for .NET
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

using Itinero.Logistics.Routes;

namespace Itinero.Logistics.Solutions.Algorithms
{
    /// <summary>
    /// Contains extension methods to do cheapest insertion.
    /// </summary>
    public static class CheapestInsertion
    {
        /// <summary>
        /// Inserts the given customer at the best location.
        /// </summary>
        public static void InsertCheapest<T>(this IRoute route, IMatrixWeights<T> weights, int customer)
            where T : struct
        {
            Pair bestLocation;
            if (route.CalculateCheapest(weights, customer, out bestLocation) < float.MaxValue)
            {
                route.InsertAfter(bestLocation.From, customer);
            }
        }

        /// <summary>
        /// Calculates the best position to insert a given customer.
        /// </summary>
        public static float CalculateCheapest<T>(this IRoute route, IMatrixWeights<T> weights, int customer, out Pair location)
            where T : struct
        {
            var bestCost = float.MaxValue;
            location = new Pair(int.MaxValue, int.MaxValue);
            foreach (var pair in route.Pairs())
            {
                var cost = weights.WeightHandler.GetTime(weights.Weights[pair.From][customer]) +
                    weights.WeightHandler.GetTime(weights.Weights[customer][pair.To]) -
                    weights.WeightHandler.GetTime(weights.Weights[pair.From][pair.To]);
                if (cost < bestCost)
                {
                    bestCost = cost;
                    location = pair;
                }
            }
            return bestCost;
        }
    }
}