using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Itinero.Optimization.Algorithms;

namespace Itinero.Optimization.Tours
{
    /// <summary>
    /// An asymetric dynamically sizeable mutliple routes object.
    /// </summary>
    public class MultiTour : IMultiTour, IEquatable<MultiTour>
    {
        protected int[] _nextArray;
        private SubTour[] _subtours;

        /// <summary>
        /// Creates a new dynamic route by creating shallow copy of the array(s) given.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="nextArray"></param>
        protected MultiTour(int[] first, int?[] last, int[] nextArray)
        {
            _nextArray = nextArray.Clone() as int[];

            _subtours = new SubTour[first.Length];
            for (int idx = 0; idx < first.Length; idx++)
            { // create the multi route parts.
                _subtours[idx] = new SubTour(this, first[idx], last[idx]);
            }
        }

        /// <summary>
        /// Creates a new dynamic route by creating shallow copy of the array(s) given.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="nextArray"></param>
        /// <param name="is_round"></param>
        private MultiTour(IEnumerable<SubTour> first, int[] nextArray)
        {
            _nextArray = nextArray.Clone() as int[];

            _subtours = new SubTour[first.Count<SubTour>()];
            int idx = 0;
            foreach (SubTour part in first)
            {
                _subtours[idx] = new SubTour(this, part.First, part.Last);
                idx++;
            }
        }

        /// <summary>
        /// Creates a new dynamic assymmetric route using an initial size and customer.
        /// </summary>
        public MultiTour(int size)
        {
            _nextArray = new int[size];
            for (int idx = 0; idx < size; idx++)
            {
                _nextArray[idx] = -1;
            }
            _subtours = new SubTour[0];
        }

        /// <summary>
        /// Adds a new empty route.
        /// </summary>
        public virtual ITour Add(int first, int? last)
        {
            // add one element to the first array.
            int routeIdx = _subtours.Length;
            Array.Resize<SubTour>(ref _subtours, _subtours.Length + 1);

            // create and set an empty route.
            _subtours[routeIdx] = new SubTour(this, first, last);

            // return the new route.
            _sizes = null;
            return this.Tour(routeIdx);
        }

        /// <summary>
        /// Adds a new tour by copying the given one.
        /// </summary>
        public virtual ITour Add(ITour tour)
        {
            bool first = true;
            int previous = -1;
            int route_idx = -1;
            foreach (int customer in tour)
            {
                if (first)
                {
                    // add one element to the first array.
                    route_idx = _subtours.Length;
                    Array.Resize<SubTour>(ref _subtours, _subtours.Length + 1);

                    // set the initial customer.
                    _subtours[route_idx] = new SubTour(this, tour.First, tour.Last);

                    // resize the array if needed.
                    if (_nextArray.Length <= customer)
                    { // resize the array.
                        this.Resize(customer);
                    }
                    _nextArray[customer] = -1;
                    first = false;
                }
                else
                {
                    _nextArray[previous] = customer;
                }

                // set the previous customer.
                previous = customer;
            }

            // return the new route.
            if (route_idx < 0)
            {
                return null;
            }

            _sizes = null;
            return this.Tour(route_idx);
        }

        /// <summary>
        /// Returns the route at the given index.
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public virtual ITour Tour(int idx)
        {
            return _subtours[idx];
        }

        /// <summary>
        /// Removes the given tour.
        /// </summary>
        public bool Remove(int tourIdx)
        {
            int start = _subtours[tourIdx].First;

            var subtours = new List<SubTour>(_subtours);
            subtours.RemoveAt(tourIdx);
            _subtours = subtours.ToArray();

            return true;
        }

        /// <summary>
        /// Resizes the array.
        /// </summary>
        /// <param name="visit"></param>
        private void Resize(int visit)
        { // THIS IS EXPENSIZE! TRY TO ESTIMATE CORRECT SIZE WHEN CREATING ROUTE!
            int old_size = _nextArray.Length;
            Array.Resize<int>(ref _nextArray, visit + 1);
            for (int new_customer = old_size; new_customer < _nextArray.Length; new_customer++)
            { // initialize with -1.
                _nextArray[new_customer] = -1;
            }
        }

        /// <summary>
        /// Returns the number of routes.
        /// </summary>
        public int Count
        {
            get
            {
                return _subtours.Length;
            }
        }

        /// <summary>
        /// Returns the size.
        /// </summary>
        public int Size
        {
            get
            {
                return _nextArray.Length;
            }
        }

        /// <summary>
        /// Returns true if the given visit is contained in this multi route.
        /// </summary>
        /// <param name="visit"></param>
        /// <returns></returns>
        public bool ContainsVisit(int visit)
        {
            foreach (var current in _subtours)
            {
                if (current.Contains(visit))
                {
                    return true;
                }
            }
            return false;
        }

        #region Sizes
        
        private int[] _sizes;

        /// <summary>
        /// Resets all sizes.
        /// </summary>
        private void ResetSizes()
        {
            _sizes = null;
        }

        /// <summary>
        /// Recalculate all sizes.
        /// </summary>
        private void RecalculateSizes()
        {
            _sizes = new int[_subtours.Length];
            for (int idx = 0; idx < _subtours.Length; idx++)
            {
                _sizes[idx] = this.Tour(idx).Count;
            }
        }

        ///// <summary>
        ///// Returns all sizes.
        ///// </summary>
        //public ReadOnlyCollection<int> Sizes
        //{
        //    get
        //    {
        //        if (_sizes == null)
        //        {
        //            this.RecalculateSizes();
        //        }
        //        return new ReadOnlyCollection<int>(_sizes);
        //    }
        //}

        /// <summary>
        /// Returns the count of all visits.
        /// </summary>
        public int VisitCount
        {
            get
            {
                int total = 0;
                foreach (int count in _sizes)
                {
                    total = total + count;
                }
                return total;
            }
        }

        #endregion

        /// <summary>
        /// Returns the customer after the given customer.
        /// </summary>
        public int Next(int customer)
        {
            int next = _nextArray[customer];
            if (next < 0)
            {
                for (int idx = 0; idx < this.Count; idx++)
                {
                    var tour = this.Tour(idx);
                    if (tour.Contains(customer) &&
                        tour.GetNeigbour(customer) == tour.First)
                    {
                        return tour.First;
                    }
                }
            }
            return next;
        }

        /// <summary>
        /// Returns true if the other multi route is equal in content.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(MultiTour other)
        {
            if (((object)this).Equals((object)other))
            {
                return true;
            }
            if (this.Count == other.Count)
            {
                if (this._nextArray.Length != other._nextArray.Length)
                {
                    return false;
                }

                // compare the initial customers.
                for (int route_idx = 0; route_idx < this.Count; route_idx++)
                {
                    if (this._subtours[route_idx] != other._subtours[route_idx])
                    {
                        return false;
                    }
                }

                // compare the next array.
                for (int idx = 0; idx < this._nextArray.Length; idx++)
                {
                    if (this._nextArray[idx] != other._nextArray[idx])
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns true if both multiroutes are equal in content.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        bool IEquatable<MultiTour>.Equals(MultiTour other)
        {
            return this.Equals(other);
        }

        /// <summary>
        /// Removes the given customer.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public bool RemoveVisit(int customer)
        {
            foreach (var part in _subtours)
            {
                if (part.Remove(customer))
                {
                    return true;
                }
            }
            
            return false;
        }

        /// <summary>
        /// Creates a deep copy of this multi route.
        /// </summary>
        /// <returns></returns>
        public virtual object Clone()
        {
            return new MultiTour(_subtours, _nextArray);
        }

        /// <summary>
        /// Returns true if the from-to is contained in this multi route.
        /// </summary>
        public bool Contains(int from, int to)
        {
            return _nextArray[from] == to;
        }

        public void CopyFrom(ISolution solution)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// A class exposing only the information about one route.
        /// </summary>
        private class SubTour : ITour
        {
            private int _first;
            private int? _last;
            private MultiTour _parent;
                        
            internal SubTour(MultiTour parent,
                int first, int? last)
            {
                _parent = parent;

                _first = first;
                _last = last;
            }

            private int _internalLast;

            /// <summary>
            /// Updates and sets the last customer.
            /// </summary>
            private void UpdateLast()
            {
                // TODO: this was an exact copy/past from tour, investigate sharing this code.
                _internalLast = _first;
                if (this.First != this.Last)
                {
                    while (_parent._nextArray[_internalLast] >= 0 && _parent._nextArray[_internalLast] != _first)
                    {
                        _internalLast = _parent._nextArray[_internalLast];
                    }
                }
            }

            /// <summary>
            /// Returns true if there exists an edge from the given customer to another.
            /// </summary>
            /// <param name="from"></param>
            /// <param name="to"></param>
            /// <returns></returns>
            public bool Contains(int from, int to)
            {
                // TODO: this was an exact copy/past from tour, investigate sharing this code.
                if (_parent._nextArray.Length > from)
                { // customers should exist.
                    if (_parent._nextArray[from] == to)
                    { // edge found.
                        return true;
                    }
                    else if (this.Contains(from) && (_parent._nextArray[from] == Constants.END))
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
            /// Inserts a customer right after from and before to.
            /// </summary>
            /// <param name="from"></param>
            /// <param name="customer"></param>
            public void ReplaceEdgeFrom(int from, int customer)
            {
                // TODO: this was an exact copy/past from tour, investigate sharing this code.
                if (from < 0)
                { // a new customer cannot be negative!
                    throw new ArgumentOutOfRangeException("Cannot add a customer after a customer with a negative index!");
                }

                if (customer == _first)
                { // the next customer is actually the first customer.
                  // set the next customer of the from customer to -1.
                    customer = Constants.END;
                }

                if (_parent._nextArray.Length > from)
                { // customers should exist.
                  // resize the array if needed.
                    if (_parent._nextArray.Length <= customer)
                    { // resize the array.
                        this.Resize(customer);
                    }

                    // insert customer.
                    _parent._nextArray[from] = customer;
                    return;
                }
                throw new ArgumentOutOfRangeException("Customer(s) do not exist in this route!");
            }

            /// <summary>
            /// Replaces the given old customer with the new customer. Assumes the new customer doesn't exist yet.
            /// </summary>
            public void Replace(int oldCustomer, int newCustomer)
            {
                if (oldCustomer == newCustomer)
                {
                    return;
                }
                if (newCustomer >= _parent._nextArray.Length)
                {
                    this.Resize(newCustomer);
                }

                _parent._nextArray[newCustomer] = _parent._nextArray[oldCustomer];
                _parent._nextArray[oldCustomer] = Constants.NOT_SET;

                if (oldCustomer == _first)
                {
                    _first = newCustomer;
                }

                if (oldCustomer == _last)
                {
                    _last = newCustomer;
                }

                for (var i = 0; i < _parent._nextArray.Length; i++)
                {
                    if (_parent._nextArray[i] == oldCustomer)
                    {
                        _parent._nextArray[i] = newCustomer;
                        break;
                    }
                }
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
                for (int idx = 0; idx < _parent._nextArray.Length; idx++)
                { // search for the 'before'.
                    if (_parent._nextArray[idx] == searchFor)
                    {
                        oldBefore = idx;
                        oldAfter = _parent._nextArray[customer];
                        if (oldBefore == before)
                        { // nothing to do here!
                            if (this.First == this.Last && oldAfter == Constants.END)
                            { // is closed and oldAfter is END then oldAfter is first.
                                oldAfter = this.First;
                            }

                            newAfter = oldAfter;
                            return true;
                        }
                        newAfter = _parent._nextArray[before];

                        // reorganize route.
                        _parent._nextArray[before] = searchFor;
                        _parent._nextArray[customer] = newAfter;
                        _parent._nextArray[oldBefore] = oldAfter;

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
            /// Inserts a customer right after from and before to.
            /// </summary>
            /// <param name="from"></param>
            /// <param name="customer"></param>
            public void InsertAfter(int from, int customer)
            {
                // TODO: this was an exact copy/past from tour, investigate sharing this code.
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
                if (_parent._nextArray.Length <= from)
                { // array is not big enough.
                    this.Resize(from);
                }
                if (_parent._nextArray.Length <= customer)
                { // resize the array.
                    this.Resize(customer);
                }

                // get the to customer if needed.
                var to = _parent._nextArray[from];
                if (to == Constants.NOT_SET)
                { // the to field is not set.
                    throw new ArgumentOutOfRangeException("from", string.Format("Customer {0} does not exist.", from));
                }

                // insert customer.
                _parent._nextArray[from] = customer;
                if (to == Constants.END)
                { // the to-customer is END.
                    if (this.First != this.Last)
                    { // update last.
                        _internalLast = customer;
                    }
                }

                // update the next for customer.
                _parent._nextArray[customer] = to;

                return;
            }

            /// <summary>
            /// Resizes the array.
            /// </summary>
            /// <param name="customer"></param>
            private void Resize(int customer)
            { // THIS EXPENSIZE! TRY TO ESTIMATE CORRECT SIZE WHEN CREATING ROUTE!
                // TODO: this was an exact copy/past from tour, investigate sharing this code.
                int old_size = _parent._nextArray.Length;
                Array.Resize<int>(ref _parent._nextArray, customer + 1);
                for (int new_customer = old_size; new_customer < _parent._nextArray.Length; new_customer++)
                { // initialize with -1.
                    _parent._nextArray[new_customer] = -1;
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
                // TODO: this was an exact copy/past from tour, investigate sharing this code.
                var cutPart = new int[length];
                var position = 0;
                var currentCustomer = this.First;

                // keep moving next until the start.
                while (position < start - 1)
                {
                    position++; // increase the position.
                    currentCustomer = _parent._nextArray[currentCustomer];
                }

                // cut the actual part.
                int startCustomer = currentCustomer;
                while (position < start + length - 1)
                {
                    // move next.
                    position++; // increase the position.
                    currentCustomer = _parent._nextArray[currentCustomer];

                    // set the current customer.
                    cutPart[position - start] = currentCustomer;
                }

                currentCustomer = _parent._nextArray[currentCustomer];

                // set the next customer.
                _parent._nextArray[startCustomer] = currentCustomer;

                return cutPart;
            }
            
            /// <summary>
            /// Returns the neigbour of the given customer.
            /// </summary>
            /// <param name="customer"></param>
            /// <returns></returns>
            public int GetNeigbour(int customer)
            {
                // TODO: this was an exact copy/past from tour, investigate sharing this code.
                var neighbour = _parent._nextArray[customer];
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

            #region Enumerators

            public IEnumerator<int> GetEnumerator()
            {
                return new Enumerator(_first, _parent._nextArray);
            }
            
            public IEnumerator<int> GetEnumerator(int customer)
            {
                return new Enumerator(_first, customer, _parent._nextArray);
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return new Enumerator(_first, _parent._nextArray);
            }

            private class Enumerator : IEnumerator<int>
            {
                // TODO: same here, exactly the same enumerator as in tour.
                private readonly int _first;
                private readonly int _start;
                private int[] _nextArray;

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

                private int _current = -1;

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
                    if (_current == -2)
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

            public IEnumerable<int> Between(int from, int to)
            {
                return new Tour.RouteBetweenEnumerable(_parent._nextArray, _first, _last, from, to);
            }

            #endregion
            
            public int Count
            {
                get
                {
                    // TODO: this was an exact copy/past from tour, investigate sharing this code.
                    throw new NotImplementedException("This linq-based stuff needs to go!");
                    //return this.Count<int>();
                }
            }
            
            public bool Remove(int customer)
            {
                // TODO: this was an exact copy/past from tour, investigate sharing this code.
                if (customer == _first)
                { // cannot remove the first customer.
                    throw new InvalidOperationException("Cannot remove first customer from a route.");
                }
                if (customer == _last)
                { // cannot remove the first customer.
                    throw new InvalidOperationException("Cannot remove last customer from a route.");
                }
                for (int idx = 0; idx < _parent._nextArray.Length; idx++)
                {
                    if (_parent._nextArray[idx] == customer)
                    {
                        _parent._nextArray[idx] = _parent._nextArray[customer];
                        _parent._nextArray[customer] = Constants.NOT_SET;
                        if (customer == _internalLast && this.First != this.Last)
                        { // update last if open problem.
                            _internalLast = idx;
                        }
                        return true;
                    }
                }
                return false;
            }
            
            public bool Remove(int customer, out int before, out int after)
            {
                // TODO: this was an exact copy/past from tour, investigate sharing this code.
                if (customer == _first)
                { // cannot remove the first customer.
                    throw new InvalidOperationException("Cannot remove first customer from a route.");
                }
                for (int idx = 0; idx < _parent._nextArray.Length; idx++)
                { // search for the 'before'.
                    if (_parent._nextArray[idx] == customer)
                    {
                        before = idx;
                        after = _parent._nextArray[customer];

                        _parent._nextArray[idx] = _parent._nextArray[customer];
                        _parent._nextArray[customer] = Constants.NOT_SET;
                        return true;
                    }
                }
                before = Constants.NOT_SET;
                after = Constants.NOT_SET;
                return false;
            }

            public int First
            {
                get
                {
                    return _first;
                }
            }
            
            public int? Last
            {
                get
                {
                    if (_last < 0)
                    {
                        // calculate the last customer.
                        this.UpdateLast();
                    }
                    return _last;
                }
            }

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

            public int GetIndexOf(int customer)
            {
                // TODO: this was an exact copy/past from tour, investigate sharing this code.
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

            public bool Contains(int customer)
            {
                // TODO: this was an exact copy/past from tour, investigate sharing this code.
                if (_parent._nextArray.Length > customer)
                {
                    if (_parent._nextArray[customer] >= 0)
                    { // customer is definetly contained.
                        return true;
                    }
                    return _parent._nextArray.Contains<int>(customer);
                }
                return false;
            }
            
            public override string ToString()
            {
                // TODO: this was an exact copy/past from tour, investigate sharing this code.
                int previous = -1;
                var result = new StringBuilder();
                foreach (int customer in this)
                {
                    if (previous < 0)
                    {
                        result.Append('[');
                        result.Append(customer);
                        result.Append(']');
                    }
                    else if (customer != this.Last)
                    {
                        result.Append("->");
                        result.Append(customer);
                    }
                    previous = customer;
                }

                if (this.Last.HasValue && this.Count > 1)
                {
                    result.Append("->[");
                    result.Append(this.Last.Value);
                    result.Append("]");
                }
                return result.ToString();
            }

            public object Clone()
            {
                throw new NotSupportedException("Cannot clone a route that's part of a multi-route.");
            }
            
            //public int Next(int customer)
            //{
            //    return _parent._nextArray[customer];
            //}

            /// <summary>
            /// Returns an enumerable that enumerates all customer pairs that occur in the route as 1->2. If the route is a round the pair that contains last->first is also included.
            /// </summary>
            /// <returns></returns>
            public IEnumerable<Pair> Pairs()
            {
                return new PairEnumerable<SubTour>(this, _first == _last);
            }

            /// <summary>
            /// Returns an enumerable that enumerates all customer triples that occur in the route as 1->2-3. If the route is a round the tuples that contain last->first are also included.
            /// </summary>
            /// <returns></returns>
            public IEnumerable<Triple> Triples()
            {
                return new TripleEnumerable(this);
            }

            public void Clear()
            {
                _first = -1;
            }

            public void CopyFrom(ISolution solution)
            { // TODO: investigate how this is used, it may be a solution in a subtour optimization.
                throw new Exception("Auwch, abstraction bleed, a subtour is not a solution.");
            }
        }
    }
}
