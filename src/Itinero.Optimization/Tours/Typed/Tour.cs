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
using System.Linq;
using Itinero.Optimization.Algorithms;

namespace Itinero.Optimization.Tours.Typed
{
    /// <summary>
    /// a route or a sequence of customers.
    /// </summary>
    public abstract class Tour<T> : ITour<T>
        where T : struct
    {
        private readonly Tour _tour;

        protected Tour(Tour tour)
        {
            _tour = tour;
        }

        /// <summary>
        /// Creates a new route based on the given array.
        /// </summary>
        public Tour(IEnumerable<T> customers)
        {
            _tour = new Tour(customers.Select(x => this.GetId(x)));
        }

        /// <summary>
        /// Creates a new route based on the given array.
        /// </summary>
        public Tour(IEnumerable<T> customers, T? last)
        {
            int? lastId = null;
            if (last.HasValue)
            {
                lastId = this.GetId(last.Value);
            }
            _tour = new Tour(customers.Select(x => this.GetId(x)), lastId);
        }

        /// <summary>
        /// Gets the visit for the given id.
        /// </summary>
        public abstract T GetVisit(int id);

        /// <summary>
        /// Gets the id for the given visit.
        /// </summary>
        public abstract int GetId(T visit);

        /// <summary>
        /// Returns the amount of customers in the route.
        /// </summary>
        public int Count => _tour.Count;
        
        /// <summary>
        /// Returns the first customer.
        /// </summary>
        public T First => this.GetVisit(_tour.First);

        /// <summary>
        /// Returns the last customer.
        /// </summary>
        public T? Last
        {
            get
            {
                if (!_tour.Last.HasValue)
                {
                    return null;
                }
                return this.GetVisit(_tour.Last.Value);
            }
        }

        /// <summary>
        /// Returns true if there is an edge in this route from from to to.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns>True if there is an edge from->to in this route.</returns>
        /// <exception cref="System.ArgumentException">When from equals to.</exception>
        public bool Contains(T from, T to)
        {
            var fromId = this.GetId(from);
            var toId = this.GetId(to);
            return _tour.Contains(fromId, toId);
        }

        /// <summary>
        /// Returns true if the given customer is in this route.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns>True if the customer occurs in this route.</returns>
        public bool Contains(T customer)
        {
            var customerId = this.GetId(customer);
            return _tour.Contains(customerId);
        }

        /// <summary>
        /// Removes a customer from the route.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns>Return true if the customer was found and removed.</returns>
        /// <exception cref="System.InvalidOperationException">When attempting to remove the first customer.</exception>
        public bool Remove(T customer)
        {
            var customerId = this.GetId(customer);
            return _tour.Remove(customerId);
        }

        /// <summary>
        /// Removes a customer from the route.
        /// </summary>
        /// <param name="customer">The customer to remove.</param>
        /// <param name="after">The customer that used to exist after.</param>
        /// <param name="before">The customer that used to exist before.</param>
        /// <returns>Return true if the customer was found and removed.</returns>
        /// <exception cref="System.InvalidOperationException">When attempting to remove the first customer.</exception>
        public bool Remove(T customer, out T before, out T after)
        {
            var customerId = this.GetId(customer);
            int beforeId, afterId;
            if (!_tour.Remove(customerId, out beforeId, out afterId))
            {
                before = default(T);
                after = default(T);
                return false;
            }
            before = this.GetVisit(beforeId);
            after = this.GetVisit(afterId);
            return true;
        }

        /// <summary>
        /// Shifts the given customer to a new location and places it after the given 'before' customer.
        /// </summary>
        /// <param name="customer">The customer to shift.</param>
        /// <param name="before">The new customer that will come right before.</param>
        /// <returns>True if the operation succeeded.</returns>
        /// <remarks>example:
        /// route: 1->2->3->4->5->6
        ///     customer:   2
        ///     before:     4
        ///     
        /// new route: 1->3->4->2->5->6
        /// </remarks>
        /// <exception cref="System.ArgumentException">When customer equals before.</exception>
        public bool ShiftAfter(T customer, T before)
        {
            var customerId = this.GetId(customer);
            var beforeId = this.GetId(before);
            return _tour.ShiftAfter(customerId, beforeId);
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
        /// <remarks>example:
        /// route: 1->2->3->4->5->6
        ///     customer:   2
        ///     before:     4
        ///     
        /// new route: 1->3->4->2->5->6
        ///     oldBefore:  1
        ///     oldAfter:   3
        ///     newAfter:   5
        /// </remarks>
        /// <exception cref="System.ArgumentException">When customer equals before.</exception>
        public bool ShiftAfter(T customer, T before, out T oldBefore, out T oldAfter, out T newAfter)
        {
            var customerId = this.GetId(customer);
            var beforeId = this.GetId(before);
            int oldBeforeId, oldAfterId, newAfterId;
            if (!_tour.ShiftAfter(customerId, beforeId, out oldBeforeId, out oldAfterId, out newAfterId))
            {
                oldBefore = default(T);
                oldAfter = default(T);
                newAfter = default(T);
                return false;
            }
            oldBefore = this.GetVisit(oldBeforeId);
            oldAfter = this.GetVisit(oldAfterId);
            newAfter = this.GetVisit(newAfterId);
            return true;
        }

        /// <summary>
        /// Removes the edge from->unknown and replaces it with the edge from->to.
        /// 0->1->2:ReplaceEdgeFrom(0, 2):0->2 without resetting the last customer property.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <exception cref="System.ArgumentException">When from equals equals to.</exception>
        public void ReplaceEdgeFrom(T from, T to)
        {
            var fromId = this.GetId(from);
            var toId = this.GetId(to);
            _tour.ReplaceEdgeFrom(fromId, toId);
        }

        /// <summary>
        /// Replaces the old customer with the new customer, assuming the new customer isn't already part of the route.
        /// </summary>
        public void Replace(T oldCustomer, T newCustomer)
        {
            var oldCustomerId = this.GetId(oldCustomer);
            var newCustomerId = this.GetId(newCustomer);
            _tour.ReplaceEdgeFrom(oldCustomerId, newCustomerId);
        }

        /// <summary>
        /// Removes the edge from->unknown and replaces it with the edge from->to->unknown.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <exception cref="System.ArgumentException">When from equals equals to.</exception>
        /// <example>Route 0->1 after InsertAfter(0, 2) becomes 0->2->1.</example>
        /// <example>Route 0 after InsertAfter(0, 1) becomes 0->1.</example>
        public void InsertAfter(T from, T to)
        {
            var fromId = this.GetId(from);
            var toId = this.GetId(to);
            _tour.InsertAfter(fromId, toId);
        }

        /// <summary>
        /// Returns the neigbour of a customer.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns>The neighbours of the given customer.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">When the customer does not exist.</exception>
        public T GetNeigbour(T customer)
        {
            var customerId = this.GetId(customer);
            return this.GetVisit(_tour.GetNeigbour(customerId));
        }

        /// <summary>
        /// Returns the index of the given customer the first being zero.
        /// </summary>
        /// <param name="customer">The customer to search for.</param>
        /// <returns>The index of the customer, it's position relative to the first customers.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">When the customer does not exist.</exception>
        public int GetIndexOf(T customer)
        {
            var customerId = this.GetId(customer);
            return _tour.GetIndexOf(customerId);
        }

        /// <summary>
        /// Gets the customer at the given index.
        /// </summary>
        /// <param name="index">The position of the customer in the route, the first being at O.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">When the index is out of range.</exception>
        public T GetCustomerAt(int index)
        {
            return this.GetVisit(_tour.GetCustomerAt(index));
        }

        /// <summary>
        /// Returns an enumerable that enumerates between the two given customers.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns>En enumerable that enumerates customers between two given customers.</returns>
        /// <exception cref="System.ArgumentException">When from equals equals to.</exception>
        public IEnumerable<T> Between(T from, T to)
        {
            return _tour.Between(this.GetId(from), this.GetId(to)).Select(x => this.GetVisit(x));
        }

        /// <summary>
        /// Returns an enumerator that iterates through the customer in this route starting at the given customer.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator(T customer)
        {
            return new Enumerator<T>(this, _tour.GetEnumerator(this.GetId(customer)));
        }

        /// <summary>
        /// Returns an enumerable that enumerates all customer pairs that occur in the route as 1->2. If the route is a tour the pair that contains last->first is also included.
        /// </summary>
        /// <returns>An enumerable that enumerates all customer pairs that occur in the route as 1->2. If the route is a tour the pair that contains last->first is also included.</returns>
        public IEnumerable<Pair<T>> Pairs()
        {
            return new PairEnumerable<T>(this, _tour.Pairs());
        }

        /// <summary>
        /// Returns an enumerable that enumerates all customer triples that occur in the route as 1->2->3. If the route is a tour the tuples that contain last->first are also included.
        /// </summary>
        /// <returns>An enumerable that enumerates all customer triples that occur in the route as 1->2->3. If the route is a tour the tuples that contain last->first are also included.</returns>
        public IEnumerable<Triple<T>> Triples()
        {
            return new TripleEnumerable<T>(this, _tour.Triples());
        }

        /// <summary>
        /// Removes all customers in this route except the first one.
        /// </summary>
        public void Clear()
        {
            _tour.Clear();
        }

        /// <summary>
        /// Returns the enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator<T>(this, _tour.GetEnumerator());
        }

        /// <summary>
        /// Returns the enumerator.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator<T>(this, _tour.GetEnumerator());
        }

        /// <summary>
        /// Creates an exact deep-copy of this route.
        /// </summary>
        /// <returns></returns>
        public abstract object Clone();

        /// <summary>
        /// Copies the given solution into this solution.
        /// </summary>
        public void CopyFrom(ISolution solution)
        {
            var typedCopy = (solution as Tour<T>);
            if (typedCopy != null)
            {
                _tour.CopyFrom(typedCopy._tour);
            }
        }
    }
}