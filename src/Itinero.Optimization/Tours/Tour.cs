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

using Itinero.Optimization.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Itinero.Optimization.Tours
{
    /// <summary>
    /// a route or a sequence of customers.
    /// </summary>
    public sealed class Tour : ITour
    {
        private int[] _nextArray;
        private int _first;
        private int? _last;

        /// <summary>
        /// Creates a new route based on the given array.
        /// </summary>
        private Tour(int first, int[] nextArray, int? last)
        {
            _first = first;
            _last = last;
            _nextArray = nextArray;

            this.UpdateLast();
        }

        /// <summary>
        /// Creates a new closed route using a preexisting sequence.
        /// </summary>
        public Tour(IEnumerable<int> customers)
            : this(customers, customers.First())
        {

        }

        /// <summary>
        /// Creates a new route using a preexisting sequence.
        /// </summary>
        public Tour(IEnumerable<int> customers, int? last)
        {
            _nextArray = new int[0];
            var first = -1;
            var previous = -1;
            foreach (var customer in customers)
            {
                // resize the array if needed.
                if (_nextArray.Length <= customer)
                {
                    this.Resize(customer);
                }

                // the first customer.
                if (first < 0)
                { // set the first customer.
                    first = customer;
                }
                else
                { // set the next array.
                    _nextArray[previous] = customer;
                }

                previous = customer;
            }

            _nextArray[previous] = Constants.END;

            // set actual route-data.
            _first = first;
            _internalLast = previous;
            _last = last;
        }

        /// <summary>
        /// Returns true if there exists an edge from the given customer to another.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public bool Contains(int from, int to)
        {
            if (_nextArray.Length > from)
            { // customers should exist.
                if (_nextArray[from] == to)
                { // edge found.
                    return true;
                }
                else if (this.Contains(from) && _nextArray[from] == Constants.END)
                { // the from customer is contained but it does not have a next customer.
                    if (this.First == this.Last)
                    {
                        return to == _first;
                    }
                }
            }
            return false; // array too small.
        }

        /// <summary>
        /// Returns true if the customer exists in this route.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public bool Contains(int customer)
        {
            if (_nextArray.Length > customer)
            {
                if (_nextArray[customer] >= 0)
                { // customer is definetly contained.
                    return true;
                }
                return _nextArray.Contains<int>(customer);
            }
            return false;
        }

        /// <summary>
        /// Inserts a customer right after from and before to.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="customer"></param>
        public void ReplaceEdgeFrom(int from, int customer)
        {
            if (from < 0)
            { // a new customer cannot be negative!
                throw new ArgumentOutOfRangeException("Cannot add a customer after a customer with a negative index!");
            }

            if (customer == _first)
            { // the next customer is actually the first customer.
                // set the next customer of the from customer to -1.
                customer = Constants.END;
            }

            if (_nextArray.Length > from)
            { // customers should exist.
                // resize the array if needed.
                if (_nextArray.Length <= customer)
                { // resize the array.
                    this.Resize(customer);
                }

                // insert customer.
                _nextArray[from] = customer;
                return;
            }
            throw new ArgumentOutOfRangeException("Customer(s) do not exist in this route!");
        }

        /// <summary>
        /// Replaces the given old customer with the new customer. Assumes the new customer doesn't exist yet.
        /// </summary>
        public void Replace(int oldCustomer, int newCustomer)
        {
            if (newCustomer >= _nextArray.Length)
            {
                this.Resize(newCustomer);
            }

            _nextArray[newCustomer] = _nextArray[oldCustomer];
            _nextArray[oldCustomer] = Constants.NOT_SET;

            if (oldCustomer == _first)
            {
                _first = newCustomer;
            }

            if (oldCustomer == _last)
            {
                _last = newCustomer;
            }

            for (var i = 0; i < _nextArray.Length; i++)
            {
                if (_nextArray[i] == oldCustomer)
                {
                    _nextArray[i] = newCustomer;
                    break;
                }
            }
        }

        /// <summary>
        /// Inserts a customer right after from and before to.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="customer"></param>
        public void InsertAfter(int from, int customer)
        {
            if (customer < 0)
            { // a new customer cannot be negative!
                throw new ArgumentOutOfRangeException("customer", "Cannot add customers with a negative id!");
            }
            if (from < 0)
            { // a new customer cannot be negative!
                throw new ArgumentOutOfRangeException("from", "Cannot add a customer after a customer with a negative id!");
            }
            if (from == customer)
            { // the customer are identical.
                throw new ArgumentException("Cannot add a customer after itself.");
            }

            // resize the array if needed.
            if (_nextArray.Length <= from)
            { // array is not big enough.
                this.Resize(from);
            }
            if (_nextArray.Length <= customer)
            { // resize the array.
                this.Resize(customer);
            }

            // get the to customer if needed.
            var to = _nextArray[from];
            if (to == Constants.NOT_SET)
            { // the to field is not set.
                throw new ArgumentOutOfRangeException("from", string.Format("Customer {0} does not exist.", from));
            }

            // insert customer.
            _nextArray[from] = customer;
            if (to == Constants.END)
            { // the to-customer is END.
                if (this.First != this.Last)
                { // update last.
                    _internalLast = customer;
                }
            }

            // update the next for customer.
            _nextArray[customer] = to;

            return;
        }

        /// <summary>
        /// Resizes the array.
        /// </summary>
        /// <param name="customer"></param>
        private void Resize(int customer)
        { // THIS EXPENSIZE! TRY TO ESTIMATE CORRECT SIZE WHEN CREATING ROUTE!
            int old_size = _nextArray.Length;
            Array.Resize<int>(ref _nextArray, customer + 1);
            for (int newCustomer = old_size; newCustomer < _nextArray.Length; newCustomer++)
            { // initialize with NOT_SET.
                _nextArray[newCustomer] = Constants.NOT_SET;
            }
        }

        /// <summary>
        /// Cuts out a part of the route and returns the customers contained.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public int[] CutAndRemove(int start, int length)
        {
            var cutPart = new int[length];
            var position = 0;
            var currentCustomer = this.First;

            // keep moving next until the start.
            while (position < start - 1)
            {
                position++; // increase the position.
                currentCustomer = _nextArray[currentCustomer];
            }

            // cut the actual part.
            int startCustomer = currentCustomer;
            while (position < start + length - 1)
            {
                // move next.
                position++; // increase the position.
                currentCustomer = _nextArray[currentCustomer];

                // set the current customer.
                cutPart[position - start] = currentCustomer;
            }

            currentCustomer = _nextArray[currentCustomer];

            // set the next customer.
            _nextArray[startCustomer] = currentCustomer;

            return cutPart;
        }

        /// <summary>
        /// Returns the neigbour of the given customer.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public int GetNeigbour(int customer)
        {
            var neighbour = _nextArray[customer];
            if (neighbour < 0)
            {
                if (this.First == this.Last)
                {
                    neighbour = this.First;
                    return neighbour;
                }
                return Constants.NOT_SET;
            }
            return neighbour;
        }

        /// <summary>
        /// Returns the neigbour of the given customer.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public int Next(int customer)
        {
            return _nextArray[customer];
        }

        /// <summary>
        /// Creates an exact deep-copy of this route.
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new Tour(_first, _nextArray.Clone() as int[], _last);
        }

        /// <summary>
        /// Copies the given solution into this solution.
        /// </summary>
        public void CopyFrom(ISolution solution)
        {
            var copy = (solution as Tour);
            if (copy != null)
            {
                _first = copy._first;
                _internalLast = copy._internalLast;
                _last = copy._last;
                _nextArray = copy._nextArray.Clone() as int[];
            }
        }

        #region Enumerators

        /// <summary>
        /// Returns the enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<int> GetEnumerator()
        {
            return new Enumerator(_first, _nextArray);
        }

        /// <summary>
        /// Returns the enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<int> GetEnumerator(int customer)
        {
            return new Enumerator(_first, customer, _nextArray);
        }

        /// <summary>
        /// Returns the enumerator.
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new Enumerator(_first, _nextArray);
        }

        private class Enumerator : IEnumerator<int>
        {
            private int _first;
            private int _start;
            private int[] _nextArray;
            private int _current = -1;

            public Enumerator(int first, int[] nextArray)
            {
                _first = first;
                _start = first;
                _nextArray = nextArray;
            }

            public Enumerator(int first, int start, int[] nextArray)
            {
                _first = first;
                _start = start;
                _nextArray = nextArray;
            }

            public int Current
            {
                get
                {
                    return _current;
                }
            }

            public void Dispose()
            {
                _nextArray = null;
            }

            object System.Collections.IEnumerator.Current
            {
                get
                {
                    return this.Current;
                }
            }

            public bool MoveNext()
            {
                if(_current == -2)
                {
                    return false;
                }
                if (_current == -1)
                {
                    _current = _start;
                }
                else
                {
                    _current = _nextArray[_current];
                    if (_current == Constants.END)
                    {
                        return false;
                    }
                }
                return _current >= 0;
            }

            public void Reset()
            {
                _current = -1;
            }
        }

        /// <summary>
        /// Returns an enumerable between.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public IEnumerable<int> Between(int from, int to)
        {
            return new RouteBetweenEnumerable(_nextArray, _first, _last, from, to);
        }

        /// <summary>
        /// Returns an enumerable that enumerates all customer pairs that occur in the route as 1->2. If the route is a round the pair that contains last->first is also included.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Pair> Pairs()
        {
            return new PairEnumerable<Tour>(this, _first == _last);
        }
        
        /// <summary>
        /// Returns an enumerable that enumerates all customer triples that occur in the route as 1->2-3. If the route is a round the tuples that contain last->first are also included.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Triple> Triples()
        {
            return new TripleEnumerable(this);
        }

        #endregion

        /// <summary>
        /// Returns the size of the route.
        /// </summary>
        public int Count
        {
            get
            {
                return this.Count<int>();
            }
        }

        /// <summary>
        /// Removes a customer from the route.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public bool Remove(int customer)
        {
            if (customer == _first)
            { // cannot remove the first customer.
                throw new InvalidOperationException("Cannot remove first customer from a route.");
            }
            if (customer == _last)
            { // cannot remove the first customer.
                throw new InvalidOperationException("Cannot remove last customer from a route.");
            }
            for (int idx = 0; idx < _nextArray.Length; idx++)
            {
                if (_nextArray[idx] == customer)
                {
                    _nextArray[idx] = _nextArray[customer];
                    _nextArray[customer] = Constants.NOT_SET;
                    if(customer == _internalLast && this.First != this.Last)
                    { // update last if open problem.
                        _internalLast = idx;
                    }
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Removes a customer from the route.
        /// </summary>
        /// <param name="customer">The customer to remove.</param>
        /// <param name="after">The customer that used to exist after.</param>
        /// <param name="before">The customer that used to exist before.</param>
        /// <returns></returns>
        public bool Remove(int customer, out int before, out int after)
        {
            if (customer == _first)
            { // cannot remove the first customer.
                throw new InvalidOperationException("Cannot remove first customer from a route.");
            }
            for (int idx = 0; idx < _nextArray.Length; idx++)
            { // search for the 'before'.
                if (_nextArray[idx] == customer)
                {
                    before = idx;
                    after = _nextArray[customer];

                    _nextArray[idx] = _nextArray[customer];
                    _nextArray[customer] = Constants.NOT_SET;
                    return true;
                }
            }
            before = Constants.NOT_SET;
            after = Constants.NOT_SET;
            return false;
        }

        /// <summary>
        /// Shifts the given customer to a new location and places it after the given 'before' customer.
        /// </summary>
        /// <param name="customer">The customer to shift.</param>
        /// <param name="before">The new customer that will come right before.</param>
        /// <returns></returns>
        public bool ShiftAfter(int customer, int before)
        {
            int oldBefore, oldAfter, newAfter;
            return this.ShiftAfter(customer, before, out oldBefore, out oldAfter, out newAfter);
        }

        /// <summary>
        /// Shifts the given customer to a new location and places it after the given 'before' customer.
        /// </summary>
        /// <param name="customer">The customer to shift.</param>
        /// <param name="before">The new customer that will come right before.</param>
        /// <param name="oldBefore">The customer that used to exist before.</param>
        /// <param name="oldAfter">The customer that used to exist after.</param>
        /// <param name="newAfter">The customer that new exists after.</param>
        public bool ShiftAfter(int customer, int before, out int oldBefore, out int oldAfter, out int newAfter)
        {
            if (customer == before) { throw new ArgumentException("Cannot shift a customer after itself."); }
            var searchFor = customer;
            if (customer == _first)
            { // search for END when customer to insert is the first customer.
                searchFor = Constants.END;
            }
            for (int idx = 0; idx < _nextArray.Length; idx++)
            { // search for the 'before'.
                if (_nextArray[idx] == searchFor)
                {
                    oldBefore = idx;
                    oldAfter = _nextArray[customer];
                    if(oldBefore == before)
                    { // nothing to do here!
                        if (this.First == this.Last && oldAfter == Constants.END)
                        { // is closed and oldAfter is END then oldAfter is first.
                            oldAfter = this.First;
                        }

                        newAfter = oldAfter;
                        return true;
                    }
                    newAfter = _nextArray[before];

                    // reorganize route.
                    _nextArray[before] = searchFor;
                    _nextArray[customer] = newAfter;
                    _nextArray[oldBefore] = oldAfter;

                    if (this.First == this.Last && oldAfter == Constants.END)
                    { // is closed and oldAfter is END then oldAfter is first.
                        oldAfter = this.First;
                    }
                    if (this.First == this.Last && newAfter == Constants.END)
                    { // is closed and newAfter is END then newAfter is first.
                        newAfter = this.First;
                    }
                    return true;
                }
            }
            oldBefore = Constants.NOT_SET;
            oldAfter = Constants.NOT_SET;
            newAfter = Constants.NOT_SET;
            return false;
        }

        /// <summary>
        /// Returns the first customer in this route.
        /// </summary>
        public int First
        {
            get
            {
                return _first;
            }
        }

        /// <summary>
        /// Returns the last customer in this route.
        /// </summary>
        public int? Last
        {
            get
            {
                return _last;
            }
        }

        /// <summary>
        /// Returns the index of the given customer.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public int GetIndexOf(int customer)
        {
            var idx = 0;
            foreach (var possibleCustomer in this)
            {
                if (possibleCustomer == customer)
                {
                    return idx;
                }
                idx++;
            }
            return Constants.NOT_SET;
        }

        /// <summary>
        /// Gets the customer at the given index.
        /// </summary>
        /// <param name="index">The position of the customer in the route, the first being at O.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">When the index is out of range.</exception>
        public int GetCustomerAt(int index)
        {
            if (index < 0) { new ArgumentOutOfRangeException("No customer can ever exist at an index smaller than 0."); }

            var idx = 0;
            foreach (var possibleCustomer in this)
            {
                if (idx == index)
                {
                    return possibleCustomer;
                }
                idx++;
            }
            throw new ArgumentOutOfRangeException(string.Format("No customer found at index {0}.", index));
        }

        private int _internalLast;

        /// <summary>
        /// Updates and sets the last customer.
        /// </summary>
        private void UpdateLast()
        {
            _internalLast = _first;
            if (this.First != this.Last)
            {
                while (_nextArray[_internalLast] >= 0 && _nextArray[_internalLast] != _first)
                {
                    _internalLast = _nextArray[_internalLast];
                }
            }
        }

        /// <summary>
        /// Returns a description of this route.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            int previous = -1;
            StringBuilder result = new StringBuilder();
            foreach (int customer in this)
            {
                if (previous < 0)
                {
                    result.Append(customer);
                }
                else
                {
                    result.Append("->");
                    result.Append(customer);
                }
                previous = customer;
            }
            return result.ToString();
        }

        /// <summary>
        /// Clears out all the customers in this route.
        /// </summary>
        public void Clear()
        {
            _nextArray = new int[_first + 1];
            for(var idx = 0; idx < _nextArray.Length; idx++)
            {
                _nextArray[idx] = Constants.NOT_SET;
            }
            _nextArray[_first] = Constants.END;
            _internalLast = _first;
        }

        /// <summary>
        /// An enumerable to enumerate customers between two given customers.
        /// </summary>
        internal class RouteBetweenEnumerable : IEnumerable<int>
        {
            private readonly int _from;
            private readonly int _to;
            private readonly int _first;
            private readonly int? _last;
            private readonly int[] _nextArray;

            /// <summary>
            /// Creates a new between enumerable.
            /// </summary>
            public RouteBetweenEnumerable(int[] nextArray, int first, int? last, int from, int to)
            {
                _nextArray = nextArray;
                _first = first;
                _last = last;
                _from = from;
                _to = to;
            }

            private class BetweenEnumerator : IEnumerator<int>
            {
                private int _current = -1;
                private readonly int _from;
                private readonly int _to;
                private readonly int _first;
                private readonly int? _last;
                private readonly int[] _nextArray;

                public BetweenEnumerator(int[] nextArray, int from, int to, int first, int? last)
                {
                    _nextArray = nextArray;
                    _first = first;
                    _last = last;
                    _from = from;
                    _to = to;
                }

                public int Current
                {
                    get
                    {
                        return _current;
                    }
                }

                public void Dispose()
                {

                }

                object System.Collections.IEnumerator.Current
                {
                    get { return this.Current; }
                }

                public bool MoveNext()
                {
                    if(_first != _last && _from > _to)
                    {
                        return false;
                    }
                    if (_current == _to)
                    {
                        return false;
                    }
                    if (_current == Constants.END)
                    {
                        _current = _from;
                        return true;
                    }
                    if(_current == -1)
                    {
                        _current = _from;
                        return true;
                    }
                    _current = _nextArray[_current];
                    if (_current == Constants.END)
                    {
                        _current = _first;
                    }
                    return true;
                }

                public void Reset()
                {
                    _current = -1;
                }
            }

            /// <summary>
            /// Returns the enumerator.
            /// </summary>
            /// <returns></returns>
            public IEnumerator<int> GetEnumerator()
            {
                return new BetweenEnumerator(_nextArray, _from, _to, _first, _last);
            }

            /// <summary>
            /// Returns the enumerator.
            /// </summary>
            /// <returns></returns>
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }
    }
}