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
using System.Diagnostics;
using System.Linq;
using System.Text;
using Itinero.Optimization.Strategies;

namespace Itinero.Optimization.Solvers.Tours
{
    /// <summary>
    /// a tour or a sequence of visits.
    /// </summary>
    public sealed class Tour : ICloneable<Tour>, IReadOnlyTour
    {
        private int[] _nextArray;
        private int _first;
        private int? _last;
        
        /// <summary>
        /// Holds the default not-set value for a customer entry.
        /// </summary>
        public const int NOT_SET = -1;

        /// <summary>
        /// Holds the default value indicating the end of a route.
        /// </summary>
        public const int END = -2;

        /// <summary>
        /// Creates a new tour based on the given array.
        /// </summary>
        private Tour(int first, int[] nextArray, int? last)
        {
            _first = first;
            _last = last;
            _nextArray = nextArray;

            this.UpdateLast();
        }

        /// <summary>
        /// Creates a new closed tour using a preexisting sequence.
        /// </summary>
        public Tour(IEnumerable<int> visits) : this(visits, visits.First())
        {

        }

        /// <summary>
        /// Creates a new tour using a preexisting sequence.
        /// </summary>
        public Tour(IEnumerable<int> visits, int? last)
        {
            _nextArray = new int[0];
            var first = -1;
            var previous = -1;

            var lastOk = (!last.HasValue);
            foreach (var visit in visits)
            {
                // resize the array if needed.
                if (_nextArray.Length <= visit)
                {
                    this.Resize(visit);
                }

                // the first visit.
                if (first < 0)
                { // set the first visit.
                    first = visit;
                }
                else
                { // set the next array.
                    _nextArray[previous] = visit;
                }

                if (!lastOk)
                {
                    lastOk = visit == last;
                }

                previous = visit;
            }

            if (!lastOk)
            {
                throw new ArgumentException("There is a fixed last visit defined but it's not in the initial visits set.");
            }

            _nextArray[previous] = END;

            // set actual tour-data.
            _first = first;
            _internalLast = previous;
            _last = last;
        }

        /// <summary>
        /// Returns true if there exists an edge from the given visit to another.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public bool Contains(int from, int to)
        {
            if (_nextArray.Length > from)
            { // visits should exist.
                if (_nextArray[from] == to)
                { // edge found.
                    return true;
                }
                else if (this.Contains(from) && _nextArray[from] == END)
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
        /// Returns true if the visit exists in this tour.
        /// </summary>
        /// <param name="visit"></param>
        /// <returns></returns>
        public bool Contains(int visit)
        {
            if (_nextArray.Length > visit)
            {
                if (_nextArray[visit] >= 0)
                { // visit is definetly contained.
                    return true;
                }
                return _nextArray.Contains<int>(visit);
            }
            return false;
        }

        /// <summary>
        /// Inserts a visit right after from and before to.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="visit"></param>
        public void ReplaceEdgeFrom(int from, int visit)
        {
            if (from < 0)
            { // a new visit cannot be negative!
                throw new ArgumentOutOfRangeException(nameof(from), "Cannot add a visit after a visit with a negative index!");
            }

            _count = -1;
            if (visit == _first)
            { // the next visit is actually the first visit.
                // set the next visit of the from visit to -1.
                visit = END;
            }

            if (_nextArray.Length <= @from)
            {
                throw new ArgumentOutOfRangeException(nameof(@from),
                    "visit(s) do not exist in this tour!"); // visits should exist.
            }

            // resize the array if needed.
            if (_nextArray.Length <= visit)
            { // resize the array.
                this.Resize(visit);
            }

            // insert visit.
            _nextArray[@from] = visit;
            return;
        }

        /// <summary>
        /// Replaces the given old visit with the new visit. Assumes the new visit doesn't exist yet.
        /// </summary>
        public void Replace(int oldvisit, int newvisit)
        { // TODO: this can be implemented in O(1) when we restructure the tour data structure.
            if (oldvisit == newvisit)
            {
                return;
            }
            if (newvisit >= _nextArray.Length)
            {
                this.Resize(newvisit);
            }

            if (oldvisit < _nextArray.Length)
            {
                _nextArray[newvisit] = _nextArray[oldvisit];
                _nextArray[oldvisit] = NOT_SET;
            }

            if (oldvisit == _first)
            {
                _first = newvisit;
            }

            if (oldvisit == _last)
            {
                _last = newvisit;
            }

            for (var i = 0; i < _nextArray.Length; i++)
            {
                if (_nextArray[i] != oldvisit) continue;
                _nextArray[i] = newvisit;
                break;
            }
        }

        /// <summary>
        /// Inserts a visit right after from and before to.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="visit"></param>
        public void InsertAfter(int from, int visit)
        {
            if (visit < 0)
            { // a new visit cannot be negative!
                throw new ArgumentOutOfRangeException(nameof(visit), "Cannot add visits with a negative id!");
            }
            if (from < 0)
            { // a new visit cannot be negative!
                throw new ArgumentOutOfRangeException(nameof(@from), "Cannot add a visit after a visit with a negative id!");
            }
            if (from == visit)
            { // the visit are identical.
                throw new ArgumentException($"Cannot add a visit after itself. You tried to add visit {visit} after itself");
            }

            _count = -1;
            // resize the array if needed.
            if (_nextArray.Length <= from)
            { // array is not big enough.
                this.Resize(from);
            }
            if (_nextArray.Length <= visit)
            { // resize the array.
                this.Resize(visit);
            }

            // get the to visit if needed.
            var to = _nextArray[from];
            if (to == NOT_SET)
            { // the to field is not set.
                throw new ArgumentOutOfRangeException(nameof(@from), $"Visit {from} does not exist.");
            }

            // insert visit.
            _nextArray[from] = visit;
            if (to == END)
            { // the to-visit is END.
                if (this.First != this.Last)
                { // update last.
                    _internalLast = visit;
                }
            }

            // update the next for visit.
            _nextArray[visit] = to;

            return;
        }

        /// <summary>
        /// Resizes the array.
        /// </summary>
        /// <param name="visit"></param>
        private void Resize(int visit)
        { // THIS EXPENSIZE! TRY TO ESTIMATE CORRECT SIZE WHEN CREATING tour!
            int old_size = _nextArray.Length;
            Array.Resize<int>(ref _nextArray, visit + 1);
            for (int newvisit = old_size; newvisit < _nextArray.Length; newvisit++)
            { // initialize with NOT_SET.
                _nextArray[newvisit] = NOT_SET;
            }
        }

        /// <summary>
        /// Cuts out a part of the tour and returns the visits contained.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public int[] CutAndRemove(int start, int length)
        {
            var cutPart = new int[length];
            var position = 0;
            var currentvisit = this.First;

            // keep moving next until the start.
            while (position < start - 1)
            {
                position++; // increase the position.
                currentvisit = _nextArray[currentvisit];
            }

            // cut the actual part.
            int startvisit = currentvisit;
            while (position < start + length - 1)
            {
                // move next.
                position++; // increase the position.
                currentvisit = _nextArray[currentvisit];

                // set the current visit.
                cutPart[position - start] = currentvisit;
            }

            currentvisit = _nextArray[currentvisit];

            // set the next visit.
            _nextArray[startvisit] = currentvisit;

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
            var neighbour = _nextArray[visit];
            if (neighbour < 0)
            {
                if (this.First == this.Last)
                {
                    neighbour = this.First;
                    return neighbour;
                }
                return NOT_SET;
            }
            return neighbour;
        }

        /// <summary>
        /// Returns the neigbour of the given visit.
        /// </summary>
        /// <param name="visit"></param>
        /// <returns></returns>
        public int Next(int visit)
        {
            return _nextArray[visit];
        }

        /// <summary>
        /// Creates an exact deep-copy of this tour.
        /// </summary>
        /// <returns></returns>
        public Tour Clone()
        {
            return new Tour(_first, _nextArray.Clone() as int[], _last);
        }

        #region Enumerators

        /// <summary>
        /// Returns the enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<int> GetEnumerator()
        {
            return new Enumerator(_first, _nextArray);
        }

        /// <summary>
        /// Returns the enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<int> GetEnumerator(int visit)
        {
            return new Enumerator(_first, visit, _nextArray);
        }

        /// <summary>
        /// Returns the enumerator.
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new Enumerator(_first, _nextArray);
        }

        private class Enumerator : IEnumerator<int>
        {
            private readonly int _first;
            private readonly int _start;
            private int[] _nextArray;
            private int _count = -1;

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

            public int Current => _current;

            public void Dispose()
            {
                _nextArray = null;
            }

            object System.Collections.IEnumerator.Current => this.Current;

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
                    if (_current == END)
                    {
                        return false;
                    }
                }

                // TODO: should we leave this in? it prevents infinite enumerations and detects invalid tours.
                _count++;
                if (_count > this._nextArray.Length + 1)
                {
                    throw new Exception($"Found at least {_count} element when there is only room for {this._nextArray.Length + 1}.");
                }
                return _current >= 0;
            }

            public void Reset()
            {
                _count = -1;
                _current = -1;
            }
        }

        /// <summary>
        /// Returns an enumerable between.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public IEnumerable<int> Between(int from, int to)
        {
            return new TourBetweenEnumerable(_nextArray, _first, _last, from, to);
        }

        /// <summary>
        /// Returns an enumerable that enumerates all visit pairs that occur in the tour as 1->2. If the tour is a round the pair that contains last->first is also included.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Pair> Pairs()
        {
            return new PairEnumerable<Tour>(this, _first == _last);
        }

        /// <summary>
        /// Returns an enumerable that enumerates all visit triples that occur in the tour as 1->2-3. If the tour is a round the tuples that contain last->first are also included.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Triple> Triples(bool wrapAround = true)
        {
            return new TripleEnumerable(this, wrapAround);
        }

        #endregion

        /// <summary>
        /// Returns the size of the tour.
        /// </summary>
        public int Count
        {
            get
            {
                this.UpdateCount();
                return _count;
            }
        }

        /// <summary>
        /// Gets the current capacity.
        /// </summary>
        public int Capacity => _nextArray.Length;

        /// <summary>
        /// Removes a visit from the tour.
        /// </summary>
        /// <param name="visit"></param>
        /// <returns></returns>
        public bool Remove(int visit)
        {
            if (visit == _first)
            { // cannot remove the first visit.
                throw new InvalidOperationException("Cannot remove first visit from a tour.");
            }
            if (visit == _last)
            { // cannot remove the first visit.
                throw new InvalidOperationException("Cannot remove last visit from a tour.");
            }
            for (var idx = 0; idx < _nextArray.Length; idx++)
            {
                if (_nextArray[idx] != visit) continue;
                _nextArray[idx] = _nextArray[visit];
                _nextArray[visit] = NOT_SET;
                if (visit == _internalLast && this.First != this.Last)
                { // update last if open problem.
                    _internalLast = idx;
                }
                _count--;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes a visit from the tour.
        /// </summary>
        /// <param name="visit">The visit to remove.</param>
        /// <param name="after">The visit that used to exist after.</param>
        /// <param name="before">The visit that used to exist before.</param>
        /// <returns></returns>
        public bool Remove(int visit, out int before, out int after)
        {
            if (visit == _first)
            { // cannot remove the first visit.
                throw new InvalidOperationException("Cannot remove first visit from a tour.");
            }
            for (var idx = 0; idx < _nextArray.Length; idx++)
            { // search for the 'before'.
                if (_nextArray[idx] != visit) continue;
                before = idx;
                after = _nextArray[visit];

                _nextArray[idx] = _nextArray[visit];
                _nextArray[visit] = NOT_SET;
                _count--;
                return true;
            }
            before = NOT_SET;
            after = NOT_SET;
            return false;
        }

        /// <summary>
        /// Shifts the given visit to a new location and places it after the given 'before' visit.
        /// </summary>
        /// <param name="visit">The visit to shift.</param>
        /// <param name="before">The new visit that will come right before.</param>
        /// <returns></returns>
        public bool ShiftAfter(int visit, int before)
        {
            return this.ShiftAfter(visit, before, out _, out _, out _);
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
                searchFor = END;
            }
            for (var idx = 0; idx < _nextArray.Length; idx++)
            { // search for the 'before'.
                if (_nextArray[idx] != searchFor) continue;
                oldBefore = idx;
                oldAfter = _nextArray[visit];
                if (oldBefore == before)
                { // nothing to do here!
                    if (this.First == this.Last && oldAfter == END)
                    { // is closed and oldAfter is END then oldAfter is first.
                        oldAfter = this.First;
                    }

                    newAfter = oldAfter;
                    return true;
                }
                newAfter = _nextArray[before];

                // reorganize tour.
                _nextArray[before] = searchFor;
                _nextArray[visit] = newAfter;
                _nextArray[oldBefore] = oldAfter;

                if (this.First == this.Last && oldAfter == END)
                { // is closed and oldAfter is END then oldAfter is first.
                    oldAfter = this.First;
                }
                if (this.First == this.Last && newAfter == END)
                { // is closed and newAfter is END then newAfter is first.
                    newAfter = this.First;
                }
                return true;
            }
            oldBefore = NOT_SET;
            oldAfter = NOT_SET;
            newAfter = NOT_SET;
            return false;
        }

        /// <summary>
        /// Returns true if the given visit is the first one.
        /// </summary>
        public bool IsFirst(int visit)
        {
            if (this.First == this.Last)
            { // no visit is first, this tour is a loop.
                return false;
            }
            return this.First == visit;
        }

        /// <summary>
        /// Returns true if the given visit is the last one.
        /// </summary>
        public bool IsLast(int visit)
        {
            if (this.First == this.Last)
            { // no visit is last, this tour is a loop.
                return false;
            }
            return _nextArray[visit] == END;
        }

        /// <summary>
        /// Returns the first visit in this tour.
        /// </summary>
        public int First => _first;

        /// <summary>
        /// Returns the last visit in this tour.
        /// </summary>
        public int? Last => _last;

        /// <summary>
        /// Returns the index of the given visit.
        /// </summary>
        /// <param name="visit"></param>
        /// <returns></returns>
        public int GetIndexOf(int visit)
        {
            var idx = 0;
            foreach (var possiblevisit in this)
            {
                if (possiblevisit == visit)
                {
                    return idx;
                }
                idx++;
            }
            return NOT_SET;
        }

        /// <summary>
        /// Gets the visit at the given index.
        /// </summary>
        /// <param name="index">The position of the visit in the tour, the first being at O.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">When the index is out of range.</exception>
        public int GetVisitAt(int index)
        {
            if (index < 0) { throw new ArgumentOutOfRangeException(nameof(index), 
                "No visit can ever exist at an index smaller than 0."); }

            var idx = 0;
            foreach (var possiblevisit in this)
            {
                if (idx == index)
                {
                    return possiblevisit;
                }
                idx++;
            }
            throw new ArgumentOutOfRangeException($"No visit found at index {index}.");
        }

        private int _internalLast;

        /// <summary>
        /// Updates and sets the last visit.
        /// </summary>
        private void UpdateLast()
        {
            _internalLast = _first;
            if (this.First == this.Last) return;
            while (_nextArray[_internalLast] >= 0 && _nextArray[_internalLast] != _first)
            {
                _internalLast = _nextArray[_internalLast];
            }
        }

        private int _count = -1;

        private void UpdateCount()
        {
            if (_count < 0)
            { // WARNING: this can be called from multiple threads at the same time, make sure to only write to '_count' once.
                var count = 0;
                foreach (var i in (this as IEnumerable<int>))
                {
                    count++;
                    if (count > this._nextArray.Length + 1)
                    {
                        throw new Exception($"Found at least {_count} element when there is only room for {this._nextArray.Length + 1}.");
                    }
                }
                _count = count;
            }
        }

        /// <summary>
        /// Returns a description of this tour.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var previous = -1;
            var result = new StringBuilder();
            var c = 0;
            foreach (var visit in this)
            {
                c++;
                if (c > this._nextArray.Length + 1)
                {
                    return "INVALID";
                }
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

            if (!this.Last.HasValue || this.Count <= 1) return result.ToString();
            result.Append("->[");
            result.Append(this.Last.Value);
            result.Append("]");
            return result.ToString();
        }

        /// <summary>
        /// Clears out all the visits in this tour.
        /// </summary>
        public void Clear()
        {
            // TODO: why explicity clear everything? also why resize, should be explicit.
            //       see multitour.subtour.
            _nextArray = new int[_first + 1];
            for (var idx = 0; idx < _nextArray.Length; idx++)
            {
                _nextArray[idx] = NOT_SET;
            }
            _nextArray[_first] = END;
            _internalLast = _first;
            _count = -1;
        }

        /// <summary>
        /// An enumerable to enumerate visits between two given visits.
        /// </summary>
        internal class TourBetweenEnumerable : IEnumerable<int>
        {
            private readonly int _from;
            private readonly int _to;
            private readonly int _first;
            private readonly int? _last;
            private readonly int[] _nextArray;

            /// <summary>
            /// Creates a new between enumerable.
            /// </summary>
            public TourBetweenEnumerable(int[] nextArray, int first, int? last, int from, int to)
            {
                _nextArray = nextArray;
                _first = first;
                _last = last;
                _from = from;
                _to = to;
            }

            private class BetweenEnumerator : IEnumerator<int>
            {
                private int _current = -1;
                private readonly int _from;
                private readonly int _to;
                private readonly int _first;
                private readonly int? _last;
                private readonly int[] _nextArray;

                public BetweenEnumerator(int[] nextArray, int from, int to, int first, int? last)
                {
                    _nextArray = nextArray;
                    _first = first;
                    _last = last;
                    _from = from;
                    _to = to;
                }

                public int Current => _current;

                public void Dispose()
                {

                }

                object System.Collections.IEnumerator.Current => this.Current;

                public bool MoveNext()
                {
                    if (_first != _last && _from > _to)
                    {
                        return false;
                    }
                    if (_current == _to)
                    {
                        return false;
                    }
                    if (_current == END)
                    {
                        _current = _from;
                        return true;
                    }
                    if (_current == -1)
                    {
                        _current = _from;
                        return true;
                    }
                    _current = _nextArray[_current];
                    if (_current == END)
                    {
                        _current = _first;
                    }
                    return true;
                }

                public void Reset()
                {
                    _current = -1;
                }
            }

            /// <summary>
            /// Returns the enumerator.
            /// </summary>
            /// <returns></returns>
            public IEnumerator<int> GetEnumerator()
            {
                return new BetweenEnumerator(_nextArray, _from, _to, _first, _last);
            }

            /// <summary>
            /// Returns the enumerator.
            /// </summary>
            /// <returns></returns>
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }
    }
}