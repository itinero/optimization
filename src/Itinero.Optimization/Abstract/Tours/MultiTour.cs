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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Itinero.Optimization.Algorithms;

namespace Itinero.Optimization.Tours
{
    /// <summary>
    /// An asymetric dynamically sizeable mutliple routes object.
    /// </summary>
    public partial class MultiTour : IMultiTour, IEquatable<MultiTour>
    {
        /// <summary>
        /// The next-array.
        /// </summary>
        protected int[] _nextArray;
        /// <summary>
        /// The subtours.
        /// </summary>
        protected SubTour[] _subtours;

        /// <summary>
        /// Creates a new dynamic route by creating shallow copy of the array(s) given.
        /// </summary>
        /// <param name="first">The first visits.</param>
        /// <param name="last">The last visits.</param>
        /// <param name="nextArray">The next-array.</param>
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
        protected MultiTour(IEnumerable<SubTour> first, int[] nextArray)
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
        /// Creates a new dynamic assymmetric route using an initial size and visit.
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

            if (last.HasValue)
            { // make sure the last is marked as last.
                _nextArray[last.Value] = Constants.END;
            }

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
            int routeIdx = -1;
            foreach (int visit in tour)
            {
                // resize the array if needed.
                if (_nextArray.Length <= visit)
                { // resize the array.
                    this.Resize(visit);
                }

                if (first)
                {
                    // add one element to the first array.
                    routeIdx = _subtours.Length;
                    Array.Resize<SubTour>(ref _subtours, _subtours.Length + 1);

                    // set the initial visit.
                    _subtours[routeIdx] = new SubTour(this, tour.First, tour.Last);
                    _nextArray[visit] = Constants.NOT_SET;
                    first = false;
                }
                else
                {
                    _nextArray[previous] = visit;
                }

                // set the previous visit.
                previous = visit;
            }

            if (previous != -1)
            {
                _nextArray[previous] = Constants.END;
            }

            // return the new route.
            if (routeIdx < 0)
            {
                return null;
            }

            _sizes = null;
            return this.Tour(routeIdx);
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
            for (int new_visit = old_size; new_visit < _nextArray.Length; new_visit++)
            { // initialize with -1.
                _nextArray[new_visit] = -1;
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
        /// Returns the visit after the given visit.
        /// </summary>
        public int Next(int visit)
        {
            int next = _nextArray[visit];
            if (next < 0)
            {
                for (int idx = 0; idx < this.Count; idx++)
                {
                    var tour = this.Tour(idx);
                    if (tour.Contains(visit) &&
                        tour.GetNeigbour(visit) == tour.First)
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

                // compare the initial visits.
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
        /// Removes the given visit.
        /// </summary>
        /// <param name="visit"></param>
        /// <returns></returns>
        public bool RemoveVisit(int visit)
        {
            foreach (var part in _subtours)
            {
                if (part.Remove(visit))
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

        /// <summary>
        /// Copies data from the given solution.
        /// </summary>
        /// <param name="solution">The solution.</param>
        public void CopyFrom(ISolution solution)
        {
            throw new NotImplementedException();
        }
    }
}
