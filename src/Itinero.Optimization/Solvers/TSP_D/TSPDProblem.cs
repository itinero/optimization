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
using System.Runtime.CompilerServices;
using Itinero.Navigation.Directions;
using Itinero.Optimization.Solvers.Shared.Directed;
using Itinero.Optimization.Solvers.Shared.NearestNeighbours;

[assembly: InternalsVisibleTo("Itinero.Optimization.Tests")]
namespace Itinero.Optimization.Solvers.TSP_D
{
    /// <summary>
    /// The directed TSP.
    /// </summary>
    internal sealed class TSPDProblem
    {
        private readonly float[][] _weights;
        private readonly float[] _turnPenalties;
        private readonly Lazy<TSPDProblem> _closedEquivalent;
        private readonly bool _behaveAsClosed = false;
        private readonly int? _last;
        private readonly HashSet<int> _visits;
        
        private TSPDProblem(TSPDProblem other, bool behaveAsClosed)
        {
            _weights = other._weights;
            _visits = other._visits;
            this.First = other.First;
            _last = other._last;
            _turnPenalties = other._turnPenalties;
            
            _behaveAsClosed = behaveAsClosed;
        }
        
        /// <summary>
        /// Creates a new TSP.
        /// - An 'open' TSP, when last is null.
        /// - A 'closed' TSP, when last = first.
        /// - A 'fixed' TSP, all other cases, when first != last and last != null.
        /// </summary>
        /// <param name="first">The first visit.</param>
        /// <param name="last">The last visit if any.</param>
        /// <param name="weights">The weights matrix.</param>
        /// <param name="turnPenalty">The turn penalty.</param>
        /// <param name="visits">The possible visits, all visits are possible if null.</param>
        public TSPDProblem(int first, int? last, float[][] weights, float turnPenalty, IEnumerable<int> visits = null)
        {
            this.First = first;
            _last = last;
            _weights = weights;
            _turnPenalties = new float[] {
                0,
                turnPenalty,
                turnPenalty,
                0
            };
            
            _closedEquivalent = new Lazy<TSPDProblem>(() => 
                new TSPDProblem(this, true));

            if (visits != null)
            {
                if (visits is HashSet<int> visitsSet)
                {
                    _visits = visitsSet;
                }
                else
                {
                    _visits = new HashSet<int>(visits);
                }
            }
            
            if (!this.Contains(this.First)) { throw new ArgumentException("Not in the visits.", nameof(first)); }
            if (this.Last.HasValue && !this.Contains(this.Last.Value)) { throw new ArgumentException("Not in the visits.", nameof(last)); }
        }
        
        /// <summary>
        /// Gets the weight between the two given visits.
        /// </summary>
        public float Weight(int from, int to)
        {
            if (_behaveAsClosed)
            {
                if (!_last.HasValue && to == this.First)
                { // make sure all costs to 'first' are '0'.
                    return 0;
                }
                else if (_last.HasValue && _last != this.First)
                { // first and last are different.
                    // pretend '-> first' is '-> last' and remove last altogether.
                    if (to == this.First) to = _last.Value;

                    return _weights[from][to];
                }
            }
            return _weights[from][to];
        }

        /// <summary>
        /// Returns true if the visit is part of this problem.
        /// </summary>
        /// <param name="visit">The visit.</param>
        /// <returns>True if this visit is part of this problem.</returns>
        public bool Contains(int visit)
        {
            if (_behaveAsClosed &&
                _last.HasValue &&
                visit == _last)
            {
                return false;
            }
            if (_visits != null)
            {
                return _visits.Contains(visit);
            }
            return visit >= 0 && visit < _weights.Length;
        }

        /// <summary>
        /// Gets the visits.
        /// </summary>
        public IEnumerable<int> Visits
        {
            get
            {
                var visits = _visits ?? System.Linq.Enumerable.Range(0, _weights.Length / 2);
                if (_behaveAsClosed)
                {
                    foreach (var visit in visits)
                    {
                        if (visit == _last)
                        {
                            continue;
                        }

                        yield return visit;
                    }
                }
                else
                {
                    foreach (var visit in visits)
                    {
                        yield return visit;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the first.
        /// </summary>
        public int First { get; private set; }

        /// <summary>
        /// Gets the last.
        /// </summary>
        public int? Last
        {
            get
            {
                if (_behaveAsClosed)
                {
                    return this.First;
                }
                return _last;
            }
        }

        /// <summary>
        /// Gets or sets the turn penalties per type of turn.
        /// </summary>
        public float TurnPenalty(TurnEnum turn)
        {
            return _turnPenalties[(byte) turn];
        }


        /// <summary>
        /// Gets the number of visits.
        /// </summary>
        public int Count
        {
            get
            {
                var count = _weights.Length;
                if (_visits != null) count = _visits.Count;
                if (_behaveAsClosed &&
                    _last.HasValue)
                {
                    count--;
                }

                return count;
            }
        }

        /// <summary>
        /// Gets the number of weights.
        /// </summary>
        public int WeightsSize => _weights.Length;

        /// <summary>
        /// Gets the closed equivalent of this problem.
        /// </summary>
        internal TSPDProblem ClosedEquivalent
        {
            get
            {
                if (this.First != this.Last)
                {
                    return _closedEquivalent.Value;
                }

                return this;
            }
        }
    }
}