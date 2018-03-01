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

using Itinero.Optimization.Algorithms.Directed;
using System.Collections.Generic;
using System;

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
            return Forward((x, y) => weights[x][y], weights.Length, max, customer);
        }

        /// <summary>
        /// Calculates then nearest neighbours using a weight smaller than or equal to a maximum in a forward-direction only.
        /// </summary>
        /// <returns></returns>
        public static SortedNearestNeighbours Forward(Func<int, int, float> weightFunc, int count, float max, int customer)
        {
            var neighbours = new Collections.SortedDictionary<float, List<int>>();
            var maxFound = 0f;
            for (var current = 0; current < count; current++)
            {
                if (current != customer)
                {
                    var weight = weightFunc(customer, current);
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
            return Backward((x, y) => weights[x][y], weights.Length, max, customer);
        }

        /// <summary>
        /// Calculates then nearest neighbours using a weight smaller than or equal to a maximum in a backward-direction only.
        /// </summary>
        /// <returns></returns>
        public static SortedNearestNeighbours Backward(Func<int, int, float> weightFunc, int count, float max, int customer)
        {
            var neighbours = new Collections.SortedDictionary<float, List<int>>();
            var maxFound = 0f;
            for (var current = 0; current < count; current++)
            {
                if (current != customer)
                {
                    var weight = weightFunc(current, customer);
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