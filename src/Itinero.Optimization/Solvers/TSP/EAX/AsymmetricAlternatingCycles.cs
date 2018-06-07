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

using System.Collections.Generic;
using System.Linq;

namespace Itinero.Optimization.Solvers.TSP.EAX
{
    /// <summary>
    /// A collection of asymmetric alternating cycles.
    /// </summary>
    internal class AsymmetricAlternatingCycles
    {
        private readonly KeyValuePair<int, int>[] _nextArray;
        private Dictionary<int, int> _cycles;

        /// <summary>
        /// Create a new collection of asymmetric alternating cycles.
        /// </summary>
        /// <param name="length"></param>
        public AsymmetricAlternatingCycles(int length)
        {
            _nextArray = new KeyValuePair<int, int>[length];
            _cycles = null;

            for (var idx = 0; idx < length; idx++)
            {
                _nextArray[idx] = new KeyValuePair<int, int>(-1, -1);
            }
        }

        /// <summary>
        /// Adds an edge.
        /// </summary>
        /// <param name="fromA"></param>
        /// <param name="to"></param>
        /// <param name="fromB"></param>
        public void AddEdge(int fromA, int to, int fromB)
        {
            _cycles = null;
            _nextArray[fromA] = new KeyValuePair<int, int>(to, fromB);
        }

        /// <summary>
        /// Returns the cycles.
        /// </summary>
        public Dictionary<int, int> Cycles
        {
            get
            {
                if (_cycles == null)
                {
                    this.CalculateCycles();
                }
                return _cycles;
            }
        }

        private void CalculateCycles()
        {
            _cycles = new Dictionary<int, int>();
            var toDo = new HashSet<int>();
            foreach (var pair in _nextArray)
            {
                if (pair.Key >= 0)
                {
                    toDo.Add(pair.Key);
                }
            }
            while (toDo.Count > 0)
            {
                this.CheckForCycle(toDo, toDo.First());
            }
        }

        private void CheckForCycle(HashSet<int> toDo, int customer)
        {
            var start = customer;
            var count = 1;
            toDo.Remove(customer);
            while (_nextArray[customer].Value >= 0)
            {

                if (_nextArray[customer].Value == start)
                {
                    _cycles.Add(start, count);
                    break;
                }

                count++;
                customer = _nextArray[customer].Value;
                toDo.Remove(customer);
            }
        }

        internal KeyValuePair<int, int> Next(int start)
        {
            return _nextArray[start];
        }
    }
}