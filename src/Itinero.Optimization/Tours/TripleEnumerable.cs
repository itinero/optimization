// Itinero.Optimization - Route optimization for .NET
// Copyright (C) 2015 Abelshausen Ben
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

using System.Collections.Generic;

namespace Itinero.Optimization.Tours
{
    /// <summary>
    /// Enumerates all triples in an IRoute.
    /// </summary>
    public class TripleEnumerable : IEnumerable<Triple>
    {
        /// <summary>
        /// Holds the route being enumerated.
        /// </summary>
        private ITour _tour;

        /// <summary>
        /// Creates a new triple enumerable.
        /// </summary>
        /// <param name="route"></param>
        public TripleEnumerable(ITour tour)
        {
            _tour = tour;
        }

        private class TripleEnumerator : IEnumerator<Triple>
        {
            private Triple _current;

            private int _first;

            private int _second;

            private IEnumerator<int> _enumerator;

            public TripleEnumerator(IEnumerator<int> enumerator, int first)
            {
                _current = new Triple(-1, -1, -1);
                _enumerator = enumerator;
                _first = first;
            }

            public Triple Current
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
                if (_current.From == -1 && _current.To == -1 && _current.Along == -1)
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
                    { // last customer has been reach, include the first one as 'to'.
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
            if (_tour.First == _tour.Last)
            {
                return new TripleEnumerator(_tour.GetEnumerator(), _tour.First);
            }
            return new TripleEnumerator(_tour.GetEnumerator(), -1);
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