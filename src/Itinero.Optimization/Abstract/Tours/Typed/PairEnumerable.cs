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
using System.Collections;
using System.Collections.Generic;

namespace Itinero.Optimization.Abstract.Tours.Typed
{
    /// <summary>
    /// A typed pair enumerable.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PairEnumerable<T> : IEnumerable<Pair<T>>
        where T : struct
    {
        private readonly IEnumerable<Pair> _enumerable;
        private readonly Func<int, T> _getVisit;

        /// <summary>
        /// Creates anew pair enumerable.
        /// </summary>
        public PairEnumerable(Func<int, T> getVisit, IEnumerable<Pair> enumerable)
        {
            _getVisit = getVisit;
            _enumerable = enumerable;
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Pair<T>> GetEnumerator()
        {
            return new PairEnumerator<T>(_getVisit,
                _enumerable.GetEnumerator());
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new PairEnumerator<T>(_getVisit,
                _enumerable.GetEnumerator());
        }
    }

    /// <summary>
    /// A typed pair enumerator.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PairEnumerator<T> : IEnumerator<Pair<T>>
        where T : struct
    {
        private readonly IEnumerator<Pair> _enumerator;
        private readonly Func<int, T> _getVisit;

        /// <summary>
        /// Creates anew pair enumerator.
        /// </summary>
        public PairEnumerator(Func<int, T> getVisit, IEnumerator<Pair> enumerator)
        {
            _getVisit = getVisit;
            _enumerator = enumerator;
        }

        /// <summary>
        /// Gets the current pair.
        /// </summary>
        public Pair<T> Current
        {
            get
            {
                var current = _enumerator.Current;
                return new Pair<T>(
                    _getVisit(current.From),
                    _getVisit(current.To));
            }
        }

        /// <summary>
        /// Gets the current item.
        /// </summary>
        object IEnumerator.Current => this.Current;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _enumerator.Dispose();
        }

        /// <summary>
        /// Move to the next item
        /// </summary>
        /// <returns></returns>
        public bool MoveNext()
        {
            return _enumerator.MoveNext();
        }

        /// <summary>
        /// Resets this enumerator.
        /// </summary>
        public void Reset()
        {
            _enumerator.Reset();
        }
    }
}
