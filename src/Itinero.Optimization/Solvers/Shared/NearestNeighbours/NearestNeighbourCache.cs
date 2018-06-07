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

namespace Itinero.Optimization.Solvers.Shared.NearestNeighbours
{
    /// <summary>
    /// A nearest neighbour cache.
    /// </summary>
    internal sealed class NearestNeighbourCache
    {
        private readonly int _count;
        private readonly Func<int, int, float> _weightFunc;

        /// <summary>
        /// Creates a new cache.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="weightFunc"></param>
        public NearestNeighbourCache(int count, Func<int, int, float> weightFunc)
        {
            _count = count;
            _weightFunc = weightFunc;
        }
        
        private Dictionary<int, NearestNeighbourArray> _nearestNeighbours; // Holds the nearest neighbours.
        private Dictionary<int, NearestNeighbourArray> _forwardNearestNeighbours; // Holds the nearest neighbours in forward direction.
        private Dictionary<int, NearestNeighbourArray> _backwardNearestNeighbours; // Holds the nearest neighbours in backward direction.
        
        /// <summary>
        /// Gets the nearest neighbours for the given 'n'.
        /// </summary>
        /// <returns>The nearest neighbour array.</returns>
        public NearestNeighbourArray GetNNearestNeighbours(int n, int customer)
        {
            if (_nearestNeighbours == null)
            { // not there yet, create.
                _nearestNeighbours = new Dictionary<int, NearestNeighbourArray>();
            }

            if (_nearestNeighbours.TryGetValue(n, out var nearestNeighbours)) return nearestNeighbours; 
            
            // not found for n, create.
            nearestNeighbours = new NearestNeighbourArray((x, y) => _weightFunc(y, x), _count, n);
            _nearestNeighbours.Add(n, nearestNeighbours);
            return nearestNeighbours;
        }

        /// <summary>
        /// Gets the nearest neighbours for the given 'n' in forward direction.
        /// </summary>
        /// <returns>The nearest neighbour array.</returns>
        public NearestNeighbourArray GetNNearestNeighboursForward(int n)
        {
            if (_forwardNearestNeighbours == null)
            { // not there yet, create.
                _forwardNearestNeighbours = new Dictionary<int, NearestNeighbourArray>();
            }

            if (_forwardNearestNeighbours.TryGetValue(n, out var nearestNeighbours)) return nearestNeighbours; 
            
            // not found for n, create.
            nearestNeighbours = new NearestNeighbourArray((x, y) => _weightFunc(x, y), _count, n);
            _forwardNearestNeighbours.Add(n, nearestNeighbours);
            return nearestNeighbours;
        }
        
        /// <summary>
        /// Gets the nearest neighbours for the given 'n' in backward direction.
        /// </summary>
        /// <returns>The nearest neighbour array.</returns>
        public NearestNeighbourArray GetNNearestNeighboursBackward(int n, int customer)
        {
            if (_backwardNearestNeighbours == null)
            { // not there yet, create.
                _backwardNearestNeighbours = new Dictionary<int, NearestNeighbourArray>();
            }

            if (_backwardNearestNeighbours.TryGetValue(n, out var nearestNeighbours)) return nearestNeighbours; 
            
            // not found for n, create.
            nearestNeighbours = new NearestNeighbourArray((x, y) => _weightFunc(y, x), _count, n);
            _backwardNearestNeighbours.Add(n, nearestNeighbours);
            return nearestNeighbours;
        }
    }
}