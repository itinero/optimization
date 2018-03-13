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
using Itinero.Optimization.Algorithms;

namespace Itinero.Optimization.Abstract.Tours.Typed
{
    /// <summary>
    /// A typed multi tour.
    /// </summary>
    public partial class MultiTour<T> : IMultiTour<T>
        where T : struct
    {
        private readonly MultiTour _multiTour;
        private readonly Func<T, int> _getId;
        private readonly Func<int, T> _getVisit;

        /// <summary>
        /// Creates a new multi tour.
        /// </summary>
        public MultiTour(Func<T, int> getId, Func<int, T> getVisit, MultiTour multiTour)
        {
            _multiTour = multiTour;
            _getId = getId;
            _getVisit = getVisit;
        }

        /// <summary>
        /// Creates a new multi tour.
        /// </summary>
        public MultiTour(Func<T, int> getId, Func<int, T> getVisit, int size)
            : this(getId, getVisit, new MultiTour(size))
        {
            
        }

        /// <summary>
        /// Returns the number of subtours.
        /// </summary>
        public int Count => _multiTour.Count;

        /// <summary>
        /// Returns the size.
        /// </summary>
        public int Size => _multiTour.Size;

        /// <summary>
        /// Adds a new subtour.
        /// </summary>        
        public ITour<T> Add(T first, T? last)
        {
            if (last.HasValue)
            {
                return new Tour<T>(_getId, _getVisit, _multiTour.Add(_getId(first), _getId(last.Value)));
            }
            else
            {
                return new Tour<T>(_getId, _getVisit, _multiTour.Add(_getId(first), null));
            }
        }

        /// <summary>
        /// Adds a new subtour by copying the given one.
        /// </summary>
        /// <param name="tour"></param>
        public void Add(ITour<T> tour)
        {
            _multiTour.Add(tour.RawTour);
        }

        /// <summary>
        /// Returns a deep-copy of this multi tour.
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Returns true if there is an edge in one of the subtours from from to to.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns>True if there is an edge from->to in one of the subtours.</returns>
        /// <exception cref="System.ArgumentException">When from equals to.</exception>
        public bool Contains(T from, T to)
        {
            var fromId = _getId(from);
            var toId = _getId(to);

            return _multiTour.Contains(fromId, toId);
        }

        /// <summary>
        /// Returns true if the given customer is in this route.
        /// </summary>
        /// <param name="visit"></param>
        /// <returns>True if the customer occurs in this route.</returns>
        public bool ContainsVisit(T visit)
        {
            var visitId = _getId(visit);

            return _multiTour.ContainsVisit(visitId);
        }

        /// <summary>
        /// Copies from the given solution.
        /// </summary>
        public void CopyFrom(ISolution solution)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Removes the route at the given index.
        /// </summary>
        /// <param name="tourIdx"></param>
        /// <returns></returns>
        public bool Remove(int tourIdx)
        {
            return _multiTour.Remove(tourIdx);
        }

        /// <summary>
        /// Removes a visit from the tour.
        /// </summary>
        /// <param name="visit"></param>
        /// <returns>Return true if the visit was found and removed.</returns>
        /// <exception cref="System.InvalidOperationException">When attempting to remove the first customer.</exception>
        public bool RemoveVisit(T visit)
        {
            var visitId = _getId(visit);

            return _multiTour.RemoveVisit(visitId);
        }

        /// <summary>
        /// Gets the tour at the given index.
        /// </summary>
        /// <param name="tourIdx"></param>
        /// <returns></returns>
        public ITour<T> Tour(int tourIdx)
        {
            return new Tour<T>(_getId, _getVisit, _multiTour.Tour(tourIdx));
        }
    }
}