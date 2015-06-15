// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

using OsmSharp.Collections;
using System.Collections.Generic;

namespace OsmSharp.Logistics.Solutions.Algorithms
{
    /// <summary>
    /// Contains n-nearest neighbour algorithm.
    /// </summary>
    public static class NNearestNeighboursAlgorithm
    {
        /// <summary>
        /// Calculates then n-nearest neighbours in a forward-direction only.
        /// </summary>
        /// <returns></returns>
        public static INNearestNeighbours Forward(double[][] weights, int n, int customer)
        {
            var neighbours = new SortedDictionary<double, List<int>>();
            for (var current = 0; current < weights.Length; current++)
            {
                if (current != customer)
                {
                    var weight = weights[customer][current];
                    List<int> customers = null;
                    if (!neighbours.TryGetValue(weight, out customers))
                    {
                        customers = new List<int>();
                        neighbours.Add(weight, customers);
                    }
                    customers.Add(current);
                }
            }

            var result = new NNearestNeighbours(n);
            foreach (var pair in neighbours)
            {
                foreach (var current in pair.Value)
                {
                    if (result.Count < n)
                    {
                        if (result.Max < pair.Key)
                        {
                            result.Max = pair.Key;
                        }
                        result.Add(current);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return result;
        }
    }
}