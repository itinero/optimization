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

namespace Itinero.Optimization.Tours
{
    /// <summary>
    /// Enumerates all pairs in an IRoute.
    /// </summary>
    public sealed class PairEnumerable<T> : IEnumerable<Pair>
        where T : IEnumerable<int>
    {
        private readonly T _tour;
        private readonly bool _isClosed;
        private readonly int _start;
        
        /// <summary>
        /// Creates a new pair enumerable starting from the given customer.
        /// </summary>
        public PairEnumerable(T tour, bool isClosed, int start = Constants.NOT_SET)
        {
            _start = start;
            _tour = tour;
            _isClosed = isClosed;
        }

        private sealed class PairEnumerator : IEnumerator<Pair>
        {
            private readonly bool _isClosed;
            private readonly IEnumerator<int> _enumerator;
            private readonly int _start;

            public PairEnumerator(IEnumerator<int> enumerator, bool isClosed, int start = Constants.NOT_SET)
            {
                _enumerator = enumerator;
                _isClosed = isClosed;
                _start = start;

                _current = new Pair(Constants.NOT_SET, Constants.NOT_SET);
                _first = Constants.NOT_SET;
                _startOk = false;
            }

            private Pair _current;
            private int _first;
            private bool _startOk;

            public Pair Current
            {
                get
                {
                    return _current;
                }
            }

            public void Dispose()
            {
                _enumerator.Dispose();
            }

            object System.Collections.IEnumerator.Current
            {
                get { return this.Current; }
            }

            public bool MoveNext()
            {
                if (_enumerator.MoveNext())
                {
                    if (_first == Constants.NOT_SET && _isClosed)
                    {
                        _first = _enumerator.Current;
                    }

                    // move to start first if it's set.
                    if (!_startOk)
                    {
                        while (_start != Constants.NOT_SET &&
                            _enumerator.Current != _start)
                        {
                            if (!_enumerator.MoveNext())
                            {
                                return false;
                            }
                        }
                    }
                    _startOk = true;

                    _current.From = _current.To;
                    _current.To = _enumerator.Current;
                    if (_current.From == Constants.NOT_SET)
                    {
                        if (!_enumerator.MoveNext())
                        {
                            if (_first != Constants.NOT_SET)
                            {
                                if (_first == _current.To)
                                {
                                    _first = Constants.NOT_SET;
                                    return false;
                                }
                                _current.From = _current.To;
                                _current.To = _first;
                                _first = Constants.NOT_SET;
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        _current.From = _current.To;
                        _current.To = _enumerator.Current;
                    }
                    return true;
                }
                else
                {
                    if (_first != Constants.NOT_SET)
                    {
                        if (_first == _current.To)
                        {
                            _first = Constants.NOT_SET;
                            return false;
                        }
                        _current.From = _current.To;
                        _current.To = _first;
                        _first = Constants.NOT_SET;
                        return true;
                    }
                }
                return false;
            }

            public void Reset()
            {
                _enumerator.Reset();

                _current = new Pair(-1, -1);
                _first = Constants.NOT_SET;
                _startOk = false;
            }
        }

        /// <summary>
        /// Returns an enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Pair> GetEnumerator()
        {
            return new PairEnumerator(_tour.GetEnumerator(), _isClosed, _start);
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