// Itinero.Optimization - Route optimization for .NET
// Copyright (C) 2016 Abelshausen Ben
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