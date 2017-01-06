﻿// Itinero.Optimization - Route optimization for .NET
// Copyright (C) 2017 Abelshausen Ben
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
using Itinero.Data.Network;
using Itinero.Optimization.Algorithms.Directed;
using Itinero.Optimization.Algorithms.Solvers;
using Itinero.Optimization.Routing.Directed;
using Itinero.Optimization.TimeWindows;
using Itinero.Optimization.Tours;
using Itinero.Profiles;
using System.Collections.Generic;
using System.Linq;

namespace Itinero.Optimization.TSP.TimeWindows.Directed
{
    /// <summary>
    /// An algorithm to calculate TSP-TW solutions with u-turn prevention.
    /// </summary>
    public class TSPTWRouter : AlgorithmBase
    {
        private readonly Profile _profile;
        private readonly RouterPoint[] _locations;
        private readonly TimeWindow[] _windows;
        private readonly int _first;
        private readonly int? _last;
        private readonly Router _router;
        private readonly float _turnPenalty;
        private readonly Itinero.Optimization.Algorithms.Solvers.ISolver<float, TSPTWProblem, TSPTWObjective, Tour, float> _solver;

        /// <summary>
        /// Creates a new TSP router.
        /// </summary>
        public TSPTWRouter(Router router, Profile profile, RouterPoint[] locations, TimeWindow[] windows, float turnPenalty, int first, int? last = null,
            SolverBase<float, TSPTWProblem, TSPTWObjective, Itinero.Optimization.Tours.Tour, float> solver = null)
        {
            _router = router;
            _locations = locations;
            _profile = profile;
            _turnPenalty = turnPenalty;
            _windows = windows;

            _first = first;
            _last = last;
            _solver = solver;
        }

        private Tour _route = null;
        private Tour _originalRoute = null;
        private DirectedBidirectionalDykstra _turnWeightMatrix;
        
        /// <summary>
        /// Executes the actual algorithm.
        /// </summary>
        protected override void DoRun()
        {
            // calculates weight matrix.
            _turnWeightMatrix = new DirectedBidirectionalDykstra(_router, _profile, _locations);
            _turnWeightMatrix.Run();
            if (!_turnWeightMatrix.HasSucceeded)
            { // algorithm has not succeeded.
                this.ErrorMessage = string.Format("Could not calculate weight matrix: {0}",
                    _turnWeightMatrix.ErrorMessage);
                return;
            }

            LocationError error;
            if (_turnWeightMatrix.Errors.TryGetValue(_first, out error))
            { // if the first location could not be resolved everything fails.
                this.ErrorMessage = string.Format("Could resolve first location: {0}",
                    error);
                return;
            }

            // build problem.
            var first = _first;
            TSPTWProblem problem = null;
            if (_last.HasValue)
            { // the last customer was set.
                if (_turnWeightMatrix.Errors.TryGetValue(_last.Value, out error))
                { // if the last location is set and it could not be resolved everything fails.
                    this.ErrorMessage = string.Format("Could resolve last location: {0}",
                        error);
                    return;
                }

                problem = new TSPTWProblem(_turnWeightMatrix.IndexOf(first), _turnWeightMatrix.IndexOf(_last.Value),
                    _turnWeightMatrix.Weights, _windows, _turnPenalty);
            }
            else
            { // the last customer was not set.
                problem = new TSPTWProblem(_turnWeightMatrix.IndexOf(first), _turnWeightMatrix.Weights, _windows, _turnPenalty);
            }

            // execute the solver.
            if (_solver == null)
            {
                _originalRoute = problem.Solve();
            }
            else
            {
                _originalRoute = problem.Solve(_solver);
            }

            // convert route to a route with the original location indices.
            if (_originalRoute.Last.HasValue)
            {
                _route = new Tour(_originalRoute.Select(x => _turnWeightMatrix.DirectedLocationIndexOf(x)),
                    _turnWeightMatrix.DirectedLocationIndexOf(
                        _originalRoute.Last.Value));
            }
            else
            {
                _route = new Tour(_originalRoute.Select(x => _turnWeightMatrix.DirectedLocationIndexOf(x)));
            }

            this.HasSucceeded = true;
        }

        /// <summary>
        /// Gets the raw route representing the order of the locations.
        /// </summary>
        public ITour RawRoute
        {
            get
            {
                return _route;
            }
        }

        /// <summary>
        /// Builds the resulting route.
        /// </summary>
        /// <returns></returns>
        public Route BuildRoute()
        {
            this.CheckHasRunAndHasSucceeded();

            Route route = null;
            // TODO: check what to do here, use the cached version or not?
            var weightHandler = _profile.DefaultWeightHandler(_router);
            foreach (var pair in _originalRoute.Pairs())
            {
                // TODO: extract more info at once!
                var pairFromDepartureId = _turnWeightMatrix.SourcePaths[DirectedHelper.ExtractDepartureId(pair.From)];
                var pairToArrivalId = _turnWeightMatrix.TargetPaths[DirectedHelper.ExtractArrivalId(pair.To)];

                var pairFromEdgeId = _router.Db.Network.GetEdges(pairFromDepartureId.From.Vertex).First(x => x.To == pairFromDepartureId.Vertex).IdDirected();
                var pairToEdgeId = _router.Db.Network.GetEdges(pairToArrivalId.Vertex).First(x => x.To == pairToArrivalId.From.Vertex).IdDirected();

                var pairFromId = DirectedHelper.ExtractId(pair.From);
                var pairToId = DirectedHelper.ExtractId(pair.To);

                var fromRouterPoint = _locations[pairFromId];
                var toRouterPoint = _locations[pairToId];

                var localRouteRaw = _router.TryCalculateRaw(_profile, weightHandler, pairFromEdgeId, pairToEdgeId, null).Value;
                localRouteRaw.StripSource();
                localRouteRaw.StripTarget();

                var localRoute = _router.BuildRoute(_profile, weightHandler, fromRouterPoint, toRouterPoint, localRouteRaw).Value;
                if (route == null)
                {
                    route = localRoute;
                }
                else
                {
                    route = route.Concatenate(localRoute);
                }
            }
            return route;
        }

        /// <summary>
        /// Builds the result route in segments divided by routes between customers.
        /// </summary>
        /// <returns></returns>
        public List<Result<Route>> TryBuildRoutes()
        {
            this.CheckHasRunAndHasSucceeded();

            var routes = new List<Result<Route>>();
            // TODO: check what to do here, use the cached version or not?
            var weightHandler = _profile.DefaultWeightHandler(_router);
            foreach (var pair in _originalRoute.Pairs())
            {
                // TODO: extract more info at once!
                var pairFromDepartureId = _turnWeightMatrix.SourcePaths[DirectedHelper.ExtractDepartureId(pair.From)];
                var pairToArrivalId = _turnWeightMatrix.TargetPaths[DirectedHelper.ExtractArrivalId(pair.To)];

                var pairFromEdgeId = _router.Db.Network.GetEdges(pairFromDepartureId.From.Vertex).First(x => x.To == pairFromDepartureId.Vertex).IdDirected();
                var pairToEdgeId = _router.Db.Network.GetEdges(pairToArrivalId.Vertex).First(x => x.To == pairToArrivalId.From.Vertex).IdDirected();

                var pairFromId = DirectedHelper.ExtractId(pair.From);
                var pairToId = DirectedHelper.ExtractId(pair.To);

                var fromRouterPoint = _locations[pairFromId];
                var toRouterPoint = _locations[pairToId];

                var localRouteRaw = _router.TryCalculateRaw(_profile, weightHandler, pairFromEdgeId, pairToEdgeId, null).Value;
                localRouteRaw.StripSource();
                localRouteRaw.StripTarget();

                var localRoute = _router.BuildRoute(_profile, weightHandler, fromRouterPoint, toRouterPoint, localRouteRaw);
                routes.Add(localRoute);
            }
            return routes;
        }

        /// <summary>
        /// Builds the result route in segments divided by routes between customers.
        /// </summary>
        /// <returns></returns>
        public List<Route> BuildRoutes()
        {
            this.CheckHasRunAndHasSucceeded();

            var routes = new List<Route>();
            // TODO: check what to do here, use the cached version or not?
            var weightHandler = _profile.DefaultWeightHandler(_router);
            foreach (var pair in _originalRoute.Pairs())
            {
                // TODO: extract more info at once!
                var pairFromDepartureId = _turnWeightMatrix.SourcePaths[DirectedHelper.ExtractDepartureId(pair.From)];
                var pairToArrivalId = _turnWeightMatrix.TargetPaths[DirectedHelper.ExtractArrivalId(pair.To)];

                var pairFromEdgeId = _router.Db.Network.GetEdges(pairFromDepartureId.From.Vertex).First(x => x.To == pairFromDepartureId.Vertex).IdDirected();
                var pairToEdgeId = _router.Db.Network.GetEdges(pairToArrivalId.Vertex).First(x => x.To == pairToArrivalId.From.Vertex).IdDirected();

                var pairFromId = DirectedHelper.ExtractId(pair.From);
                var pairToId = DirectedHelper.ExtractId(pair.To);

                var fromRouterPoint = _locations[pairFromId];
                var toRouterPoint = _locations[pairToId];

                var localRouteRaw = _router.TryCalculateRaw(_profile, weightHandler, pairFromEdgeId, pairToEdgeId, null).Value;
                localRouteRaw.StripSource();
                localRouteRaw.StripTarget();

                var localRoute = _router.BuildRoute(_profile, weightHandler, fromRouterPoint, toRouterPoint, localRouteRaw);
                if (localRoute.IsError)
                {
                    throw new Itinero.Exceptions.RouteNotFoundException(
                        string.Format("Part of the TSP-TW-route was not found: {0}[{1}] -> {2}[{3}] - {4}.",
                            pair.From, pairFromId, pair.To, pairToId, localRoute.ErrorMessage));
                }
                routes.Add(localRoute.Value);
            }
            return routes;
        }

    }
}