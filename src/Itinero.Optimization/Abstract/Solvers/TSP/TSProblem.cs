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

using Itinero.Optimization.Algorithms.NearestNeighbour;
using Itinero.Optimization.Tours;
using System.Collections.Generic;

namespace Itinero.Optimization.Abstract.Solvers.TSP
{
    /// <summary>
    /// The default TSP profile definition.
    /// </summary>
    public sealed class TSProblem : ITSProblem
    {
        private readonly float[][] _weights;

        /// <summary>
        /// Creates a new TSP 'open' TSP with only a start customer.
        /// </summary>
        public TSProblem(int first, float[][] weights)
        {
            this.First = first;
            this.Last = null;
            _weights = weights;
            this.Count = weights.Length;
            
            for (var x = 0; x < _weights.Length; x++)
            {
                _weights[x][first] = 0;
            }
        }

        /// <summary>
        /// Creates a new TSP, 'closed' when first equals last.
        /// </summary>
        public TSProblem(int first, int last, float[][] weights)
        {
            this.First = first;
            this.Last = last;
            _weights = weights;
            this.Count = weights.Length;

            _weights[first][last] = 0;
        }
        
        /// <summary>
        /// Gets the weight between the two given visits.
        /// </summary>
        public float Weight(int from, int to)
        {
            return _weights[from][to];
        }

        /// <summary>
        /// Gets the first customer.
        /// </summary>
        public int First { get; set; }

        /// <summary>
        /// Gets the last customer if the problem is closed.
        /// </summary>
        public int? Last { get; set; }

        /// <summary>
        /// Gets the number of visits.
        /// </summary>
        public int Count { get; private set; }

        private NearestNeighbourCache _nnCache = null;

        /// <summary>
        /// Gets the nearest neighbour cache.
        /// </summary>
        /// <returns></returns>
        public NearestNeighbourCache NearestNeighbourCache
        {
            get
            {
                if (_nnCache == null)
                {
                    _nnCache = new NearestNeighbourCache(_weights.Length, 
                        (x, y) => _weights[x][y]);
                }
                return _nnCache;
            }
        }
    }
}