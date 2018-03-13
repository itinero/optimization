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
    /// A typed triple enumerable.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TripleEnumerable<T> : IEnumerable<Triple<T>>
        where T : struct
    {
        private readonly IEnumerable<Triple> _enumerable;
        private readonly Func<int, T> _getVisit;

        /// <summary>
        /// Creates anew triple enumerable.
        /// </summary>
        public TripleEnumerable(Func<int, T> getVisit, IEnumerable<Triple> enumerable)
        {
            _getVisit = getVisit;
            _enumerable = enumerable;
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Triple<T>> GetEnumerator()
        {
            return new TripleEnumerator<T>(_getVisit,
                _enumerable.GetEnumerator());
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new TripleEnumerator<T>(_getVisit,
                _enumerable.GetEnumerator());
        }
    }

    /// <summary>
    /// A typed triple enumerator.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TripleEnumerator<T> : IEnumerator<Triple<T>>
        where T : struct
    {
        private readonly IEnumerator<Triple> _enumerator;
        private readonly Func<int, T> _getVisit;

        /// <summary>
        /// Creates anew triple enumerator.
        /// </summary>
        public TripleEnumerator(Func<int, T> getVisit, IEnumerator<Triple> enumerator)
        {
            _getVisit = getVisit;
            _enumerator = enumerator;
        }

        /// <summary>
        /// Gets the current triple.
        /// </summary>
        public Triple<T> Current
        {
            get
            {
                var current = _enumerator.Current;
                return new Triple<T>(
                    _getVisit(current.From),
                    _getVisit(current.Along),
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
