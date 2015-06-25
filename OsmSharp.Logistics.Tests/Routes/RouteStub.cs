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

using OsmSharp.Logistics.Routes;
using System;
using System.Collections.Generic;

namespace OsmSharp.Logistics.Tests.Routes
{
    /// <summary>
    /// A simple route stub.
    /// </summary>
    public class RouteStub : IRoute
    {
        private List<int> _customers;
        private bool _isClosed;
        private bool _lastFixed;

        /// <summary>
        /// Creates a new route.
        /// </summary>
        public RouteStub(List<int> customers, bool isClosed)
        {
            _customers = customers;
            _isClosed = isClosed;
            _lastFixed = false;
        }

        /// <summary>
        /// Creates a new route.
        /// </summary>
        public RouteStub(int[] customers, bool isClosed)
        {
            _customers = new List<int>(customers);
            _isClosed = isClosed;
            _lastFixed = false;
        }

        /// <summary>
        /// Creates a new route.
        /// </summary>
        public RouteStub(List<int> customers, bool isClosed, bool lastFixed)
        {
            _customers = customers;
            _isClosed = isClosed;
            _lastFixed = lastFixed;
        }

        /// <summary>
        /// Creates a new route.
        /// </summary>
        public RouteStub(int[] customers, bool isClosed, bool lastFixed)
        {
            _customers = new List<int>(customers);
            _isClosed = isClosed;
            _lastFixed = lastFixed;
        }

        /// <summary>
        /// Returns true if this route is empty.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return this.Count == 0;
            }
        }

        /// <summary>
        /// Returns true if this route is closed.
        /// </summary>
        public bool IsClosed
        {
            get
            {
                return _isClosed;
            }
        }

        /// <summary>
        /// Returns true if the last customer is fixed.
        /// </summary>
        public bool IsLastFixed
        {
            get
            {
                return _lastFixed;
            }
        }

        /// <summary>
        /// Returns the customer count.
        /// </summary>
        public int Count
        {
            get
            {
                return _customers.Count;
            }
        }

        /// <summary>
        /// Returns true if the customers are contained in this route.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public bool Contains(int from, int to)
        {
            for (int idx = 0; idx < _customers.Count - 1; idx++)
            {
                if (_customers[idx] == from && _customers[idx + 1] == to)
                {
                    return true;
                }
            }
            if (this.IsClosed)
            {
                return _customers[0] == to && _customers[_customers.Count - 1] == from;
            }
            return false;
        }

        /// <summary>
        /// Removes a given customer.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public bool Remove(int customer)
        {
            int customer_idx = _customers.IndexOf(customer);
            if (customer_idx >= 0)
            {
                _customers.RemoveAt(customer_idx);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Replaces an edge.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="customer"></param>
        public void ReplaceEdgeFrom(int from, int customer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Inserts after.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="customer"></param>
        public void InsertAfter(int from, int customer)
        {
            int idx = _customers.IndexOf(from);
            if (idx < _customers.Count - 1)
            {
                _customers.Insert(idx + 1, customer);
            }
            else
            {
                _customers.Add(customer);
            }
        }

        /// <summary>
        /// Returns the neighbours.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public int[] GetNeigbours(int customer)
        {
            int[] neighbours;
            if (_customers.Count == 0)
            {
                return new int[0];
            }
            int idx = _customers.IndexOf(customer);
            if (idx == 0)
            {
                if (_isClosed)
                {
                    neighbours = new int[2];
                    neighbours[0] = _customers[1];
                    neighbours[0] = _customers[_customers.Count - 1];
                    return neighbours;
                }
                else
                {
                    neighbours = new int[1];
                    neighbours[0] = _customers[1];
                    return neighbours;
                }
            }
            if (idx == _customers.Count - 1)
            {
                if (_isClosed)
                {
                    neighbours = new int[2];
                    neighbours[0] = _customers[0];
                    neighbours[0] = _customers[_customers.Count - 2];
                    return neighbours;
                }
                else
                {
                    neighbours = new int[1];
                    neighbours[0] = _customers[_customers.Count - 2];
                    return neighbours;
                }
            }

            neighbours = new int[2];
            neighbours[0] = _customers[idx - 1];
            neighbours[0] = _customers[idx + 1];
            return neighbours;
        }

        /// <summary>
        /// Returns true if this route is valid.
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            return true;
        }


        #region Enumerators

        /// <summary>
        /// Returns the enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<int> GetEnumerator()
        {
            return _customers.GetEnumerator();
        }

        /// <summary>
        /// Returns the enumerator.
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _customers.GetEnumerator();
        }

        #endregion

        /// <summary>
        /// Returns the first customer in this route.
        /// </summary>
        public int First
        {
            get
            {
                if (_customers.Count > 0)
                {
                    return _customers[0];
                }
                return -1;
            }
        }

        /// <summary>
        /// Returns the last customer in this route.
        /// </summary>
        public int Last
        {
            get
            {
                if (_customers.Count > 0)
                {
                    if (this.IsClosed)
                    {
                        return this.First;
                    }
                    return _customers[this.Count - 1];
                }
                return -1;
            }
        }

        /// <summary>
        /// Returns the index of the given customer.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public int GetIndexOf(int customer)
        {
            return _customers.IndexOf(customer);
        }

        /// <summary>
        /// Returns a enumerable between.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public IEnumerable<int> Between(int from, int to)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns true if the route contains the given customer.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public bool Contains(int customer)
        {
            return _customers.Contains(customer);
        }

        /// <summary>
        /// Clones this route.
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns an enumerable that enumerates all customer pairs that occur in the route as 1->2. If the route is a round the pair that contains last->first is also included.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Pair> Pairs()
        {
            return new PairEnumerable(this);
        }

        /// <summary>
        /// Returns an enumerable that enumerates all customer triples that occur in the route as 1->2-3. If the route is a round the tuples that contain last->first are also included.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Triple> Triples()
        {
            return new TripleEnumerable(this);
        }

        /// <summary>
        /// Inserts a new first customer.
        /// </summary>
        /// <param name="first"></param>
        public void InsertFirst(int first)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Replaces the first customer.
        /// </summary>
        /// <param name="first"></param>
        public void ReplaceFirst(int first)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Clears all customers.
        /// </summary>
        public void Clear()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
        /// <returns></returns>
        public bool ShiftAfter(int customer, int before, out int oldBefore, out int oldAfter, out int newAfter)
        {
            // remove.
            int indexOf = _customers.IndexOf(customer);
            oldBefore = _customers[indexOf - 1];
            oldAfter = _customers[indexOf + 1];
            _customers.RemoveAt(indexOf);

            // insert.
            indexOf = _customers.IndexOf(before);
            if(indexOf + 1 < _customers.Count)
            {
                newAfter = _customers[indexOf + 1];
            }
            else
            {
                newAfter = _customers[0];
            }
            _customers.Insert(indexOf + 1, customer);
            return true;
        }
    }
}