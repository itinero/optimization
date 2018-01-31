/*
 *  Licensed to SharpSoftware under one or more contributor
 *  license agreements. See the NOTICE file distributed with this work for 
 *  additional information regarding copyright ownership.
 * 
 *  SharpSoftware licenses this file to you under the Apache License, 
 *  Version 2.0 (the "License"); you may not use this file except in 
 *  compliance with the License. You may obtain a copy of the License at
 * 
 *       http://www.apache.org/licenses/LICENSE-2.0
 * 
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */

using System;
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
            return Forward((x, y) => weights[x][y], weights.Length, n, customer);
        }

        /// <summary>
        /// Calculates then n-nearest neighbours in a forward-direction only.
        /// </summary>
        /// <returns></returns>
        public static NearestNeighbours Forward(Func<int, int, float> weightFunc, int count, int n, int customer)
        {
            var neighbours = new Collections.SortedDictionary<float, List<int>>();
            for (var current = 0; current < count; current++)
            {
                if (current != customer)
                {
                    var weight = weightFunc(customer, current);
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
            return Backward((x, y) => weights[x][y], weights.Length, n, customer);
        }

        /// <summary>
        /// Calculates then n-nearest neighbours in a backward-direction only.
        /// </summary>
        /// <returns></returns>
        public static NearestNeighbours Backward(Func<int, int, float> weightFunc, int count, int n, int customer)
        {
            var neighbours = new Collections.SortedDictionary<float, List<int>>();
            for (var current = 0; current < count; current++)
            {
                if (current != customer)
                {
                    var weight = weightFunc(current, customer);
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