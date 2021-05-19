using System;
using System.Collections.Generic;
using Itinero.Optimization.Solvers.Shared;
using Itinero.Optimization.Solvers.Shared.NearestNeighbours;
using Itinero.Optimization.Solvers.Tours;

namespace Itinero.Optimization.Solvers.TSP_TW
{
    /// <summary>
    /// The TSP-TW problem.
    /// </summary>
    internal sealed class TSPTWProblem
    {
        private readonly float[][] _weights;
        private readonly Lazy<NearestNeighbourCache> _nearestNeighbourCacheLazy;
        private readonly Lazy<TSPTWProblem> _closedEquivalent;
        private readonly Lazy<TSPTWProblem> _openEquivalent;
        private readonly TourTypeEnum _behaveAs = TourTypeEnum.Fixed;
        private readonly int? _last;
        private readonly HashSet<int> _visits;
        private readonly TimeWindow[] _timeWindows;

        private TimeWindow[]? _closedTimeWindows;
        
        private TSPTWProblem(TSPTWProblem other, TourTypeEnum behaveAs)
        {
            _weights = other._weights;
            _nearestNeighbourCacheLazy = other._nearestNeighbourCacheLazy;
            _visits = other._visits;
            this.First = other.First;
            _last = other._last;
            _timeWindows = other._timeWindows; 
            
            _behaveAs = behaveAs;

            if (_behaveAs == TourTypeEnum.Closed)
            {
                _closedTimeWindows = other._timeWindows.SubArray(0, other._timeWindows.Length - 1);
            }
            
            if (_behaveAs == TourTypeEnum.Closed)
            {
                _closedEquivalent = new Lazy<TSPTWProblem>(() => this);
                _openEquivalent = new Lazy<TSPTWProblem>(() => 
                    new TSPTWProblem(this, TourTypeEnum.Open));
            }
            else if (_behaveAs == TourTypeEnum.Open)
            {
                _closedEquivalent = new Lazy<TSPTWProblem>(() => 
                    new TSPTWProblem(this, TourTypeEnum.Closed));
                _openEquivalent = new Lazy<TSPTWProblem>(() => this);
            }
            else
            {
                _closedEquivalent = new Lazy<TSPTWProblem>(() => 
                    new TSPTWProblem(this, TourTypeEnum.Closed));
                _openEquivalent = new Lazy<TSPTWProblem>(() => 
                    new TSPTWProblem(this, TourTypeEnum.Open));
            }
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
        public TSPTWProblem(int first, int? last, float[][] weights, TimeWindow[] windows, IEnumerable<int>? visits = null)
        {
            this.First = first;
            _last = last;
            _weights = weights;
            _timeWindows = windows;

            _behaveAs = TourTypeEnum.Fixed;
            _nearestNeighbourCacheLazy = new Lazy<NearestNeighbourCache>(() =>
                new NearestNeighbourCache(_weights.Length, (x, y) => _weights[x][y]));
            _closedEquivalent = new Lazy<TSPTWProblem>(() => 
                new TSPTWProblem(this, TourTypeEnum.Closed));
            _openEquivalent = new Lazy<TSPTWProblem>(() => 
                new TSPTWProblem(this, TourTypeEnum.Open));

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
            if (this.Windows.Length != _weights.Length) { throw new ArgumentException($"Time window array has the wrong dimensions: Expected {_weights.Length}, is {this.Windows.Length}."); }
        }
        
        /// <summary>
        /// Gets the weight between the two given visits.
        /// </summary>
        public float Weight(int from, int to)
        {
            if (_behaveAs != TourTypeEnum.Fixed)
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
            if (_behaveAs != TourTypeEnum.Fixed &&
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
                var visits = _visits ?? System.Linq.Enumerable.Range(0, _weights.Length);
                if (_behaveAs != TourTypeEnum.Fixed)
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
                if (_behaveAs != TourTypeEnum.Fixed)
                {
                    return this.First;
                }
                return _last;
            }
        }

        /// <summary>
        /// Gets the windows.
        /// </summary>
        public TimeWindow[] Windows
        {
            get
            {
                if (_behaveAs is TourTypeEnum.Fixed or TourTypeEnum.Open)
                    return _timeWindows;

                return _closedTimeWindows;
            }
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
                if (_behaveAs != TourTypeEnum.Fixed &&
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
        /// Gets the nearest neighbour cache.
        /// </summary>
        internal NearestNeighbourCache NearestNeighbourCache => _nearestNeighbourCacheLazy.Value;

        /// <summary>
        /// Gets the closed equivalent of this problem.
        /// </summary>
        internal TSPTWProblem ClosedEquivalent => _closedEquivalent.Value;

        /// <summary>
        /// Gets the open equivalent of this problem.
        /// </summary>
        internal TSPTWProblem OpenEquivalent => _openEquivalent.Value;
    }
}