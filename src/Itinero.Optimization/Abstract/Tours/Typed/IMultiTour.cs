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

namespace Itinero.Optimization.Tours.Typed
{
    /// <summary>
    /// An abstract definition of a tour with multiple subtours.
    /// </summary>
    public interface IMultiTour<T> : ISolution
        where T : struct
    {
        /// <summary>
        /// Returns one of the sub tours.
        /// </summary>
        /// <param name="tourIdx"></param>
        /// <returns></returns>
        ITour<T> Tour(int tourIdx);

        /// <summary>
        /// Adds a new subtour.
        /// </summary>
        ITour<T> Add(T first, T? last);

        /// <summary>
        /// Adds a new subtour by copying the given one.
        /// </summary>
        /// <param name="tour"></param>
        void Add(ITour<T> tour);

        /// <summary>
        /// Removes the route at the given index.
        /// </summary>
        /// <param name="tourIdx"></param>
        /// <returns></returns>
        bool Remove(int tourIdx);

        /// <summary>
        /// Returns the number of subtours.
        /// </summary>
        int Count
        {
            get;
        }

        /// <summary>
        /// Returns the size.
        /// </summary>
        int Size
        {
            get;
        }

        /// <summary>
        /// Returns true if there is an edge in one of the subtours from from to to.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns>True if there is an edge from->to in one of the subtours.</returns>
        /// <exception cref="System.ArgumentException">When from equals to.</exception>
        bool Contains(T from, T to);

        /// <summary>
        /// Returns true if the given customer is in this route.
        /// </summary>
        /// <param name="visit"></param>
        /// <returns>True if the customer occurs in this route.</returns>
        bool ContainsVisit(T visit);

        /// <summary>
        /// Removes a visit from the tour.
        /// </summary>
        /// <param name="visit"></param>
        /// <returns>Return true if the visit was found and removed.</returns>
        /// <exception cref="System.InvalidOperationException">When attempting to remove the first customer.</exception>
        bool RemoveVisit(T visit);
    }
}