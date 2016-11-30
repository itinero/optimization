// Itinero.Optimization - Route optimization for .NET
// Copyright (C) 2016 Abelshausen Ben
// 
// This file is part of Itinero.
// 
// Itinero is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// Itinero is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Itinero. If not, see <http://www.gnu.org/licenses/>.

using Itinero.Algorithms;
using Itinero.Algorithms.Weights;
using Itinero.Data.Contracted;
using Itinero.Graphs.Directed;
using Itinero.Profiles;
using System;
using System.Collections.Generic;

namespace Itinero.Routing.Optimization.Directed
{
    /// <summary>
    /// An algorithm to calculate a turn-aware weight matrix.
    /// </summary>
    public class DirectedBidirectionalDykstra<T> : AlgorithmBase
        where T : struct
    {
        private readonly RouterDb _routerDb;
        private readonly DirectedDynamicGraph _graph;
        private readonly RouterPoint[] _locations;
        private readonly Dictionary<uint, Dictionary<int, LinkedEdgePath<T>>> _buckets;
        private readonly WeightHandler<T> _weightHandler;
        private readonly T _max;
        /// <summary>
        /// Creates a new algorithm.
        /// </summary>
        public DirectedBidirectionalDykstra(RouterDb routerDb, Profile profile, WeightHandler<T> weightHandler, RouterPoint[] locations)
            : this(routerDb, profile, weightHandler, locations, weightHandler.Infinite)
        {

        }

        /// <summary>
        /// Creates a new algorithm.
        /// </summary>
        public DirectedBidirectionalDykstra(RouterDb routerDb, Profile profile, WeightHandler<T> weightHandler, RouterPoint[] locations,
            T max)
        {
            _routerDb = routerDb;
            _locations = locations;
            _weightHandler = weightHandler;

            ContractedDb contractedDb;
            if (!_routerDb.TryGetContracted(profile, out contractedDb))
            {
                throw new NotSupportedException(
                    "Contraction-based many-to-many calculates are not supported in the given router db for the given profile.");
            }
            if (!contractedDb.HasEdgeBasedGraph)
            {
                throw new NotSupportedException(
                    "Contraction-based edge-based many-to-many calculates are not supported in the given router db for the given profile.");
            }
            _graph = contractedDb.EdgeBasedGraph;
            weightHandler.CheckCanUse(contractedDb);
            _max = max;

            _buckets = new Dictionary<uint, Dictionary<int, LinkedEdgePath<T>>>();
        }

        private Dictionary<int, LocationError> _errors; // all errors per original location idx.
        private List<int> _resolvedPointsIndices; // the original location per resolved point index.
        private T[][] _weights;
        private EdgePath<T>[] _sourcePaths;
        private EdgePath<T>[] _targetPaths;
        private List<RouterPoint> _validPoints;

        /// <summary>
        /// Executes the actual run.
        /// </summary>
        protected override void DoRun()
        {
            _errors = new Dictionary<int, LocationError>();
            _resolvedPointsIndices = new List<int>();

            // convert sources into directed paths.
            _sourcePaths = new EdgePath<T>[_locations.Length * 2];
            for (var i = 0; i < _locations.Length; i++)
            {
                _resolvedPointsIndices.Add(i);

                var paths = _locations[i].ToEdgePathsAdvanced(_routerDb, _weightHandler, true);
                if (paths.Length == 0)
                {
                    this.ErrorMessage = string.Format("Source at {0} could not be resolved properly.", i);
                    return;
                }
                if (paths.Length != 2)
                {
                    this.ErrorMessage = "There should be exactly two paths leaving every location.";
                    return;
                }
                _sourcePaths[i * 2 + 0] = paths[0];
                _sourcePaths[i * 2 + 1] = paths[1];
            }

            // convert targets into directed paths.
            _targetPaths = new EdgePath<T>[_locations.Length * 2];
            for (var i = 0; i < _locations.Length; i++)
            {
                var paths = _locations[i].ToEdgePathsAdvanced(_routerDb, _weightHandler, false);
                if (paths.Length == 0)
                {
                    this.ErrorMessage = string.Format("Target at {0} could not be resolved properly.", i);
                    return;
                }
                if (paths.Length != 2)
                {
                    this.ErrorMessage = "There should be exactly two paths leaving every location.";
                    return;
                }

                // make sure paths are the opposive of the sources.
                if (paths[0].Edge == _sourcePaths[i * 2 + 0].Edge)
                { // switchs.
                    _targetPaths[i * 2 + 1] = paths[0];
                    _targetPaths[i * 2 + 0] = paths[1];
                }
                else
                { // keep.
                    _targetPaths[i * 2 + 0] = paths[0];
                    _targetPaths[i * 2 + 1] = paths[1];
                }
            }

            // put in default weights and weights for one-edge-paths.
            _weights = new T[_sourcePaths.Length][];
            for (var i = 0; i < _sourcePaths.Length; i++)
            {
                var source = _sourcePaths[i];
                _weights[i] = new T[_targetPaths.Length];
                for (var j = 0; j < _targetPaths.Length; j++)
                {
                    var target = _targetPaths[j];
                    _weights[i][j] = _weightHandler.Infinite;

                    //if (target.EdgeId == source.EdgeId)
                    //{
                    //    var path = source.EdgePathTo(_routerDb, _weightHandler, target);
                    //    if (path != null)
                    //    {
                    //        _weights[i][j] = path.Weight;
                    //    }
                    //}
                }
            }

            // do forward searches into buckets.
            for (var i = 0; i < _sourcePaths.Length; i++)
            {
                var path = _sourcePaths[i];
                if (path != null)
                {
                    var forward = new Itinero.Algorithms.Contracted.EdgeBased.Dykstra<T>(_graph, _weightHandler, new EdgePath<T>[] { path }, (v) => null, false, _max);
                    forward.WasFound += (foundPath) =>
                    {
                        LinkedEdgePath<T> visits;
                        forward.TryGetVisits(foundPath.Vertex, out visits);
                        return this.ForwardVertexFound(i, foundPath.Vertex, visits);
                    };
                    forward.Run();
                }
            }

            // do backward searches into buckets.
            for (var i = 0; i < _targetPaths.Length; i++)
            {
                var path = _targetPaths[i];
                if (path != null)
                {
                    var backward = new Itinero.Algorithms.Contracted.EdgeBased.Dykstra<T>(_graph, _weightHandler, new EdgePath<T>[] { path }, (v) => null, true, _max);
                    backward.WasFound += (foundPath) =>
                    {
                        LinkedEdgePath<T> visits;
                        backward.TryGetVisits(foundPath.Vertex, out visits);
                        return this.BackwardVertexFound(i, foundPath.Vertex, visits);
                    };
                    backward.Run();
                }
            }

            // check for invalids.
            var invalidTargetCounts = new int[_sourcePaths.Length];
            var nonNullInvalids = new HashSet<int>();
            for (var s = 0; s < _weights.Length; s++)
            {
                var invalids = 0;
                for (var t = 0; t < _weights[s].Length; t++)
                {
                    if (t != s)
                    {
                        if (_weightHandler.GetMetric(_weights[s][t]) == float.MaxValue)
                        {
                            invalids++;
                            invalidTargetCounts[t]++;
                            if (invalidTargetCounts[t] > (_sourcePaths.Length - 1) / 2)
                            {
                                nonNullInvalids.Add(t);
                            }
                        }
                    }
                }

                if (invalids > (_sourcePaths.Length - 1) / 2)
                {
                    nonNullInvalids.Add(s);
                }
            }

            // take into account the non-null invalids now.
            if (nonNullInvalids.Count > 0)
            { // shrink lists and add errors.
                // convert to original indices.
                var originalInvalids = new HashSet<int>();
                foreach (var invalid in nonNullInvalids)
                { // check if both are invalid for each router point.
                    if (invalid % 2 == 0)
                    {
                        if (originalInvalids.Contains(invalid + 1))
                        {
                            originalInvalids.Add(invalid / 2);
                        }
                    }
                    else
                    {
                        if (originalInvalids.Contains(invalid - 1))
                        {
                            originalInvalids.Add(invalid / 2);
                        }
                    }
                }

                _validPoints = (new List<RouterPoint>(_locations)).ShrinkAndCopyList(originalInvalids);
                _resolvedPointsIndices = _resolvedPointsIndices.ShrinkAndCopyList(originalInvalids);

                // convert back to the path indexes.
                nonNullInvalids = new HashSet<int>();
                foreach (var invalid in originalInvalids)
                {
                    nonNullInvalids.Add(invalid * 2);
                    nonNullInvalids.Add(invalid * 2 + 1);
                }

                foreach (var invalid in nonNullInvalids)
                {
                    _errors[_resolvedPointsIndices[invalid / 2]] = new LocationError()
                    {
                        Code = LocationErrorCode.NotRoutable,
                        Message = "Location could not routed to or from."
                    };
                }

                _weights = _weights.SchrinkAndCopyMatrix(nonNullInvalids);
            }

            this.HasSucceeded = true;
        }

        /// <summary>
        /// Gets the weights.
        /// </summary>
        public T[][] Weights
        {
            get
            {
                return _weights;
            }
        }

        /// <summary>
        /// Gets the source paths.
        /// </summary>
        public EdgePath<T>[] SourcePaths
        {
            get
            {
                return _sourcePaths;
            }
        }

        /// <summary>
        /// Gets the target paths.
        /// </summary>
        public EdgePath<T>[] TargetPaths
        {
            get
            {
                return _targetPaths;
            }
        }

        /// <summary>
        /// Called when a forward vertex was found.
        /// </summary>
        /// <returns></returns>
        private bool ForwardVertexFound(int i, uint vertex, LinkedEdgePath<T> visit)
        {
            Dictionary<int, LinkedEdgePath<T>> bucket;
            if (!_buckets.TryGetValue(vertex, out bucket))
            {
                bucket = new Dictionary<int, LinkedEdgePath<T>>();
                _buckets.Add(vertex, bucket);
            }
            bucket[i] = visit;
            return false;
        }

        /// <summary>
        /// Called when a backward vertex was found.
        /// </summary>
        /// <returns></returns>
        private bool BackwardVertexFound(int i, uint vertex, LinkedEdgePath<T> backwardVisit)
        {
            Dictionary<int, LinkedEdgePath<T>> bucket;
            if (_buckets.TryGetValue(vertex, out bucket))
            {
                var originalBackwardVisit = backwardVisit;
                foreach (var pair in bucket)
                {
                    var best = _weights[pair.Key][i];

                    var forwardVisit = pair.Value;
                    while (forwardVisit != null)
                    {
                        var forwardCurrent = forwardVisit.Path;
                        if (_weightHandler.IsLargerThan(forwardCurrent.Weight, best))
                        {
                            forwardVisit = forwardVisit.Next;
                            continue;
                        }
                        backwardVisit = originalBackwardVisit;
                        while (backwardVisit != null)
                        {
                            var backwardCurrent = backwardVisit.Path;
                            var totalCurrentWeight = _weightHandler.Add(forwardCurrent.Weight, backwardCurrent.Weight);
                            if (_weightHandler.IsSmallerThan(totalCurrentWeight, best))
                            { // potentially a weight improvement.
                                var allowed = true;
                                //if (restrictions != null)
                                //{
                                //    allowed = false;
                                //    var sequence = new List<uint>(
                                //        current.GetSequence2(edgeEnumerator));
                                //    sequence.Reverse();
                                //    sequence.Add(current.Vertex);
                                //    var s1 = backwardPath.Path.GetSequence2(edgeEnumerator);
                                //    sequence.AddRange(s1);

                                //    allowed = restrictions.IsSequenceAllowed(sequence);
                                //}

                                if (allowed)
                                {
                                    best = totalCurrentWeight;
                                }
                            }
                            backwardVisit = backwardVisit.Next;
                        }
                        forwardVisit = forwardVisit.Next;
                    }

                    _weights[pair.Key][i] = best;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns the index of the location in the resolved points list.
        /// </summary>
        /// <returns></returns>
        public int IndexOf(int locationIdx)
        {
            this.CheckHasRunAndHasSucceeded();

            return _resolvedPointsIndices.IndexOf(locationIdx);
        }

        /// <summary>
        /// Returns the index of the router point in the original locations array.
        /// </summary>
        /// <returns></returns>
        public int LocationIndexOf(int routerPointIdx)
        {
            this.CheckHasRunAndHasSucceeded();

            return _resolvedPointsIndices[routerPointIdx];
        }

        /// <summary>
        /// Returns the errors indexed per location idx.
        /// </summary>
        public Dictionary<int, LocationError> Errors
        {
            get
            {
                this.CheckHasRunAndHasSucceeded();

                return _errors;
            }
        }
    }

    /// <summary>
    /// An algorithm to calculate many-to-many weights based on a contraction hierarchy.
    /// </summary>
    public sealed class DirectedBidirectionalDykstra : DirectedBidirectionalDykstra<float>
    {
        /// <summary>
        /// Creates a new algorithm.
        /// </summary>
        public DirectedBidirectionalDykstra(Router router, Profile profile, RouterPoint[] locations, float max = float.MaxValue)
            : base(router.Db, profile, profile.DefaultWeightHandler(router), locations, max)
        {

        }
    }
}
