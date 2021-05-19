using System;
using System.Collections.Generic;
using System.Linq;
using Itinero.Algorithms.Weights;
using Itinero.Optimization.Solvers.Shared;
using Itinero.Optimization.Solvers.Shared.Directed;
using Itinero.Optimization.Solvers.Shared.NearestNeighbours;

namespace Itinero.Optimization.Solvers.TSP_TW_D
{
    /// <summary>
    /// The TSP.
    /// </summary>
    internal sealed class TSPTWDProblem
    {
        private readonly float[][] _weights;
        private readonly float[] _turnPenalties;
        private readonly Lazy<NearestNeighbourCache> _nearestNeighbourCacheLazy;
        private readonly Lazy<TSPTWDProblem> _closedEquivalent;
        private readonly Lazy<TSP_TW.TSPTWProblem> _undirectedEquivalentLazy;
        private readonly bool _behaveAsClosed = false;
        private readonly int? _last;
        private readonly HashSet<int> _visits;
        
        private TSPTWDProblem(TSPTWDProblem other, bool behaveAsClosed)
        {
            _weights = other._weights;
            _nearestNeighbourCacheLazy = other._nearestNeighbourCacheLazy;
            _visits = other._visits;
            this.First = other.First;
            _last = other._last;
            this.Windows = other.Windows;
            _turnPenalties = other._turnPenalties;
            
            _behaveAsClosed = behaveAsClosed;
        }
        
        /// <summary>
        /// Creates a new TSP-TW.
        /// - An 'open' TSP-TW, when last is null.
        /// - A 'closed' TSP-TW, when last = first.
        /// - A 'fixed' TSP-TW, all other cases, when first != last and last != null.
        /// </summary>
        /// <param name="first">The first visit.</param>
        /// <param name="last">The last visit if any.</param>
        /// <param name="weights">The weights matrix.</param>
        /// <param name="windows">The time windows.</param>
        /// <param name="visits">The possible visits, all visits are possible if null.</param>
        public TSPTWDProblem(int first, int? last, float[][] weights, float turnPenalty, TimeWindow[] windows, IEnumerable<int>? visits = null)
        {
            this.First = first;
            _last = last;
            _weights = weights;
            this.Windows = windows;
            _turnPenalties = new [] {
                0,
                turnPenalty,
                turnPenalty,
                0
            };
            
            _nearestNeighbourCacheLazy = new Lazy<NearestNeighbourCache>(() =>
                new NearestNeighbourCache(_weights.Length, (x, y) => _weights[x][y]));
            _closedEquivalent = new Lazy<TSPTWDProblem>(() => 
                new TSPTWDProblem(this, true));
            _undirectedEquivalentLazy = new Lazy<TSP_TW.TSPTWProblem>(() => new TSP_TW.TSPTWProblem(this.First, _last, 
                _weights.ConvertToUndirected(), this.Windows, _visits?.Select(DirectedHelper.ExtractVisit)));

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
            if (this.Windows == null) { throw new ArgumentNullException(nameof(windows)); }
            if (this.Windows.Length != _weights.Length / 2) { throw new ArgumentException($"Time window array has the wrong dimensions: Expected {_weights.Length / 2}, is {this.Windows.Length}."); }
        }
        
        /// <summary>
        /// Gets the weight between the two given visits.
        /// </summary>
        public float Weight(int from, int to)
        {
            if (_behaveAsClosed)
            {
                var toDetails = DirectedHelper.WeightIdToVisit(to);
                if (!_last.HasValue && toDetails.visit == this.First)
                { // make sure all costs to 'first' are '0'.
                    return 0;
                }
                else if (_last.HasValue && _last != this.First)
                { // first and last are different.
                    // pretend '-> first' is '-> last' and remove last altogether.
                    if (toDetails.visit == this.First)
                    {
                        to = DirectedHelper.WeightId(_last.Value, toDetails.direction);
                    }

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
        /// Gets the windows.
        /// </summary>
        public TimeWindow[] Windows { get; private set; }

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
        /// Gets or sets the turn penalties per type of turn.
        /// </summary>
        internal float TurnPenalty(TurnEnum turn)
        {
            return _turnPenalties[(byte) turn];
        }

        /// <summary>
        /// Gets the number of weights.
        /// </summary>
        public int WeightsSize => _weights.Length;

        /// <summary>
        /// Gets the nearest neighbour cache.
        /// </summary>
        internal NearestNeighbourCache NearestNeighbourCache => _nearestNeighbourCacheLazy.Value;

        /// <summary>
        /// Gets the closed equivalent of this problem.
        /// </summary>
        internal TSPTWDProblem ClosedEquivalent
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