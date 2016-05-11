// Itinero.Logistics - Route optimization for .NET
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

using Itinero.Profiles;
using System.Collections.Generic;

namespace Itinero.Logistics.Routing.STSP
{
    /// <summary>
    /// A router that calculates and generates an STSP-route using a set of given points.
    /// </summary>
    public class STSPRouter : Algorithm, ISTSPRouter
    {
        private readonly RouterPoint _source;
        private readonly Profile _profile;
        private readonly HashSet<RouterPoint> _locations;
        private readonly Router _router;
        private readonly float _max;

        /// <summary>
        /// Creates a new STSP-router starting at the given source and using the given locations as guideline.
        /// </summary>
        public STSPRouter(Router router, Profile profile, RouterPoint source, HashSet<RouterPoint> locations, float max)
        {
            _router = router;
            _profile = profile;
            _source = source;
            _locations = locations;
            _max = max;
        }

        /// <summary>
        /// Creates a new STSP-router starting at the given source and using the given locations as guideline.
        /// </summary>
        public STSPRouter(Router router, Profile profile, RouterPoint source, HashSet<uint> vertices, float max)
        {
            _router = router;
            _profile = profile;
            _source = source;
            _max = max;

            _locations = new HashSet<RouterPoint>();
            foreach (var vertex in vertices)
            {
                _locations.Add(_router.Db.Network.CreateRouterPointForVertex(vertex));
            }
        }

        private Itinero.Logistics.Routes.IRoute _route = null;
        private List<RouterPoint> _all = null;

        /// <summary>
        /// Executes the actual run.
        /// </summary>
        protected override void DoRun()
        {
            _all = new List<RouterPoint>(_locations);
            _all.Insert(0, _source);
            var weights = _router.CalculateWeight(_profile, _all.ToArray(), null);

            var solver = new Itinero.Logistics.Solutions.STSP.VNS.VNSConstructionSolver(2, 2);
            var problem = new Itinero.Logistics.Solutions.STSP.STSPProblem(0, 0, weights, _max);
            _route = solver.Solve(problem, new Solutions.STSP.MinimumWeightObjective());

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
            foreach (var pair in _route.Pairs())
            {
                var localRoute = _router.Calculate(_profile, _all[pair.From],
                    _all[pair.To]);
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
            foreach (var pair in _route.Pairs())
            {
                routes.Add(_router.TryCalculate(_profile, _all[pair.From],
                    _all[pair.To]));
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
            foreach (var pair in _route.Pairs())
            {
                var from = _all[pair.From];
                var to = _all[pair.To];

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
