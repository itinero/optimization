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
using System.Collections.Generic;

namespace Itinero.Optimization.Algorithms.NearestNeighbour
{
    /// <summary>
    /// An enumerable containing n-nearest neighbours and some extra information like maximum weight and n.
    /// </summary>
    public class SortedNearestNeighbours : List<int>
    {
        /// <summary>
        /// Creates a new sorted nearest neighbour collection.
        /// </summary>
        public SortedNearestNeighbours(float max)
        {
            this.Max = max;
        }

        /// <summary>
        /// Gets the customer at the given index.
        /// </summary>
        /// <returns></returns>
        public int Get(int idx)
        {
            return this[idx];
        }

        /// <summary>
        /// Gets the # of nearest neighbours.
        /// </summary>
        public int N
        {
            get { return this.Count; }
        }

        /// <summary>
        /// Gets the maximum weight.
        /// </summary>
        public float Max
        {
            get;
            private set;
        }

        /// <summary>
        /// Calculates then nearest neighbours using a weight smaller than or equal to a maximum in a forward-direction only.
        /// </summary>
        /// <returns></returns>
        public static SortedNearestNeighbours Forward(float[][] weights, float max, int customer)
        {
            var neighbours = new Collections.SortedDictionary<float, List<int>>();
            var maxFound = 0f;
            for (var current = 0; current < weights.Length; current++)
            {
                if (current != customer)
                {
                    var weight = weights[customer][current];
                    if (weight <= max)
                    {
                        List<int> customers = null;
                        if (!neighbours.TryGetValue(weight, out customers))
                        {
                            customers = new List<int>();
                            neighbours.Add(weight, customers);
                        }
                        customers.Add(current);

                        if (maxFound < weight)
                        {
                            maxFound = weight;
                        }
                    }
                }
            }

            var result = new SortedNearestNeighbours(maxFound);
            foreach (var pair in neighbours)
            {
                foreach (var current in pair.Value)
                {
                    result.Add(current);
                }
            }
            return result;
        }

        /// <summary>
        /// Calculates then nearest neighbours using a weight smaller than or equal to a maximum in a forward-direction only.
        /// </summary>
        /// <returns></returns>
        public static SortedNearestNeighbours ForwardDirected(float[][] weights, float max, int customer)
        {
            var neighbours = new Collections.SortedDictionary<float, List<int>>();
            var maxFound = 0f;
            for (var current = 0; current < weights.Length / 2; current++)
            {
                if (current != customer)
                {
                    var weight = weights.MinWeight(customer, current);
                    if (weight <= max)
                    {
                        List<int> customers = null;
                        if (!neighbours.TryGetValue(weight, out customers))
                        {
                            customers = new List<int>();
                            neighbours.Add(weight, customers);
                        }
                        customers.Add(current);

                        if (maxFound < weight)
                        {
                            maxFound = weight;
                        }
                    }
                }
            }

            var result = new SortedNearestNeighbours(maxFound);
            foreach (var pair in neighbours)
            {
                foreach (var current in pair.Value)
                {
                    result.Add(current);
                }
            }
            return result;
        }

        /// <summary>
        /// Calculates then nearest neighbours using a weight smaller than or equal to a maximum in a backward-direction only.
        /// </summary>
        /// <returns></returns>
        public static SortedNearestNeighbours Backward(float[][] weights, float max, int customer)
        {
            var neighbours = new Collections.SortedDictionary<float, List<int>>();
            var maxFound = 0f;
            for (var current = 0; current < weights.Length; current++)
            {
                if (current != customer)
                {
                    var weight = weights[current][customer];
                    if (weight <= max)
                    {
                        List<int> customers = null;
                        if (!neighbours.TryGetValue(weight, out customers))
                        {
                            customers = new List<int>();
                            neighbours.Add(weight, customers);
                        }
                        customers.Add(current);

                        if (maxFound <= weight)
                        {
                            maxFound = weight;
                        }
                    }
                }
            }

            var result = new SortedNearestNeighbours(maxFound);
            foreach (var pair in neighbours)
            {
                foreach (var current in pair.Value)
                {
                    result.Add(current);
                }
            }
            return result;
        }

        /// <summary>
        /// Calculates then nearest neighbours using a weight smaller than or equal to a maximum in a backward-direction only.
        /// </summary>
        /// <returns></returns>
        public static SortedNearestNeighbours BackwardDirected(float[][] weights, float max, int customer)
        {
            var neighbours = new Collections.SortedDictionary<float, List<int>>();
            var maxFound = 0f;
            for (var current = 0; current < weights.Length / 2; current++)
            {
                if (current != customer)
                {
                    var weight = weights.MinWeight(current, customer);
                    if (weight <= max)
                    {
                        List<int> customers = null;
                        if (!neighbours.TryGetValue(weight, out customers))
                        {
                            customers = new List<int>();
                            neighbours.Add(weight, customers);
                        }
                        customers.Add(current);

                        if (maxFound <= weight)
                        {
                            maxFound = weight;
                        }
                    }
                }
            }

            var result = new SortedNearestNeighbours(maxFound);
            foreach (var pair in neighbours)
            {
                foreach (var current in pair.Value)
                {
                    result.Add(current);
                }
            }
            return result;
        }
    }
}