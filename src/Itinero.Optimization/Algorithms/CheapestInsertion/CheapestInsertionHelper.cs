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

using Itinero.Optimization.Tours;

namespace Itinero.Optimization.Algorithms.CheapestInsertion
{
    /// <summary>
    /// Contains extension methods to do cheapest insertion.
    /// </summary>
    public static class CheapestInsertionHelper
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
    }
}
