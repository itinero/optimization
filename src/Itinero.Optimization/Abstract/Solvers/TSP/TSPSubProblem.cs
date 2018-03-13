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
using Itinero.Optimization.Abstract.Tours;
using System.Collections.Generic;

namespace Itinero.Optimization.Abstract.Solvers.TSP
{
    /// <summary>
    /// A TSP problem that encapsulates a larger weight matrix.
    /// </summary>
    public class TSPSubProblem : ITSProblem
    {
        private readonly float[][] _weights;
        private readonly int[] _forwardMap;
        private readonly Dictionary<int, int> _backwardMap;

        /// <summary>
        /// Creates a new TSP.
        /// </summary>
        /// <param name="tour">A tour relative to the weight matrix.</param>
        /// <param name="weights">The larger weight matrix.</param>
        public TSPSubProblem(ITour tour, float[][] weights)
        {
            _backwardMap = new Dictionary<int, int>();
            _weights = weights;

            // build backward map.
            foreach (var visit in tour)
            {
                if (!_backwardMap.ContainsKey(visit))
                {
                    _backwardMap[visit] = _backwardMap.Count;
                }
            }

            // build forward map.
            _forwardMap = new int[_backwardMap.Count];
            foreach (var backwardMapping in _backwardMap)
            {
                _forwardMap[backwardMapping.Value] = backwardMapping.Key;
            }
        }
        
        /// <summary>
        /// Gets the weight between the two given visits.
        /// </summary>
        public float Weight(int from, int to)
        {
            return _weights[_forwardMap[from]][_forwardMap[to]];
        }

        /// <summary>
        /// Gets the first customer.
        /// </summary>
        public int First { get; private set; }

        /// <summary>
        /// Gets the last customer if the problem is closed.
        /// </summary>
        public int? Last { get; private set; }

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
                    _nnCache = new NearestNeighbourCache(_forwardMap.Length, 
                        this.Weight);
                }
                return _nnCache;
            }
        }
    }
}