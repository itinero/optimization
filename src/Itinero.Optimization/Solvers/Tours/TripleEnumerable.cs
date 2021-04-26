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
    /// Enumerates all triples in a tour.
    /// </summary>
    public class TripleEnumerable : IEnumerable<Triple>
    {
        private readonly Tour _tour;

        /// <summary>
        /// Creates a new triple enumerable.
        /// </summary>
        /// <param name="tour">The tour to enumerate triples from</param>
        /// <param name="wrapAround">If true and the tour is a closed way, also include the triples (x, last, first) and (last, first, y)</param>
        public TripleEnumerable(Tour tour, bool wrapAround)
        {
            _tour = tour;
            _wrapAround = wrapAround;
        }
        
        private bool _wrapAround;

        private class TripleEnumerator : IEnumerator<Triple>
        {
            private readonly int _first;
            private readonly bool _wrapAround;

            public TripleEnumerator(IEnumerator<int> enumerator, int first, bool wrapAround)
            {
                _current = new Triple(-1, -1, -1);
                _enumerator = enumerator;
                _first = first;
                _wrapAround = wrapAround;
            }
            
            private Triple _current;
            private int _second;
            private IEnumerator<int> _enumerator;

            public Triple Current => _current;

            public void Dispose()
            {
                _enumerator.Dispose();
                _enumerator = null;
            }

            object System.Collections.IEnumerator.Current => this.Current;

            public bool MoveNext()
            {
                if (_current.From == -1 && _current.To == -1 && _current.Along == -1)
                {   // the case that we have to seed the first tuple
                    if (_enumerator.MoveNext())
                    {
                        _current.From = _enumerator.Current;
                    }
                    else
                    {
                        return false;
                    }

                    if (_enumerator.MoveNext())
                    {
                        _current.Along = _enumerator.Current;
                        _second = _current.Along;
                    }
                    else
                    {
                        return false;
                    }

                    if (_enumerator.MoveNext())
                    {
                        _current.To = _enumerator.Current;
                    }
                    else
                    {
                        return false;
                    }
                    return true;
                }
                else if (_current.To != _second && _current.From >= 0 && _current.To >= 0)
                {
                    if (_enumerator.MoveNext())
                    { // regular enumeration until the last customer.
                        _current.From = _current.Along;
                        _current.Along = _current.To;
                        _current.To = _enumerator.Current;
                    }
                    else if (_first >= 0 && _current.To != _first)
                    {
                        if (!_wrapAround)
                        {
                            return false; // wrap around is disabled; we just return
                        }
                        // last customer has been reached, include the first one as 'to'.
                        _current.From = _current.Along;
                        _current.Along = _current.To;
                        _current.To = _first;
                    }
                    else if (_first >= 0 && _current.To == _first)
                    { // first customer is now to, it needs to become 'along'.
                        _current.From = _current.Along;
                        _current.Along = _current.To;
                        _current.To = _second;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
                return true;
            }

            public void Reset()
            {
                _enumerator.Reset();
                _current = new Triple(-1, -1, -1);
            }
        }

        /// <summary>
        /// Returns an enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Triple> GetEnumerator()
        {
            return GetEnumerator(true);
        }

        /// <summary>
        /// Returns an enumerator. Wraparound can be specified here.
        /// </summary>
        /// <param name="wrapAround">When true, also returns the last to first pair.</param>
        /// <returns></returns>
        public IEnumerator<Triple> GetEnumerator(bool wrapAround)
        {
            if (_tour.First == _tour.Last)
            {
                return new TripleEnumerator(_tour.GetEnumerator(), _tour.First, wrapAround);
            }
            return new TripleEnumerator(_tour.GetEnumerator(), -1, wrapAround);
        }

        /// <summary>
        /// Returns an enumerator.
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}