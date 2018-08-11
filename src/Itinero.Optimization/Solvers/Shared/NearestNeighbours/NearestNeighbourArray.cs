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
using System.Collections;
using System.Collections.Generic;

namespace Itinero.Optimization.Solvers.Shared.NearestNeighbours
{
    /// <summary>
    /// A nearest neighbour array, keeps 'n' nearest neighbours for each entry in the given weight matrix.
    /// </summary>
    public class NearestNeighbourArray : IEnumerable<int>
    {
        private readonly int[] _nn; // Contains the nearest neigbour per visit.

        /// <summary>
        /// Creates a new nearest neigbour array using bidirectional weights for all visits.
        /// </summary>
        /// <param name="weights">The weights to use.</param>
        public NearestNeighbourArray(float[][] weights) 
            : this(weights, weights.Length - 1)
        {

        }

        /// <summary>
        /// Creates a new nearest neigbour array using bidirectional weights keeping <paramref name="n"/> per visit.
        /// </summary>
        /// <param name="weights">The weights to use.</param>
        /// <param name="n">The number of nearest neigbours to keep.</param>
        public NearestNeighbourArray(float[][] weights, int n) 
            : this((v1, v2) => weights[v1][v2] + weights[v2][v1], weights.Length, n)
        {

        }

        /// <summary>
        /// Creates a new nearest neigbour array.
        /// </summary>
        /// <param name="weightFunc">The weight function.</param>
        /// <param name="count">The # of visits.</param>
        /// <param name="n">The number of neigbours to keep per visit.</param>
        public NearestNeighbourArray(Func<int, int, float> weightFunc, int count, int n)
        {
            N = n;
            _nn = new int[n * count];

            for (var v = 0; v < count; v++)
            {
                var neighbours = new SortedDictionary<float, List<int>>();
                for (var current = 0; current < count; current++)
                {
                    if (current == v) continue;
                    var weight = weightFunc(v, current);
                    if (!neighbours.TryGetValue(weight, out var visits))
                    {
                        visits = new List<int>();
                        neighbours.Add(weight, visits);
                    }
                    visits.Add(current);
                }

                var neigbourCount = 0;
                foreach (var pair in neighbours)
                {
                    foreach (var current in pair.Value)
                    {
                        if (neigbourCount >= n)
                        {
                            break;
                        }

                        _nn[v * n + neigbourCount] = current;
                        neigbourCount++;
                    }
                }
            }
        }
        
        /// <summary>
        /// Gets the # of nearest neigbours per visit.
        /// </summary>
        /// <returns></returns>
        public int N { get; }

        /// <summary>
        /// Gets the nearest neigbours for the given visit.
        /// </summary>
        /// <param name="v">The visit.</param>
        public int[] this[int v]
        {
            get
            {
                var nn = new int[N];
                _nn.CopyTo(nn, 0, v * N, N);
                return nn;
            }
        }

        /// <summary>
        /// Copies the nearest neigbours of the given visit to the given array.
        /// </summary>
        /// <param name="v">The visit.</param>
        /// <param name="nn">The array to copy to.</param>
        /// <param name="index">The index to start copying at.</param>
        public void CopyTo(int v, int[] nn, int index = 0)
        {
            _nn.CopyTo(nn, index, v * N, N);
        }

        /// <summary>
        /// Copies the nearest neigbours of the given visit to the given array.
        /// </summary>
        /// <param name="v">The visit.</param>
        /// <param name="nn">The array to copy to.</param>
        /// <param name="index">The index to start copying at.</param>
        /// <param name="count">The # of nn to copy.</param>
        public void CopyTo(int v, int[] nn, int index, int count)
        {
            _nn.CopyTo(nn, index, v * N, count);
        }

        public IEnumerator<int> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}