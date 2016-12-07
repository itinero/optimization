// Itinero.Optimization - Route optimization for .NET
// Copyright (C) 2015 Abelshausen Ben
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
using System.Linq;

namespace Itinero.Optimization.Tours.Cycles
{
    /// <summary>
    /// A collection of asymmetric alternating cycles.
    /// </summary>
    public class AsymmetricAlternatingCycles
    {
        private KeyValuePair<int, int>[] _nextArray;
        private Dictionary<int, int> _cycles;

        /// <summary>
        /// Create a new collection of asymmetric alternating cycles.
        /// </summary>
        /// <param name="length"></param>
        public AsymmetricAlternatingCycles(int length)
        {
            _nextArray = new KeyValuePair<int, int>[length];
            _cycles = null;

            for (int idx = 0; idx < length; idx++)
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
            HashSet<int> to_do = new HashSet<int>();
            foreach (KeyValuePair<int, int> pair in _nextArray)
            {
                if (pair.Key >= 0)
                {
                    to_do.Add(pair.Key);
                }
            }
            while (to_do.Count > 0)
            {
                this.CheckForCycle(to_do, to_do.First());
            }
        }

        private void CheckForCycle(HashSet<int> to_do, int customer)
        {
            int start = customer;
            int count = 1;
            to_do.Remove(customer);
            while (_nextArray[customer].Value >= 0)
            {

                if (_nextArray[customer].Value == start)
                {
                    _cycles.Add(start, count);
                    break;
                }

                count++;
                customer = _nextArray[customer].Value;
                to_do.Remove(customer);
            }
        }

        internal KeyValuePair<int, int> Next(int start)
        {
            return _nextArray[start];
        }
    }
}