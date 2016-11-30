// Itinero.Optimization - Route optimization for .NET
// Copyright (C) 2016 Abelshausen Ben
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

using System.Collections;
using System.Collections.Generic;

namespace Itinero.Optimization.Algorithms.Random
{
    /// <summary>
    /// A pool to choose a random distinct sequence.
    /// </summary>
    /// <remarks>
    /// Based on: https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle#Modern_method
    /// </remarks>
    public sealed class RandomPool : IEnumerator<int>
    {
        private readonly RandomGenerator _random;
        private readonly int[] _pool;

        /// <summary>
        /// Creates a new pool.
        /// </summary>
        public RandomPool(int size)
        {
            _pool = new int[size];
            _random = RandomGeneratorExtensions.GetRandom();

            this.Reset();
        }

        private int _current = -1;
        private int _left = -1;

        /// <summary>
        /// Gets the size of this pool.
        /// </summary>
        public int Size
        {
            get
            {
                return _pool.Length;
            }
        }

        /// <summary>
        /// Returns the current element.
        /// </summary>
        public int Current
        {
            get
            {
                return _current;
            }
        }

        /// <summary>
        /// Returns the current element.
        /// </summary>
        object IEnumerator.Current
        {
            get
            {
                return _current;
            }
        }

        /// <summary>
        /// Moves to the next element.
        /// </summary>
        /// <returns></returns>
        public int GetNext()
        {
            int current;
            this.MoveNext(out current);
            return current;
        }

        /// <summary>
        /// Moves to the next element.
        /// </summary>
        /// <returns></returns>
        public bool MoveNext(out int current)
        {
            if (_left == 0)
            {
                _current = -1;
                current = -1;
                return false;
            }

            if (_left == 1)
            {
                _current = _pool[0];
            }
            else
            {
                var i = _random.Generate(_left);
                _current = _pool[i];
                if (i < _left - 1)
                {
                    _pool[i] = _pool[_left - 1];
                }
            }
            _left--;
            current = _current;
            return true;
        }

        /// <summary>
        /// Moves to the next element.
        /// </summary>
        /// <returns></returns>
        public bool MoveNext()
        {
            int current;
            return this.MoveNext(out current);
        }

        /// <summary>
        /// Resets the pool.
        /// </summary>
        public void Reset()
        {
            for(var i = 0; i < _pool.Length; i++)
            {
                _pool[i] = i;
            }
            _left = _pool.Length;
        }

        /// <summary>
        /// Disposes of all native resources associated with this enumerator.
        /// </summary>
        public void Dispose()
        {

        }
    }
}