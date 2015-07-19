// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

using OsmSharp.Logistics.Solutions.TSP;
using OsmSharp.Logistics.Solutions.TSP.GA.EAX;
using OsmSharp.Logistics.Solvers;
using OsmSharp.Logistics.Solvers.GA;
using OsmSharp.Math.Geo;
using OsmSharp.Routing;
using OsmSharp.Routing.Routers;
using OsmSharp.Routing.Vehicles;
using System.Collections.Generic;
using System.Linq;

namespace OsmSharp.Logistics.Routing.TSP
{
    /// <summary>
    /// A router that calculates and solves the TSP-route along a set of given points.
    /// </summary>
    public class TSPRouter : RoutingAlgorithmBase
    {
        private readonly ISolver<ITSP, ITSPObjective, OsmSharp.Logistics.Routes.IRoute> _solver;
        private readonly ITypedRouter _router;
        private readonly Vehicle _vehicle;
        private readonly GeoCoordinate[] _locations;
        private readonly int _first;
        private readonly int? _last;

        /// <summary>
        /// Creates a new router with default solver and settings.
        /// </summary>
        public TSPRouter(ITypedRouter router, Vehicle vehicle, GeoCoordinate[] locations, int first)
            : this(router, vehicle, locations, first, null, new EAXSolver(new GASettings()
            {
                CrossOverPercentage = 10,
                ElitismPercentage = 1,
                PopulationSize = 100,
                MaxGenerations = 100000,
                MutationPercentage = 0,
                StagnationCount = 100
            }))
        {

        }

        /// <summary>
        /// Creates a new router with default solver and settings.
        /// </summary>
        public TSPRouter(ITypedRouter router, Vehicle vehicle, GeoCoordinate[] locations, int first, int last)
            : this(router, vehicle, locations, first, last, new EAXSolver(new GASettings()
            {
                CrossOverPercentage = 10,
                ElitismPercentage = 1,
                PopulationSize = 100,
                MaxGenerations = 100000,
                MutationPercentage = 0,
                StagnationCount = 100
            }))
        {

        }

        /// <summary>
        /// Creates a new router with a given solver.
        /// </summary>
        public TSPRouter(ITypedRouter router, Vehicle vehicle, GeoCoordinate[] locations, int first, int? last, 
            ISolver<ITSP, ITSPObjective, OsmSharp.Logistics.Routes.IRoute> solver)
        {
            _router = router;
            _vehicle = vehicle;
            _locations = locations;
            _solver = solver;
            _first = first;
            _last = last;
        }

        private OsmSharp.Logistics.Routes.IRoute _route = null;
        private RouterPoint[] _resolvedPoints;
        private List<int> _errors = new List<int>();

        /// <summary>
        /// Executes the actual run of the algorithm.
        /// </summary>
        protected override void DoRun()
        {
            // resolve all locations.
            _resolvedPoints = _router.Resolve(_vehicle, _locations);

            // add id's to each resolved point.
            for (var idx = 0; idx < _resolvedPoints.Length; idx++)
            {
                if (_resolvedPoints[idx] != null)
                { // location resolved just fine.
                    if (_resolvedPoints[idx].Tags == null)
                    {
                        _resolvedPoints[idx].Tags = new List<KeyValuePair<string, string>>();
                    }
                    _resolvedPoints[idx].Tags.Add(new KeyValuePair<string, string>("point_id", idx.ToInvariantString()));
                }
                else
                { // location could not be resolved.
                    _errors.Add(idx);
                }
            }

            // filter out all points that failed to resolve.
            var nonNullResolved = new List<RouterPoint>();
            var nonNullOffset = new List<int>();
            for (var idx = 0; idx < _resolvedPoints.Length; idx++)
            {
                if (_resolvedPoints[idx] != null)
                {
                    nonNullResolved.Add(_resolvedPoints[idx]);
                    nonNullOffset.Add(nonNullOffset.Count - idx);
                }
            }

            // calculate weights.
            var nonNullResolvedArray = nonNullResolved.ToArray();
            var nonNullInvalids = new HashSet<int>();
            var nonNullWeights = _router.CalculateManyToManyWeight(_vehicle, nonNullResolvedArray, nonNullResolvedArray, nonNullInvalids);

            // solve.
            var first = _first;
            if(_last.HasValue)
            { // the last customer was set.
                _route = _solver.Solve(new TSPProblem(first, _last.Value, nonNullWeights), new MinimumWeightObjective());
            }
            else
            { // the last customer was not set.
                _route = _solver.Solve(new TSPProblem(first, nonNullWeights), new MinimumWeightObjective());
            }

            this.HasSucceeded = true;
        }

        /// <summary>
        /// Builds a route along all the given points.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="resolved"></param>
        /// <param name="coordinates"></param>
        /// <returns></returns>
        private Route BuildRoute(Vehicle vehicle, RouterPoint[] resolved, GeoCoordinate[] coordinates)
        {
            var routes = new Route[resolved.Length - 1];
            for (int idx = 1; idx < resolved.Length; idx++)
            {
                if (resolved[idx - 1] == null || resolved[idx] == null)
                { // failed to resolve point(s), replace with a dummy route.
                    routes[idx - 1] = null;
                }
                else
                { // both points are resolved, calculate route.
                    var localRoute = _router.Calculate(vehicle, resolved[idx - 1], resolved[idx]);
                    if (localRoute != null)
                    { // route was found.
                        routes[idx - 1] = localRoute;
                    }
                    else
                    { // failed to calculate route, replace with a dummy route.
                        routes[idx - 1] = null;
                    }
                }
            }

            // concatenate the routes.
            var route = routes[0];
            for (int idx = 1; idx < routes.Length; idx++)
            {
                route = Route.Concatenate(route, routes[idx]);
            }
            return route;
        }

        /// <summary>
        /// Returns the list of location's indexes that could not be routed.
        /// </summary>
        public List<int> Errors
        {
            get
            {
                return _errors;
            }
        }

        /// <summary>
        /// Gets the raw route representing the order of the locations.
        /// </summary>
        public OsmSharp.Logistics.Routes.IRoute RawRoute
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

            // sort resolved and coordinates.
            var solution = _route.ToArray();
            var size = _first == _last ? solution.Length + 1 : solution.Length;
            var sortedResolved = new RouterPoint[size];
            var sortedCoordinates = new GeoCoordinate[size];
            for (var idx = 0; idx < solution.Length; idx++)
            {
                sortedResolved[idx] = _resolvedPoints[solution[idx]];
                sortedCoordinates[idx] = _locations[solution[idx]];
            }

            // make round if needed.
            if (_first == _last)
            {
                sortedResolved[size - 1] = sortedResolved[0];
                sortedCoordinates[size - 1] = sortedCoordinates[0];
            }

            // build the route.
            return this.BuildRoute(_vehicle, sortedResolved, sortedCoordinates);
        }

        /// <summary>
        /// Builds the result route in segments divided by routes between customers.
        /// </summary>
        /// <returns></returns>
        public Route[] BuildRoutes()
        {
            this.CheckHasRunAndHasSucceeded();

            // sort resolved and coordinates.
            var solution = _route.ToArray();
            var size = _first == _last ? solution.Length + 1 : solution.Length;
            var sortedResolved = new RouterPoint[size];
            var sortedCoordinates = new GeoCoordinate[size];
            for (var idx = 0; idx < solution.Length; idx++)
            {
                sortedResolved[idx] = _resolvedPoints[solution[idx]];
                sortedCoordinates[idx] = _locations[solution[idx]];
            }

            // make round if needed.
            if (_first == _last)
            {
                sortedResolved[size - 1] = sortedResolved[0];
                sortedCoordinates[size - 1] = sortedCoordinates[0];
            }

            // build the route.
            var routes = new Route[sortedResolved.Length - 1];
            for (var i = 0; i < sortedResolved.Length - 1; i++)
            {
                routes[i] = this.BuildRoute(_vehicle, new RouterPoint[] { sortedResolved[i], sortedResolved[i + 1] },
                    new GeoCoordinate[] { sortedCoordinates[i], sortedCoordinates[i + 1] });
            }
            return routes;
        }
    }
}