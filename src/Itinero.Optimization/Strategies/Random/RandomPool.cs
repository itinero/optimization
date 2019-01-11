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

using System.Collections;
using System.Collections.Generic;

namespace Itinero.Optimization.Strategies.Random
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
            _random = RandomGenerator.Default;

            this.Reset();
        }

        private int _left = -1;

        /// <summary>
        /// Gets the size of this pool.
        /// </summary>
        public int Size => _pool.Length;

        /// <summary>
        /// Returns the current element.
        /// </summary>
        public int Current { get; private set; } = -1;

        /// <summary>
        /// Returns the current element.
        /// </summary>
        object IEnumerator.Current => Current;

        /// <summary>
        /// Moves to the next element.
        /// </summary>
        /// <returns></returns>
        public int GetNext()
        {
            this.MoveNext(out var current);
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
                Current = -1;
                current = -1;
                return false;
            }

            if (_left == 1)
            {
                Current = _pool[0];
            }
            else
            {
                var i = _random.Generate(_left);
                Current = _pool[i];
                if (i < _left - 1)
                {
                    _pool[i] = _pool[_left - 1];
                }
            }
            _left--;
            current = Current;
            return true;
        }

        /// <summary>
        /// Moves to the next element.
        /// </summary>
        /// <returns></returns>
        public bool MoveNext()
        {
            return this.MoveNext(out _);
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