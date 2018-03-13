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

namespace Itinero.Optimization.Abstract.Tours.Cycles
{
    /// <summary>
    /// Asymmetric cycles.
    /// </summary>
    public class AsymmetricCycles
    {
        private int[] _nextArray;
        private Dictionary<int, int> _cycles;

        /// <summary>
        /// Creates a new collection of asymmetric cylces.
        /// </summary>
        /// <param name="length"></param>
        public AsymmetricCycles(int length)
        {
            _nextArray = new int[length];
            _cycles = null;

            for (int idx = 0; idx < length; idx++)
            {
                _nextArray[idx] = -1;
            }
        }

        /// <summary>
        /// Creates a new collection of asymmetric cycles.
        /// </summary>
        /// <param name="nextArray"></param>
        /// <param name="cycles"></param>
        private AsymmetricCycles(int[] nextArray,
            Dictionary<int, int> cycles)
        {
            _nextArray = nextArray;
            _cycles = cycles;
        }

        /// <summary>
        /// Returns the nextarray.
        /// </summary>
        public int[] NextArray
        {
            get
            {
                return _nextArray;
            }
        }

        /// <summary>
        /// Returns the next customer.
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public int this[int from]
        {
            get
            {
                return _nextArray[from];
            }
        }

        /// <summary>
        /// Adds a new edge.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public void AddEdge(int from, int to)
        {
            _cycles = null;
            // set the next to.
            _nextArray[from] = to;
        }

        /// <summary>
        /// Returns cycles.
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
            var done = new bool[_nextArray.Length];
            for (int idx = 0; idx < _nextArray.Length; idx++)
            {
                if (!done[idx])
                {
                    this.CheckForCycle(done, idx);
                }
            }
        }

        private void CheckForCycle(bool[] done, int customer)
        {
            int start = customer;
            int count = 1;
            while (_nextArray[customer] >= 0)
            {
                done[customer] = true;

                if (_nextArray[customer] == start)
                {
                    _cycles.Add(start, count);
                    break;
                }

                count++;
                customer = _nextArray[customer];

                if (count > _nextArray.Length)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Clones this asymmetric cycles.
        /// </summary>
        /// <returns></returns>
        public AsymmetricCycles Clone()
        {
            return new AsymmetricCycles(_nextArray.Clone() as int[], null);
        }

        /// <summary>
        /// Returns the length.
        /// </summary>
        public int Length
        {
            get
            {
                return _nextArray.Length;
            }
        }
    }
}