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

using System.Collections.Generic;
using Itinero.Optimization.Algorithms.Directed;

namespace Itinero.Optimization.Algorithms.NearestNeighbour
{
    /// <summary>
    /// An enumerable containing n-nearest neighbours and some extra information like maximum weight and n.
    /// </summary>
    public class NearestNeighbours : HashSet<int>
    {
        /// <summary>
        /// Creates a new nearest neighbours enumerable.
        /// </summary>
        public NearestNeighbours(int n)
        {
            this.N = n;
        }

        /// <summary>
        /// Gets the requested N.
        /// </summary>
        /// <remarks>It's possible this contains less than N if problem size is smaller than N for example.</remarks>
        public int N
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the maximum weight of the furthest customer.
        /// </summary>
        public float Max
        {
            get;
            set;
        }


        /// <summary>
        /// Calculates then n-nearest neighbours in a forward-direction only.
        /// </summary>
        /// <returns></returns>
        public static NearestNeighbours Forward(float[][] weights, int n, int customer)
        {
            var neighbours = new Collections.SortedDictionary<float, List<int>>();
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

            var result = new NearestNeighbours(n);
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
        
        /// <summary>
        /// Calculates then n-nearest neighbours in a forward-direction only.
        /// </summary>
        /// <returns></returns>
        public static NearestNeighbours ForwardDirected(float[][] weights, int n, int customer)
        {
            var neighbours = new Collections.SortedDictionary<float, List<int>>();
            for (var current = 0; current < weights.Length / 2; current++)
            {
                if (current != customer)
                {
                    var weight = weights.MinWeight(customer, current);
                    List<int> customers = null;
                    if (!neighbours.TryGetValue(weight, out customers))
                    {
                        customers = new List<int>();
                        neighbours.Add(weight, customers);
                    }
                    customers.Add(current);
                }
            }

            var result = new NearestNeighbours(n);
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


        /// <summary>
        /// Calculates then n-nearest neighbours in a backward-direction only.
        /// </summary>
        /// <returns></returns>
        public static NearestNeighbours Backward(float[][] weights, int n, int customer)
        {
            var neighbours = new Collections.SortedDictionary<float, List<int>>();
            for (var current = 0; current < weights.Length; current++)
            {
                if (current != customer)
                {
                    var weight = weights[current][customer];
                    List<int> customers = null;
                    if (!neighbours.TryGetValue(weight, out customers))
                    {
                        customers = new List<int>();
                        neighbours.Add(weight, customers);
                    }
                    customers.Add(current);
                }
            }

            var result = new NearestNeighbours(n);
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
        
        /// <summary>
        /// Calculates then n-nearest neighbours in a backward-direction only.
        /// </summary>
        /// <returns></returns>
        public static NearestNeighbours BackwardDirected(float[][] weights, int n, int customer)
        {
            var neighbours = new Collections.SortedDictionary<float, List<int>>();
            for (var current = 0; current < weights.Length / 2; current++)
            {
                if (current != customer)
                {
                    var weight = weights.MinWeight(current, customer);
                    List<int> customers = null;
                    if (!neighbours.TryGetValue(weight, out customers))
                    {
                        customers = new List<int>();
                        neighbours.Add(weight, customers);
                    }
                    customers.Add(current);
                }
            }

            var result = new NearestNeighbours(n);
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