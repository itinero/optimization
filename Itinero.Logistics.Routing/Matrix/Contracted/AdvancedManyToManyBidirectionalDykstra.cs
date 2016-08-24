// Itinero - OpenStreetMap (OSM) SDK
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

namespace Itinero.Logistics.Routing.Matrix.Contracted
{
    /// <summary>
    /// An algorithm to calculate many-to-many weights based on a contraction hierarchy.
    /// </summary>
    public class AdvancedManyToManyBidirectionalDykstra<T> : AlgorithmBase
        where T : struct
    {
        private readonly RouterDb _routerDb;
        private readonly DirectedDynamicGraph _graph;
        private readonly RouterPoint[] _sources;
        private readonly RouterPoint[] _targets;
        private readonly Dictionary<uint, Dictionary<int, LinkedEdgePath<T>>> _buckets;
        private readonly WeightHandler<T> _weightHandler;

        /// <summary>
        /// Creates a new algorithm.
        /// </summary>
        public AdvancedManyToManyBidirectionalDykstra(RouterDb routerDb, Profile profile, WeightHandler<T> weightHandler, RouterPoint[] sources,
            RouterPoint[] targets)
        {
            _routerDb = routerDb;
            _sources = sources;
            _targets = targets;
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

            _buckets = new Dictionary<uint, Dictionary<int, LinkedEdgePath<T>>>();
        }

        private T[][] _weights;
        private EdgePath<T>[] _sourcePaths;
        private EdgePath<T>[] _targetPaths;

        /// <summary>
        /// Executes the actual run.
        /// </summary>
        protected override void DoRun()
        {
            // convert sources into directed paths.
            _sourcePaths = new EdgePath<T>[_sources.Length * 2];
            for(var i = 0; i < _sources.Length; i++)
            {
                var paths = _sources[i].ToEdgePathsAdvanced(_routerDb, _weightHandler, true);
                if (paths.Length == 0)
                {
                    this.ErrorMessage = string.Format("Source at {0} could not be resolved properly.", i);
                    return;
                }
                for (var p = 0; p < paths.Length; p++)
                {
                    _sourcePaths[i * 2 + p] = paths[p];
                }
            }

            // convert targets into directed paths.
            _targetPaths = new EdgePath<T>[_targets.Length * 2];
            for (var i = 0; i < _targets.Length; i++)
            {
                var paths = _targets[i].ToEdgePathsAdvanced(_routerDb, _weightHandler, false);
                if (paths.Length == 0)
                {
                    this.ErrorMessage = string.Format("Target at {0} could not be resolved properly.", i);
                    return;
                }
                for (var p = 0; p < paths.Length; p++)
                {
                    _targetPaths[i * 2 + p] = paths[p];
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
                    var forward = new Itinero.Algorithms.Contracted.EdgeBased.Dykstra<T>(_graph, _weightHandler, new EdgePath<T>[] { path }, (v) => null, false);
                    forward.WasFound += (vertex, weight) =>
                    {
                        LinkedEdgePath<T> visits;
                        forward.TryGetVisits(vertex, out visits);
                        return this.ForwardVertexFound(i, vertex, visits);
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
                    var backward = new Itinero.Algorithms.Contracted.EdgeBased.Dykstra<T>(_graph, _weightHandler, new EdgePath<T>[] { path }, (v) => null, true);
                    backward.WasFound += (vertex, weight) =>
                    {
                        LinkedEdgePath<T> visits;
                        backward.TryGetVisits(vertex, out visits);
                        return this.BackwardVertexFound(i, vertex, visits);
                    };
                    backward.Run();
                }
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
    }

    /// <summary>
    /// An algorithm to calculate many-to-many weights based on a contraction hierarchy.
    /// </summary>
    public sealed class AdvancedManyToManyBidirectionalDykstra : AdvancedManyToManyBidirectionalDykstra<float>
    {
        /// <summary>
        /// Creates a new algorithm.
        /// </summary>
        public AdvancedManyToManyBidirectionalDykstra(Router router, Profile profile, RouterPoint[] sources,
            RouterPoint[] targets)
            : base(router.Db, profile, profile.DefaultWeightHandler(router), sources, targets)
        {

        }
    }
}