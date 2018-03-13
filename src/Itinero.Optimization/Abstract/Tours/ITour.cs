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

namespace Itinero.Optimization.Abstract.Tours
{
    /// <summary>
    /// Abstract representation of a route or a sequence of customers.
    /// </summary>
    public interface ITour : IEnumerable<int>, ISolution
    {
        /// <summary>
        /// Returns the amount of customers in the route.
        /// </summary>
        int Count
        {
            get;
        }

        /// <summary>
        /// Returns the first visit.
        /// </summary>
        /// <returns></returns>
        int First
        {
            get;
        }

        /// <summary>
        /// Returns the last visit.
        /// </summary>
        int? Last
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
        /// Returns true if the given visit is in this route.
        /// </summary>
        /// <param name="visit"></param>
        /// <returns>True if the visit occurs in this route.</returns>
        bool Contains(int visit);

        /// <summary>
        /// Removes a visit from the route.
        /// </summary>
        /// <param name="visit"></param>
        /// <returns>Return true if the visit was found and removed.</returns>
        /// <exception cref="System.InvalidOperationException">When attempting to remove the first visit.</exception>
        bool Remove(int visit);

        /// <summary>
        /// Removes a visit from the route.
        /// </summary>
        /// <param name="visit">The visit to remove.</param>
        /// <param name="after">The visit that used to exist after.</param>
        /// <param name="before">The visit that used to exist before.</param>
        /// <returns>Return true if the visit was found and removed.</returns>
        /// <exception cref="System.InvalidOperationException">When attempting to remove the first visit.</exception>
        bool Remove(int visit, out int before, out int after);

        /// <summary>
        /// Shifts the given visit to a new location and places it after the given 'before' visit.
        /// </summary>
        /// <param name="visit">The visit to shift.</param>
        /// <param name="before">The new visit that will come right before.</param>
        /// <returns>True if the operation succeeded.</returns>
        /// <remarks>example:
        /// route: 1->2->3->4->5->6
        ///     visit:   2
        ///     before:     4
        ///     
        /// new route: 1->3->4->2->5->6
        /// </remarks>
        /// <exception cref="System.ArgumentException">When visit equals before.</exception>
        bool ShiftAfter(int visit, int before);

        /// <summary>
        /// Shifts the given visit to a new location and places it after the given 'before' visit.
        /// </summary>
        /// <param name="visit">The visit to shift.</param>
        /// <param name="before">The new visit that will come right before.</param>
        /// <param name="oldBefore">The visit that used to exist before.</param>
        /// <param name="oldAfter">The visit that used to exist after.</param>
        /// <param name="newAfter">The visit that new exists after.</param>
        /// <returns></returns>
        /// <remarks>example:
        /// route: 1->2->3->4->5->6
        ///     visit:   2
        ///     before:     4
        ///     
        /// new route: 1->3->4->2->5->6
        ///     oldBefore:  1
        ///     oldAfter:   3
        ///     newAfter:   5
        /// </remarks>
        /// <exception cref="System.ArgumentException">When visit equals before.</exception>
        bool ShiftAfter(int visit, int before, out int oldBefore, out int oldAfter, out int newAfter);

        /// <summary>
        /// Removes the edge from->unknown and replaces it with the edge from->to.
        /// 0->1->2:ReplaceEdgeFrom(0, 2):0->2 without resetting the last visit property.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <exception cref="System.ArgumentException">When from equals equals to.</exception>
        void ReplaceEdgeFrom(int from, int to);

        /// <summary>
        /// Replaces the old visit with the new visit, assuming the new visit isn't already part of the route.
        /// </summary>
        void Replace(int oldVisit, int newVisit);

        /// <summary>
        /// Removes the edge from->unknown and replaces it with the edge from->to->unknown.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <exception cref="System.ArgumentException">When from equals equals to.</exception>
        /// <example>Route 0->1 after InsertAfter(0, 2) becomes 0->2->1.</example>
        /// <example>Route 0 after InsertAfter(0, 1) becomes 0->1.</example>
        void InsertAfter(int from, int to);

        /// <summary>
        /// Returns the neigbour of a visit.
        /// </summary>
        /// <param name="visit"></param>
        /// <returns>The neighbours of the given visit.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">When the visit does not exist.</exception>
        int GetNeigbour(int visit);

        /// <summary>
        /// Returns the index of the given visit the first being zero.
        /// </summary>
        /// <param name="visit">The visit to search for.</param>
        /// <returns>The index of the visit, it's position relative to the first customers.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">When the visit does not exist.</exception>
        int GetIndexOf(int visit);

        /// <summary>
        /// Gets the visit at the given index.
        /// </summary>
        /// <param name="index">The position of the visit in the route, the first being at O.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">When the index is out of range.</exception>
        int GetVisitAt(int index);

        /// <summary>
        /// Returns an enumerable that enumerates between the two given customers.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns>En enumerable that enumerates customers between two given customers.</returns>
        /// <exception cref="System.ArgumentException">When from equals equals to.</exception>
        IEnumerable<int> Between(int from, int to);

        /// <summary>
        /// Returns an enumerator that iterates through the visit in this route starting at the given visit.
        /// </summary>
        /// <returns></returns>
        IEnumerator<int> GetEnumerator(int visit);

        /// <summary>
        /// Returns an enumerable that enumerates all visit pairs that occur in the route as 1->2. If the route is a tour the pair that contains last->first is also included.
        /// </summary>
        /// <returns>An enumerable that enumerates all visit pairs that occur in the route as 1->2. If the route is a tour the pair that contains last->first is also included.</returns>
        IEnumerable<Pair> Pairs();

        /// <summary>
        /// Returns an enumerable that enumerates all visit triples that occur in the route as 1->2->3. If the route is a tour the tuples that contain last->first are also included.
        /// </summary>
        /// <returns>An enumerable that enumerates all visit triples that occur in the route as 1->2->3. If the route is a tour the tuples that contain last->first are also included.</returns>
        IEnumerable<Triple> Triples();

        /// <summary>
        /// Removes all customers in this route except the first one.
        /// </summary>
        void Clear();
    }
}
