// Itinero - OpenStreetMap (OSM) SDK
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

namespace Itinero.Logistics.Routes.Cycles
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