// Itinero - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
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
using Itinero.LocalGeo;
using Itinero.Logistics.Solutions.TSP;
using Itinero.Logistics.Solutions.TSP.GA.EAX;
using Itinero.Logistics.Solvers;
using Itinero.Logistics.Solvers.GA;
using Itinero.Profiles;
using System.Collections.Generic;
using System.Linq;

namespace Itinero.Logistics.Routing.TSP
{
    /// <summary>
    /// A router that calculates and solves the TSP-route along a set of given points.
    /// </summary>
    public class TSPRouter : RoutingAlgorithmBase, Itinero.Logistics.Routing.TSP.ITSPRouter
    {
        private readonly ISolver<ITSP, ITSPObjective, Itinero.Logistics.Routes.IRoute> _solver;
        private readonly IRouter _router;
        private readonly Profile _profile;
        private readonly Coordinate[] _locations;
        private readonly int _first;
        private readonly int? _last;

        /// <summary>
        /// Creates a new router with default solver and settings.
        /// </summary>
        public TSPRouter(IRouter router, Profile profile, Coordinate[] locations, int first)
            : this(router, profile, locations, first, null, new EAXSolver(new GASettings()
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
        public TSPRouter(IRouter router, Profile profile, Coordinate[] locations, int first, int last)
            : this(router, profile, locations, first, last, new EAXSolver(new GASettings()
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
        public TSPRouter(IRouter router, Profile profile, Coordinate[] locations, int first, int? last,
            IWeightMatrixAlgorithm weightMatrixAlgorithm)
            : this(router, profile, locations, first, last, new EAXSolver(new GASettings()
            {
                CrossOverPercentage = 10,
                ElitismPercentage = 1,
                PopulationSize = 100,
                MaxGenerations = 100000,
                MutationPercentage = 0,
                StagnationCount = 100
            }), weightMatrixAlgorithm)
        {

        }

        /// <summary>
        /// Creates a new router with a given solver.
        /// </summary>
        public TSPRouter(IRouter router, Profile profile, Coordinate[] locations, int first, int? last, 
            ISolver<ITSP, ITSPObjective, Itinero.Logistics.Routes.IRoute> solver)
            : this(router, profile, locations, first, last, solver, new WeightMatrixAlgorithm(router, profile, locations))
        {

        }

        /// <summary>
        /// Creates a new router with a given solver.
        /// </summary>
        public TSPRouter(IRouter router, Profile profile, Coordinate[] locations, int first, int? last, 
            ISolver<ITSP, ITSPObjective, Itinero.Logistics.Routes.IRoute> solver, IWeightMatrixAlgorithm weightMatrixAlgorithm)
        {
            _router = router;
            _profile = profile;
            _locations = locations;
            _solver = solver;
            _first = first;
            _last = last;
            _weightMatrixAlgorithm = weightMatrixAlgorithm;
        }

        private Itinero.Logistics.Routes.IRoute _route = null;
        private Itinero.Logistics.Routes.IRoute _originalRoute = null;
        private IWeightMatrixAlgorithm _weightMatrixAlgorithm;

        /// <summary>
        /// Executes the actual run of the algorithm.
        /// </summary>
        protected override void DoRun()
        {
            // calculate weight matrix.
            if(!_weightMatrixAlgorithm.HasRun)
            { // only run if it has not been run yet.
                _weightMatrixAlgorithm.Run();
            }
            if(!_weightMatrixAlgorithm.HasSucceeded)
            { // algorithm has not succeeded.
                this.ErrorMessage = string.Format("Could not calculate weight matrix: {0}",
                    _weightMatrixAlgorithm.ErrorMessage);
                return;
            }

            LocationError error;
            if (_weightMatrixAlgorithm.Errors.TryGetValue(_first, out error))
            { // if the first location could not be resolved everything fails.
                this.ErrorMessage = string.Format("Could resolve first location: {0}",
                    error);
                return;
            }

            // solve.
            var first = _first;
            if(_last.HasValue)
            { // the last customer was set.
                if (_weightMatrixAlgorithm.Errors.TryGetValue(_last.Value, out error))
                { // if the last location is set and it could not be resolved everything fails.
                    this.ErrorMessage = string.Format("Could resolve last location: {0}",
                        error);
                    return;
                }

                _originalRoute = _solver.Solve(new TSPProblem(
                    _weightMatrixAlgorithm.IndexOf(first), _weightMatrixAlgorithm.IndexOf(_last.Value), _weightMatrixAlgorithm.Weights), 
                        new MinimumWeightObjective());
            }
            else
            { // the last customer was not set.
                _originalRoute = _solver.Solve(new TSPProblem(
                    _weightMatrixAlgorithm.IndexOf(first), _weightMatrixAlgorithm.Weights), 
                        new MinimumWeightObjective());
            }

            // convert route to a route with the original location indices.
            if (_originalRoute.Last.HasValue)
            {
                _route = new Itinero.Logistics.Routes.Route(_originalRoute.Select(x => _weightMatrixAlgorithm.LocationIndexOf(x)),
                    _weightMatrixAlgorithm.LocationIndexOf(
                        _originalRoute.Last.Value));
            }
            else
            {
                _route = new Itinero.Logistics.Routes.Route(_originalRoute.Select(x => _weightMatrixAlgorithm.LocationIndexOf(x)));
            }

            this.HasSucceeded = true;
        }

        /// <summary>
        /// Gets the raw route representing the order of the locations.
        /// </summary>
        public Itinero.Logistics.Routes.IRoute RawRoute
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
            foreach (var pair in _originalRoute.Pairs())
            {
                var localRoute = _router.Calculate(_profile, _weightMatrixAlgorithm.RouterPoints[pair.From],
                    _weightMatrixAlgorithm.RouterPoints[pair.To]);
                if(route == null)
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
            foreach (var pair in _originalRoute.Pairs())
            {
                routes.Add(_router.TryCalculate(_profile, _weightMatrixAlgorithm.RouterPoints[pair.From],
                    _weightMatrixAlgorithm.RouterPoints[pair.To]));
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
            foreach (var pair in _originalRoute.Pairs())
            {
                var from = _weightMatrixAlgorithm.RouterPoints[pair.From];
                var to = _weightMatrixAlgorithm.RouterPoints[pair.To];

                var result = _router.TryCalculate(_profile, from, to);
                if (result.IsError)
                {
                    throw new Itinero.Exceptions.RouteNotFoundException(
                        string.Format("Part of the TSP-route was not found: {0}[{1}] -> {2}[{3}] - {4}.",
                            pair.From, from, pair.To, to, result.ErrorMessage));
                }
                routes.Add(result.Value);
            }
            return routes;
        }
    }
}