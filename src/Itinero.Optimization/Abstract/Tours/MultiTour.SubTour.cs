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

namespace Itinero.Optimization.Abstract.Tours
{
    /// <summary>
    /// An asymetric dynamically sizeable mutliple routes object.
    /// </summary>
    public partial class MultiTour
    {
        /// <summary>
        /// A class exposing only the information about one route.
        /// </summary>
        protected class SubTour : ITour
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
            /// Updates and sets the last visit.
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

            private int _count = -1;

            private void UpdateCount()
            {
                if (_count < 0)
                {
                    _count = (this as IEnumerable<int>).Count();
                }
            }

            /// <summary>
            /// Returns true if there exists an edge from the given visit to another.
            /// </summary>
            /// <param name="from"></param>
            /// <param name="to"></param>
            /// <returns></returns>
            public bool Contains(int from, int to)
            {
                // TODO: this was an exact copy/past from tour, investigate sharing this code.
                if (_parent._nextArray.Length > from)
                { // visits should exist.
                    if (_parent._nextArray[from] == to)
                    { // edge found.
                        return true;
                    }
                    else if (this.Contains(from) && (_parent._nextArray[from] == Constants.END))
                    { // the from visit is contained but it does not have a next visit.
                        if (this.First == this.Last)
                        {
                            return to == _first;
                        }
                    }
                }
                return false; // array too small.
            }

            /// <summary>
            /// Inserts a visit right after from and before to.
            /// </summary>
            /// <param name="from"></param>
            /// <param name="visit"></param>
            public void ReplaceEdgeFrom(int from, int visit)
            {
                // TODO: this was an exact copy/past from tour, investigate sharing this code.
                if (from < 0)
                { // a new visit cannot be negative!
                    throw new ArgumentOutOfRangeException("Cannot add a visit after a visit with a negative index!");
                }

                _count = -1;
                if (visit == _first)
                { // the next visit is actually the first visit.
                  // set the next visit of the from visit to -1.
                    visit = Constants.END;
                }

                if (_parent._nextArray.Length > from)
                { // visits should exist.
                  // resize the array if needed.
                    if (_parent._nextArray.Length <= visit)
                    { // resize the array.
                        this.Resize(visit);
                    }

                    // insert visit.
                    _parent._nextArray[from] = visit;
                    return;
                }
                throw new ArgumentOutOfRangeException("visit(s) do not exist in this route!");
            }

            /// <summary>
            /// Replaces the given old visit with the new visit. Assumes the new visit doesn't exist yet.
            /// </summary>
            public void Replace(int oldvisit, int newvisit)
            {
                // TODO: this was an exact copy/past from tour, investigate sharing this code.
                if (oldvisit == newvisit)
                {
                    return;
                }
                if (newvisit >= _parent._nextArray.Length)
                {
                    this.Resize(newvisit);
                }

                _parent._nextArray[newvisit] = _parent._nextArray[oldvisit];
                _parent._nextArray[oldvisit] = Constants.NOT_SET;

                if (oldvisit == _first)
                {
                    _first = newvisit;
                }

                if (oldvisit == _last)
                {
                    _last = newvisit;
                }

                for (var i = 0; i < _parent._nextArray.Length; i++)
                {
                    if (_parent._nextArray[i] == oldvisit)
                    {
                        _parent._nextArray[i] = newvisit;
                        break;
                    }
                }
            }

            /// <summary>
            /// Shifts the given visit to a new location and places it after the given 'before' visit.
            /// </summary>
            /// <param name="visit">The visit to shift.</param>
            /// <param name="before">The new visit that will come right before.</param>
            /// <returns></returns>
            public bool ShiftAfter(int visit, int before)
            {
                int oldBefore, oldAfter, newAfter;
                return this.ShiftAfter(visit, before, out oldBefore, out oldAfter, out newAfter);
            }

            /// <summary>
            /// Shifts the given visit to a new location and places it after the given 'before' visit.
            /// </summary>
            /// <param name="visit">The visit to shift.</param>
            /// <param name="before">The new visit that will come right before.</param>
            /// <param name="oldBefore">The visit that used to exist before.</param>
            /// <param name="oldAfter">The visit that used to exist after.</param>
            /// <param name="newAfter">The visit that new exists after.</param>
            public bool ShiftAfter(int visit, int before, out int oldBefore, out int oldAfter, out int newAfter)
            {
                if (visit == before) { throw new ArgumentException("Cannot shift a visit after itself."); }
                var searchFor = visit;
                if (visit == _first)
                { // search for END when visit to insert is the first visit.
                    searchFor = Constants.END;
                }
                for (int idx = 0; idx < _parent._nextArray.Length; idx++)
                { // search for the 'before'.
                    if (_parent._nextArray[idx] == searchFor)
                    {
                        oldBefore = idx;
                        oldAfter = _parent._nextArray[visit];
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
                        _parent._nextArray[visit] = newAfter;
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
            /// Inserts a visit right after from and before to.
            /// </summary>
            /// <param name="from"></param>
            /// <param name="visit"></param>
            public void InsertAfter(int from, int visit)
            {
                // TODO: this was an exact copy/past from tour, investigate sharing this code.
                if (visit < 0)
                { // a new visit cannot be negative!
                    throw new ArgumentOutOfRangeException("visit", "Cannot add visits with a negative id!");
                }
                if (from < 0)
                { // a new visit cannot be negative!
                    throw new ArgumentOutOfRangeException("from", "Cannot add a visit after a visit with a negative id!");
                }
                if (from == visit)
                { // the visit are identical.
                    throw new ArgumentException("Cannot add a visit after itself.");
                }

                _count = -1;
                // resize the array if needed.
                if (_parent._nextArray.Length <= from)
                { // array is not big enough.
                    this.Resize(from);
                }
                if (_parent._nextArray.Length <= visit)
                { // resize the array.
                    this.Resize(visit);
                }

                // get the to visit if needed.
                var to = _parent._nextArray[from];
                if (to == Constants.NOT_SET)
                { // the to field is not set.
                    throw new ArgumentOutOfRangeException("from", string.Format("visit {0} does not exist.", from));
                }

                // insert visit.
                _parent._nextArray[from] = visit;
                if (to == Constants.END)
                { // the to-visit is END.
                    if (this.First != this.Last)
                    { // update last.
                        _internalLast = visit;
                    }
                }

                // update the next for visit.
                _parent._nextArray[visit] = to;

                return;
            }

            /// <summary>
            /// Resizes the array.
            /// </summary>
            /// <param name="visit"></param>
            private void Resize(int visit)
            { // THIS EXPENSIZE! TRY TO ESTIMATE CORRECT SIZE WHEN CREATING ROUTE!
                // TODO: this was an exact copy/past from tour, investigate sharing this code.
                int old_size = _parent._nextArray.Length;
                Array.Resize<int>(ref _parent._nextArray, visit + 1);
                for (int new_visit = old_size; new_visit < _parent._nextArray.Length; new_visit++)
                { // initialize with -1.
                    _parent._nextArray[new_visit] = -1;
                }
            }

            /// <summary>
            /// Cuts out a part of the route and returns the visits contained.
            /// </summary>
            /// <param name="start"></param>
            /// <param name="length"></param>
            /// <returns></returns>
            public int[] CutAndRemove(int start, int length)
            {
                // TODO: this was an exact copy/past from tour, investigate sharing this code.
                var cutPart = new int[length];
                var position = 0;
                var currentvisit = this.First;

                // keep moving next until the start.
                while (position < start - 1)
                {
                    position++; // increase the position.
                    currentvisit = _parent._nextArray[currentvisit];
                }

                // cut the actual part.
                int startvisit = currentvisit;
                while (position < start + length - 1)
                {
                    // move next.
                    position++; // increase the position.
                    currentvisit = _parent._nextArray[currentvisit];

                    // set the current visit.
                    cutPart[position - start] = currentvisit;
                }

                currentvisit = _parent._nextArray[currentvisit];

                // set the next visit.
                _parent._nextArray[startvisit] = currentvisit;

                _count -= length;
                return cutPart;
            }
            
            /// <summary>
            /// Returns the neigbour of the given visit.
            /// </summary>
            /// <param name="visit"></param>
            /// <returns></returns>
            public int GetNeigbour(int visit)
            {
                // TODO: this was an exact copy/past from tour, investigate sharing this code.
                var neighbour = _parent._nextArray[visit];
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

            /// <summary>
            /// Gets the enumerator.
            /// </summary>
            public IEnumerator<int> GetEnumerator()
            {
                return new Enumerator(_first, _parent._nextArray);
            }

            /// <summary>
            /// Gets the enumerator.
            /// </summary>
            public IEnumerator<int> GetEnumerator(int visit)
            {
                return new Enumerator(_first, visit, _parent._nextArray);
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

            /// <summary>
            /// Enumerates everything between the two given visits.
            /// </summary>
            /// <param name="from">The first visit.</param>
            /// <param name="to">The last visit.</param>
            /// <returns></returns>
            public IEnumerable<int> Between(int from, int to)
            {
                return new Tour.TourBetweenEnumerable(_parent._nextArray, _first, _last, from, to);
            }

            #endregion
            
            /// <summary>
            /// Returns the # of visits in this tour.
            /// </summary>
            /// <returns></returns>
            public int Count
            {
                get
                {
                    // TODO: this was an exact copy/past from tour, investigate sharing this code.
                    this.UpdateCount();
                    return _count;
                }
            }
            
            /// <summary>
            /// Removes the given visit.
            /// </summary>
            /// <param name="visit">The visit to remove.</param>
            /// <returns></returns>
            public bool Remove(int visit)
            {
                // TODO: this was an exact copy/past from tour, investigate sharing this code.
                if (visit == _first)
                { // cannot remove the first visit.
                    throw new InvalidOperationException("Cannot remove first visit from a route.");
                }
                if (visit == _last)
                { // cannot remove the first visit.
                    throw new InvalidOperationException("Cannot remove last visit from a route.");
                }
                for (int idx = 0; idx < _parent._nextArray.Length; idx++)
                {
                    if (_parent._nextArray[idx] == visit)
                    {
                        _parent._nextArray[idx] = _parent._nextArray[visit];
                        _parent._nextArray[visit] = Constants.NOT_SET;
                        if (visit == _internalLast && this.First != this.Last)
                        { // update last if open problem.
                            _internalLast = idx;
                        }
                        _count--;
                        return true;
                    }
                }
                return false;
            }
            
            /// <summary>
            /// Removes the given visit.
            /// </summary>
            /// <param name="visit">The visit to remove.</param>
            /// <param name="before">The visit right before the removed visit.</param>
            /// <param name="after">The visit right after the removed visit.</param>
            /// <returns></returns>
            public bool Remove(int visit, out int before, out int after)
            {
                // TODO: this was an exact copy/past from tour, investigate sharing this code.
                if (visit == _first)
                { // cannot remove the first visit.
                    throw new InvalidOperationException("Cannot remove first visit from a route.");
                }
                for (int idx = 0; idx < _parent._nextArray.Length; idx++)
                { // search for the 'before'.
                    if (_parent._nextArray[idx] == visit)
                    {
                        before = idx;
                        after = _parent._nextArray[visit];

                        _parent._nextArray[idx] = _parent._nextArray[visit];
                        _parent._nextArray[visit] = Constants.NOT_SET;
                        _count--;
                        return true;
                    }
                }
                before = Constants.NOT_SET;
                after = Constants.NOT_SET;
                return false;
            }

            /// <summary>
            /// The first visit in this tour.
            /// </summary>
            /// <returns></returns>
            public int First
            {
                get
                {
                    return _first;
                }
            }
            
            /// <summary>
            /// The last visit in this tour.
            /// </summary>
            /// <returns></returns>
            public int? Last
            {
                get
                {
                    if (_last < 0)
                    {
                        // calculate the last visit.
                        this.UpdateLast();
                    }
                    return _last;
                }
            }

            /// <summary>
            /// Gets the visit at the given index.
            /// </summary>
            /// <param name="index">The index.</param>
            /// <returns></returns>
            public int GetVisitAt(int index)
            {
                if (index < 0) { new ArgumentOutOfRangeException("No visit can ever exist at an index smaller than 0."); }

                var idx = 0;
                foreach (var possiblevisit in this)
                {
                    if (idx == index)
                    {
                        return possiblevisit;
                    }
                    idx++;
                }
                throw new ArgumentOutOfRangeException(string.Format("No visit found at index {0}.", index));
            }

            /// <summary>
            /// Gets the index of the given visit.
            /// </summary>
            /// <param name="visit">The visit.</param>
            /// <returns></returns>
            public int GetIndexOf(int visit)
            {
                // TODO: this was an exact copy/paste from tour, investigate sharing this code.
                var idx = 0;
                foreach (var possiblevisit in this)
                {
                    if (possiblevisit == visit)
                    {
                        return idx;
                    }
                    idx++;
                }
                return Constants.NOT_SET;
            }

            /// <summary>
            /// Returns true if the given visits exists in this tour.
            /// </summary>
            /// <param name="visit">The visit.</param>
            /// <returns></returns>
            public bool Contains(int visit)
            {
                // TODO: this was an exact copy/past from tour, investigate sharing this code.
                if (_parent._nextArray.Length > visit)
                {
                    if (_parent._nextArray[visit] >= 0)
                    { // visit is definetly contained.
                        return true;
                    }
                    return _parent._nextArray.Contains<int>(visit);
                }
                return false;
            }
            
            /// <summary>
            /// Returns a human-readable description of this tour.
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                // TODO: this was an exact copy/past from tour, investigate sharing this code.
                int previous = -1;
                var result = new StringBuilder();
                foreach (int visit in this)
                {
                    if (previous < 0)
                    {
                        result.Append('[');
                        result.Append(visit);
                        result.Append(']');
                    }
                    else if (visit != this.Last)
                    {
                        result.Append("->");
                        result.Append(visit);
                    }
                    previous = visit;
                }

                if (this.Last.HasValue && this.Count > 1)
                {
                    result.Append("->[");
                    result.Append(this.Last.Value);
                    result.Append("]");
                }
                return result.ToString();
            }

            /// <summary>
            /// Makes a deep-copy of this tour.
            /// </summary>
            /// <returns></returns>
            public object Clone()
            {
                throw new NotSupportedException("Cannot clone a route that's part of a multi-route.");
            }
            
            //public int Next(int visit)
            //{
            //    return _parent._nextArray[visit];
            //}

            /// <summary>
            /// Returns an enumerable that enumerates all visit pairs that occur in the route as 1->2. If the route is a round the pair that contains last->first is also included.
            /// </summary>
            /// <returns></returns>
            public IEnumerable<Pair> Pairs()
            {
                return new PairEnumerable<SubTour>(this, _first == _last);
            }

            /// <summary>
            /// Returns an enumerable that enumerates all visit triples that occur in the route as 1->2-3. If the route is a round the tuples that contain last->first are also included.
            /// </summary>
            /// <returns></returns>
            public IEnumerable<Triple> Triples(bool wrapAround = true)
            {
                return new TripleEnumerable(this, wrapAround);
            }

            /// <summary>
            /// Clears out all the visits in this tour.
            /// </summary>
            public void Clear()
            {
                _parent._nextArray[_first] = Constants.END;
                _internalLast = _first;
                _count = -1;
            }

            /// <summary>
            /// Copies data from the given solution.
            /// </summary>
            /// <param name="solution">The solution.</param>
            public void CopyFrom(ISolution solution)
            { // TODO: investigate how this is used, it may be a solution in a subtour optimization.
                throw new Exception("Auwch, abstraction bleed, a subtour is not a solution.");
            }
        }
    }
}