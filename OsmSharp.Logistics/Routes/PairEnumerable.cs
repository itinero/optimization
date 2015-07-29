// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

using System.Collections.Generic;

namespace OsmSharp.Logistics.Routes
{
    /// <summary>
    /// Enumerates all pairs in an IRoute.
    /// </summary>
    public class PairEnumerable : IEnumerable<Pair>
    {
        private readonly IRoute _route;
        private readonly int _start;

        /// <summary>
        /// Creates a new pair enumerable.
        /// </summary>
        public PairEnumerable(IRoute route)
        {
            _route = route;
            _start = _route.First;
        }

        /// <summary>
        /// Creates a new pair enumerable starting from the given customer.
        /// </summary>
        public PairEnumerable(IRoute route, int start)
        {
            _route = route;
            _start = start;
        }

        private class PairEnumerator : IEnumerator<Pair>
        {
            private Pair _current;
            private int _first;
            private IEnumerator<int> _enumerator;

            public PairEnumerator(IEnumerator<int> enumerator, int first)
            {
                _current = new Pair(-1, -1);
                _enumerator = enumerator;
                _first = first;
            }

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
                _enumerator = null;
            }

            object System.Collections.IEnumerator.Current
            {
                get { return this.Current; }
            }

            public bool MoveNext()
            {
                if (_current.From == -1 && _current.To == -1)
                {
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
                        _current.To = _enumerator.Current;
                    }
                    else if (_first >= 0 && _current.From != _first)
                    {
                        _current.To = _first;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (_current.To != _first && _current.From >= 0 && _current.To >= 0)
                {
                    if (_enumerator.MoveNext())
                    {
                        _current.From = _current.To;
                        _current.To = _enumerator.Current;
                    }
                    else if (_first >= 0)
                    {
                        _current.From = _current.To;
                        _current.To = _first;
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
                _current = new Pair(-1, -1);
            }
        }

        /// <summary>
        /// Returns an enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Pair> GetEnumerator()
        {
            if (_route.First == _route.Last)
            {
                return new PairEnumerator(_route.GetEnumerator(_start), _route.First);
            }
            return new PairEnumerator(_route.GetEnumerator(_start), -1);
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