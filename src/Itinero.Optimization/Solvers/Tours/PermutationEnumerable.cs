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

namespace Itinero.Optimization.Solvers.Tours
{

    /// <summary>
    /// An enumerable of all possible permutations of a given sequence of objects.
    /// </summary>
    /// <remarks>
    /// Implements the Shimon Even variant of the Steinhaus–Johnson–Trotter algorithm.
    /// 
    /// https://en.wikipedia.org/wiki/Steinhaus%E2%80%93Johnson%E2%80%93Trotter_algorithm#Even.27s_speedup
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public class PermutationEnumerable<T> : IEnumerable<T[]>
    {
        private readonly IReadOnlyList<T> _sequence;

        /// <summary>
        /// Creates a new permutation enumerator over a given sequence.
        /// </summary>
        /// <param name="sequence"></param>
        public PermutationEnumerable(IReadOnlyList<T> sequence)
        {
            _sequence = sequence;
        }

        /// <summary>
        /// Returns the enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T[]> GetEnumerator()
        {
            return new PermutationEnumerator<T>(_sequence);
        }

        /// <summary>
        /// Returns the enumerator.
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new PermutationEnumerator<T>(_sequence);
        }
    }

    /// <summary>
    /// Implements the Shimon Even variant of the Steinhaus–Johnson–Trotter algorithm.
    /// 
    /// https://en.wikipedia.org/wiki/Steinhaus%E2%80%93Johnson%E2%80%93Trotter_algorithm#Even.27s_speedup
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class PermutationEnumerator<T> : IEnumerator<T[]>
    {
        private T[] _sequence;
        private ElementStatus[] _status;

        internal PermutationEnumerator(IReadOnlyList<T> sequence)
        {
            _sequence = new T[sequence.Count];
            for (var i = 0; i < _sequence.Length; i++)
            {
                _sequence[i] = sequence[i];
            }
            _status = null;
        }

        public T[] Current => _sequence.Clone() as T[];

        public void Dispose()
        {

        }

        object System.Collections.IEnumerator.Current => this.Current;

        public bool MoveNext()
        {
            // intialize if needed.
            if (_status == null)
            {
                // reset the status.
                _status = new ElementStatus[_sequence.Length];
                _status[0] = new ElementStatus(1, null);
                for (uint idx = 1; idx < _sequence.Length; idx++)
                {
                    _status[idx] = new ElementStatus(idx + 1, false);
                }
                return true;
            }

            var maxIdx = 0;
            var max = new ElementStatus(uint.MinValue, null);
            for (var idx = 0; idx < _status.Length; idx++)
            {
                if (max.Value >= _status[idx].Value || !_status[idx].Direction.HasValue) continue; // the new value is bigger!
                maxIdx = idx;
                max = _status[idx];
            }

            // the algorithm is terminated if nothing is found.
            if (!max.Direction.HasValue)
            { // all directions are unmarked.
                return false;
            }

            // execute the swap.
            var i1 = maxIdx;
            var i2 = max.IsForward ? maxIdx + 1 : maxIdx - 1;
            this.Swap(i1, i2);

            // update status.
            if (i2 > i1 && i2 < _status.Length - 1)
            { // there is a next element, test if it is larger.
                if (_status[i2 + 1].Value > max.Value)
                { // reset the direction of max.
                    max.Direction = null;
                }
            }
            else if (i2 < i1 && i2 > 0)
            { // there is a next element, test if it is larger.
                if (_status[i2 - 1].Value > max.Value)
                { // reset the direction of max.
                    max.Direction = null;
                }
            }
            if (i2 == 0 || i2 == _status.Length - 1)
            { // reset the direction of that element
                _status[i2].Direction = null;
            }
            for (var i = 0; i < _status.Length; i++)
            { // redirect all of the element higher than the selected element.
                if (_status[i].Value > max.Value)
                { // reset the direction.
                    _status[i].Direction = (i < i2);
                }
            }
            return true;
        }

        private void Swap(int idx1, int idx2)
        {
            var tempStatus = _status[idx1];
            _status[idx1] = _status[idx2];
            _status[idx2] = tempStatus;

            var temp = _sequence[idx1];
            _sequence[idx1] = _sequence[idx2];
            _sequence[idx2] = temp;
        }

        public void Reset()
        {
            // restore the initial sequence.
            var original = new T[_sequence.Length];
            for (var i = 0; i < _sequence.Length; i++)
            { // loop over all positions and restore them from the sequence.
                original[i] = _sequence[_status[i].Value - 1];
            }
            _sequence = original;
            _status = null;
        }

        private class ElementStatus
        {
            public ElementStatus(uint value, bool? direction)
            {
                this.Value = value;
                this.Direction = direction;
            }

            public uint Value { get; private set; }

            public bool? Direction { get; set; }

            public bool IsForward => this.Direction.HasValue && this.Direction.Value;
            
            public bool IsBackward => this.Direction.HasValue && !this.Direction.Value;
        }
    }
}