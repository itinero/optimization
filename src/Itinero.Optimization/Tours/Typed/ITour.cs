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
 
using Itinero.Optimization.Algorithms;
using System.Collections.Generic;

namespace Itinero.Optimization.Tours.Typed
{
    /// <summary>
    /// Abstract representation of a route or a sequence of customers.
    /// </summary>
    public interface ITour<T> : IEnumerable<T>, ISolution
        where T : struct
    {
        /// <summary>
        /// Returns the amount of customers in the route.
        /// </summary>
        int Count
        {
            get;
        }

        /// <summary>
        /// Returns the first customer.
        /// </summary>
        T First
        {
            get;
        }

        /// <summary>
        /// Returns the last customer.
        /// </summary>
        T? Last
        {
            get;
        }

        /// <summary>
        /// Returns true if there is an edge in this route from from to to.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns>True if there is an edge from->to in this route.</returns>
        /// <exception cref="System.ArgumentException">When from equals to.</exception>
        bool Contains(T from, T to);

        /// <summary>
        /// Returns true if the given customer is in this route.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns>True if the customer occurs in this route.</returns>
        bool Contains(T customer);

        /// <summary>
        /// Removes a customer from the route.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns>Return true if the customer was found and removed.</returns>
        /// <exception cref="System.InvalidOperationException">When attempting to remove the first customer.</exception>
        bool Remove(T customer);

        /// <summary>
        /// Removes a customer from the route.
        /// </summary>
        /// <param name="customer">The customer to remove.</param>
        /// <param name="after">The customer that used to exist after.</param>
        /// <param name="before">The customer that used to exist before.</param>
        /// <returns>Return true if the customer was found and removed.</returns>
        /// <exception cref="System.InvalidOperationException">When attempting to remove the first customer.</exception>
        bool Remove(T customer, out T before, out T after);

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
        bool ShiftAfter(T customer, T before);

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
        bool ShiftAfter(T customer, T before, out T oldBefore, out T oldAfter, out T newAfter);

        /// <summary>
        /// Removes the edge from->unknown and replaces it with the edge from->to.
        /// 0->1->2:ReplaceEdgeFrom(0, 2):0->2 without resetting the last customer property.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <exception cref="System.ArgumentException">When from equals equals to.</exception>
        void ReplaceEdgeFrom(T from, T to);

        /// <summary>
        /// Replaces the old customer with the new customer, assuming the new customer isn't already part of the route.
        /// </summary>
        void Replace(T oldCustomer, T newCustomer);

        /// <summary>
        /// Removes the edge from->unknown and replaces it with the edge from->to->unknown.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <exception cref="System.ArgumentException">When from equals equals to.</exception>
        /// <example>Route 0->1 after InsertAfter(0, 2) becomes 0->2->1.</example>
        /// <example>Route 0 after InsertAfter(0, 1) becomes 0->1.</example>
        void InsertAfter(T from, T to);

        /// <summary>
        /// Returns the neigbour of a customer.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns>The neighbours of the given customer.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">When the customer does not exist.</exception>
        T GetNeigbour(T customer);

        /// <summary>
        /// Returns the index of the given customer the first being zero.
        /// </summary>
        /// <param name="customer">The customer to search for.</param>
        /// <returns>The index of the customer, it's position relative to the first customers.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">When the customer does not exist.</exception>
        int GetIndexOf(T customer);

        /// <summary>
        /// Gets the customer at the given index.
        /// </summary>
        /// <param name="index">The position of the customer in the route, the first being at O.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">When the index is out of range.</exception>
        T GetVisitAt(int index);

        /// <summary>
        /// Returns an enumerable that enumerates between the two given customers.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns>En enumerable that enumerates customers between two given customers.</returns>
        /// <exception cref="System.ArgumentException">When from equals equals to.</exception>
        IEnumerable<T> Between(T from, T to);

        /// <summary>
        /// Returns an enumerator that iterates through the customer in this route starting at the given customer.
        /// </summary>
        /// <returns></returns>
        IEnumerator<T> GetEnumerator(T customer);

        /// <summary>
        /// Returns an enumerable that enumerates all customer pairs that occur in the route as 1->2. If the route is a tour the pair that contains last->first is also included.
        /// </summary>
        /// <returns>An enumerable that enumerates all customer pairs that occur in the route as 1->2. If the route is a tour the pair that contains last->first is also included.</returns>
        IEnumerable<Pair<T>> Pairs();

        /// <summary>
        /// Returns an enumerable that enumerates all customer triples that occur in the route as 1->2->3. If the route is a tour the tuples that contain last->first are also included.
        /// </summary>
        /// <returns>An enumerable that enumerates all customer triples that occur in the route as 1->2->3. If the route is a tour the tuples that contain last->first are also included.</returns>
        IEnumerable<Triple<T>> Triples();

        /// <summary>
        /// Removes all customers in this route except the first one.
        /// </summary>
        void Clear();
    }
}