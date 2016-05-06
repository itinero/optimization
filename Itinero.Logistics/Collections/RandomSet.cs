// Itinero.Logistics - Route optimization for .NET
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

using Itinero.Logistics.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace Itinero.Logistics.Collections
{
    /// <summary>
    /// A set that allows random selection in constant time.
    /// </summary>
    /// <remarks>Focus is on speed, data that was added will never be removed unless clear is used.</remarks>
    public class RandomSet<T> : IEnumerable<T>
    {
        private readonly Dictionary<T, int> _index;

        /// <summary>
        /// Creates a new random set.
        /// </summary>
        public RandomSet()
            : this(Enumerable.Empty<T>())
        {

        }

        /// <summary>
        /// Creates a new random set.
        /// </summary>
        public RandomSet(IEnumerable<T> data)
        {
            _data = new List<T>();
            _index = new Dictionary<T, int>();

            foreach(var d in data)
            {
                this.Add(d);
            }
        }

        private List<T> _data;

        /// <summary>
        /// Adds a new element.
        /// </summary>
        public void Add(T value)
        {
            if (!_index.ContainsKey(value))
            {
                _index[value] = _data.Count;
                _data.Add(value);
            }
        }

        /// <summary>
        /// Removes a value from this set.
        /// </summary>
        public void Remove(T value)
        {
            int idx;
            if (_index.TryGetValue(value, out idx))
            {
                _index.Remove(value);
            }
        }

        /// <summary>
        /// Gets a random element.
        /// </summary>
        public T GetRandom()
        {
            if (this.Count == 0)
            {
                throw new InvalidOperationException("Cannot select a random element from an empty set.");
            }
            var e = _data[RandomGeneratorExtensions.GetRandom().Generate(_data.Count)];
            return e;
        }

        /// <summary>
        /// Gets and removes a random element.
        /// </summary>
        public T RemoveRandom()
        {
            if (this.Count == 0)
            {
                throw new InvalidOperationException("Cannot select a random element from an empty set.");
            }
            var e = _data[RandomGeneratorExtensions.GetRandom().Generate(_data.Count)];
            while (!_index.ContainsKey(e))
            {
                e = _data[RandomGeneratorExtensions.GetRandom().Generate(_data.Count)];
            }
            _index.Remove(e);

            if (_data.Count <= System.Math.Ceiling(_index.Count / 2))
            { // if >= 1/2 chances of selecting a remove item rebuild data array.
                _data = new List<T>(_index.Keys);
            }

            return e;
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        public IEnumerator<T> GetEnumerator()
        {
            return _index.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _index.Keys.GetEnumerator();
        }

        /// <summary>
        /// Gets the number of elements.
        /// </summary>
        public int Count
        {
            get
            {
                return _index.Count;
            }
        }
    }
}
