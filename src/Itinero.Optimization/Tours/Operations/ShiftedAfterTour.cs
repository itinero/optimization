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

namespace Itinero.Optimization.Tours.Operations
{
    /// <summary>
    /// Represents a shifted after tour.
    /// </summary>
    public class ShiftedAfterTour : IEnumerable<int>
    {
        private readonly ITour _tour;
        private int _before;
        private int _customer;

        /// <summary>
        /// Creates a new shifted after tour.
        /// </summary>
        public ShiftedAfterTour(ITour tour, int customer, int before)
        {
            _tour = tour;
            _customer = customer;
            _before = before;
        }

        struct ShiftedAfterRouterEnumerator : IEnumerator<int>
        {
            private readonly ShiftedAfterTour _tour;
            private readonly IEnumerator<int> _tourEnumerator;

            public ShiftedAfterRouterEnumerator(ShiftedAfterTour tour)
            {
                _tour = tour;
                _tourEnumerator = _tour._tour.GetEnumerator();

                _next = Constants.NOT_SET;
                _current = Constants.NOT_SET;
            }

            private int _next;
            private int _current;
            
            public int Current
            {
                get
                {
                    return _current;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return _current;
                }
            }

            public void Dispose()
            {

            }

            public bool MoveNext()
            {
                if (_next != Constants.NOT_SET)
                {
                    _current = _next;
                    _next = Constants.NOT_SET;
                    return true;
                }

                if (!_tourEnumerator.MoveNext())
                {
                    return false;
                }

                _current = _tourEnumerator.Current;
                if (_current == _tour._customer)
                { // skip if current is the to-be-replaced customer.
                    if (!_tourEnumerator.MoveNext())
                    {
                        return false;
                    }
                    _current = _tourEnumerator.Current;
                }

                if (_current == _tour._before)
                { // set the customer right after the before customer.
                    _next = _tour._customer;
                }
                return true;
            }

            public void Reset()
            {
                _next = Constants.NOT_SET;
                _current = Constants.NOT_SET;
            }
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<int> GetEnumerator()
        {
            return new ShiftedAfterRouterEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new ShiftedAfterRouterEnumerator(this);
        }
    }
}