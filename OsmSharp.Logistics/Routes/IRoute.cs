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
    /// Abstract representation of a route or a sequence of customers.
    /// </summary>
    public interface IRoute : IEnumerable<int>, ICloneable
    {
        /// <summary>
        /// Returns true if the last customer is linked with the first one.
        /// </summary>
        /// <remarks>
        /// When the route is closed the first customer follows the last. This means the a pair and triple will existing containing the part last->first.
        /// </remarks>
        bool IsClosed
        {
            get;
        }

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
        /// <returns></returns>
        int First
        {
            get;
        }

        /// <summary>
        /// Returns the last customer.
        /// </summary>
        /// <remarks>
        /// Returns the first customer when this route is closed, can return any customer when the route is not closed.
        /// </remarks>
        int Last
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
        bool Contains(int from, int to);

        /// <summary>
        /// Returns true if the given customer is in this route.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns>True if the customer occurs in this route.</returns>
        bool Contains(int customer);

        /// <summary>
        /// Removes a customer from the route.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns>Return true if the customer was found and removed.</returns>
        /// <exception cref="System.InvalidOperationException">When attempting to remove the first customer.</exception>
        bool Remove(int customer);

        /// <summary>
        /// Removes a customer from the route.
        /// </summary>
        /// <param name="customer">The customer to remove.</param>
        /// <param name="after">The customer that used to exist after.</param>
        /// <param name="before">The customer that used to exist before.</param>
        /// <returns>Return true if the customer was found and removed.</returns>
        /// <exception cref="System.InvalidOperationException">When attempting to remove the first customer.</exception>
        bool Remove(int customer, out int before, out int after);

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
        bool ShiftAfter(int customer, int before);

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
        bool ShiftAfter(int customer, int before, out int oldBefore, out int oldAfter, out int newAfter);

        /// <summary>
        /// Removes the edge from->unknown and replaces it with the edge from->to.
        /// 0->1->2:ReplaceEdgeFrom(0, 2):0->2 without resetting the last customer property.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <exception cref="System.ArgumentException">When from equals equals to.</exception>
        void ReplaceEdgeFrom(int from, int to);

        /// <summary>
        /// Removes the edge from->unknown and replaces it with the edge from->to->unknown.
        /// 0->1:InsertAfter(0, 2):0->2-1
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <exception cref="System.ArgumentException">When from equals equals to.</exception>
        void InsertAfter(int from, int to);

        /// <summary>
        /// Returns the neigbours of a customer.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns>The neighbours of the given customer.</returns>
        /// <exception cref="System.ArgumentOutOfRanceException">When the customer does not exist.</exception>
        int[] GetNeigbours(int customer);

        /// <summary>
        /// Returns the index of the given customer the first being zero.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns>The index of the customer, it's position relative to the first customers.</returns>
        /// <exception cref="System.ArgumentOutOfRanceException">When the customer does not exist.</exception>
        int GetIndexOf(int customer);

        /// <summary>
        /// Returns an enumerable that enumerates between the two given customers.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns>En enumerable that enumerates customers between two given customers.</returns>
        /// <exception cref="System.ArgumentException">When from equals equals to.</exception>
        IEnumerable<int> Between(int from, int to);

        /// <summary>
        /// Returns an enumerable that enumerates all customer pairs that occur in the route as 1->2. If the route is a round the pair that contains last->first is also included.
        /// </summary>
        /// <returns>An enumerable that enumerates all customer pairs that occur in the route as 1->2. If the route is a round the pair that contains last->first is also included.</returns>
        IEnumerable<Pair> Pairs();

        /// <summary>
        /// Returns an enumerable that enumerates all customer triples that occur in the route as 1->2-3. If the route is a round the tuples that contain last->first are also included.
        /// </summary>
        /// <returns>An enumerable that enumerates all customer triples that occur in the route as 1->2-3. If the route is a round the tuples that contain last->first are also included.</returns>
        IEnumerable<Triple> Triples();

        /// <summary>
        /// Removes all customers in this route except the first one.
        /// </summary>
        void Clear();
    }
}
