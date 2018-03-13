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
    /// A typed enumerator.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct Enumerator<T> : IEnumerator<T>
        where T : struct
    {
        private readonly IEnumerator<int> _enumerator;
        private readonly Func<int, T> _getVisit;

        /// <summary>
        /// Creates a new enumerator.
        /// </summary>
        /// <param name="getVisit">The get visit callback.</param>
        /// <param name="enumerator">The enumerator.</param>
        public Enumerator(Func<int, T> getVisit, IEnumerator<int> enumerator)
        {
            _enumerator = enumerator;
            _getVisit = getVisit;
        }

        /// <summary>
        /// Gets the current item.
        /// </summary>
        public T Current
        {
            get
            {
                return _getVisit(_enumerator.Current);
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