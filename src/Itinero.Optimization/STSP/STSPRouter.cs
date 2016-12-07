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
using Itinero.LocalGeo;
using Itinero.Optimization.Algorithms.Solvers;
using Itinero.Optimization.Tours;
using Itinero.Profiles;
using System.Collections.Generic;
using System.Linq;

namespace Itinero.Optimization.STSP
{

    /// <summary>
    /// An algorithm to calculate STSP solutions.
    /// </summary>
    public class STSPRouter : AlgorithmBase
    {
        private readonly RouterBase _router;
        private readonly Coordinate[] _locations;
        private readonly int _first;
        private readonly int? _last;
        private readonly Profile _profile;
        private readonly float _max;

        /// <summary>
        /// Creates a new TSP router.
        /// </summary>
        public STSPRouter(RouterBase router, Profile profile, Coordinate[] locations, float max, int first = 0, int? last = null, 
            SolverBase<float, STSProblem, STSPObjective, Tour, STSPFitness> solver = null,
            IWeightMatrixAlgorithm<float> weightMatrixAlgorithm = null)
        {
            _router = router;
            _locations = locations;
            _profile = profile;
            _first = first;
            _last = last;
            _max = max;

            _weightMatrixAlgorithm = weightMatrixAlgorithm;
            _solver = solver;
        }

        private Tour _route = null;
        private Tour _originalRoute = null;
        private SolverBase<float, STSProblem, STSPObjective, Tour, STSPFitness> _solver;
        private IWeightMatrixAlgorithm<float> _weightMatrixAlgorithm;

        /// <summary>
        /// Excutes the actual algorithm.
        /// </summary>
        protected override void DoRun()
        {
            if (_weightMatrixAlgorithm == null)
            { // use the default implementation.
                _weightMatrixAlgorithm = new WeightMatrixAlgorithm(_router, _profile, _locations);
            }
            // calculate weight matrix.
            if (!_weightMatrixAlgorithm.HasRun)
            { // only run if it has not been run yet.
                _weightMatrixAlgorithm.Run();
            }
            if (!_weightMatrixAlgorithm.HasSucceeded)
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

            // build problem.
            var first = _first;
            STSProblem problem = null;
            if (_last.HasValue)
            { // the last customer was set.
                if (_weightMatrixAlgorithm.Errors.TryGetValue(_last.Value, out error))
                { // if the last location is set and it could not be resolved everything fails.
                    this.ErrorMessage = string.Format("Could resolve last location: {0}",
                        error);
                    return;
                }

                problem = new STSProblem(_weightMatrixAlgorithm.IndexOf(first), _weightMatrixAlgorithm.IndexOf(_last.Value),
                    _weightMatrixAlgorithm.Weights, _max);
            }
            else
            { // the last customer was not set.
                problem = new STSProblem(_weightMatrixAlgorithm.IndexOf(first), _weightMatrixAlgorithm.Weights, _max);
            }

            // solve.
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
                _route = new Tour(_originalRoute.Select(x => _weightMatrixAlgorithm.LocationIndexOf(x)),
                    _weightMatrixAlgorithm.LocationIndexOf(
                        _originalRoute.Last.Value));
            }
            else
            {
                _route = new Tour(_originalRoute.Select(x => _weightMatrixAlgorithm.LocationIndexOf(x)));
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
            foreach (var pair in _originalRoute.Pairs())
            {
                var localRoute = _router.Calculate(_profile, _weightMatrixAlgorithm.RouterPoints[pair.From],
                    _weightMatrixAlgorithm.RouterPoints[pair.To]);
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
