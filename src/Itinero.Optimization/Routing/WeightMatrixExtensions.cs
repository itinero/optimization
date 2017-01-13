// Itinero.Optimization - Route optimization for .NET
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
using Itinero.Algorithms.Matrices;
using Itinero.Algorithms.Weights;
using Itinero.Data.Network;
using Itinero.Optimization.Algorithms.Directed;
using Itinero.Optimization.Tours;
using System.Collections.Generic;
using System.Linq;

namespace Itinero.Optimization.Routing
{

    /// <summary>
    /// Contains extension methods for weight matrix algorithms.
    /// </summary>
    public static class WeightMatrixExtensions
    {
        /// <summary>
        /// Converts a customer index to a location index.
        /// </summary>
        public static int OriginalLocationIndexOf(this WeightMatrixAlgorithm algorithm, int id)
        {
            return algorithm.MassResolver.LocationIndexOf(algorithm.OriginalIndexOf(id));
        }

        /// <summary>
        /// Converts a location index to a customer index.
        /// </summary>
        public static int OriginalIndexOf(this WeightMatrixAlgorithm algorithm, int id)
        {
            return algorithm.IndexOf(algorithm.MassResolver.IndexOf(id));
        }
        
        /// <summary>
        /// Builds a route from a given tour.
        /// </summary>
        /// <returns></returns>
        public static Route BuildRoute(this WeightMatrixAlgorithm algorithm, Tour tour)
        {
            Route route = null;
            foreach (var pair in tour.Pairs())
            {
                var localRoute = algorithm.Router.Calculate(algorithm.Profile, algorithm.RouterPoints[pair.From],
                    algorithm.RouterPoints[pair.To]);
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
        /// Builds the routes in segments divided by routes between customers in the given tour.
        /// </summary>
        /// <returns></returns>
        public static List<Result<Route>> TryBuildRoutes(this WeightMatrixAlgorithm algorithm, Tour tour)
        {
            var routes = new List<Result<Route>>();
            foreach (var pair in tour.Pairs())
            {
                routes.Add(algorithm.Router.TryCalculate(algorithm.Profile, algorithm.RouterPoints[pair.From],
                    algorithm.RouterPoints[pair.To]));
            }
            return routes;
        }

        /// <summary>
        /// Builds the result route in segments divided by routes between customers.
        /// </summary>
        /// <returns></returns>
        public static List<Route> BuildRoutes(this WeightMatrixAlgorithm algorithm, Tour tour)
        {
            var routes = new List<Route>();
            foreach (var pair in tour.Pairs())
            {
                var from = algorithm.RouterPoints[pair.From];
                var to = algorithm.RouterPoints[pair.To];

                var result = algorithm.Router.TryCalculate(algorithm.Profile, from, to);
                if (result.IsError)
                {
                    throw new Itinero.Exceptions.RouteNotFoundException(
                        string.Format("Part of the tour was not found: {0}[{1}] -> {2}[{3}] - {4}.",
                            pair.From, from, pair.To, to, result.ErrorMessage));
                }
                routes.Add(result.Value);
            }
            return routes;
        }

        /// <summary>
        /// Converts a directed customer index to a directed location index.
        /// </summary>
        public static int DirectedOriginalLocationIndexOf(this DirectedWeightMatrixAlgorithm algorithm, int directedId)
        {
            int arrivalId, departuredId, id, turn;
            DirectedHelper.ExtractAll(directedId, out arrivalId, out departuredId, out id, out turn);
            var index = algorithm.MassResolver.LocationIndexOf(algorithm.OriginalIndexOf(id));
            return DirectedHelper.BuildDirectedId(index, turn);
        }

        /// <summary>
        /// Converts a directed location index to a directed customer index.
        /// </summary>
        public static int DirectedOriginalIndexOf(this DirectedWeightMatrixAlgorithm algorithm, int directedId)
        {
            int arrivalId, departuredId, id, turn;
            DirectedHelper.ExtractAll(directedId, out arrivalId, out departuredId, out id, out turn);
            var index = algorithm.IndexOf(algorithm.MassResolver.IndexOf(id));
            return DirectedHelper.BuildDirectedId(index, turn);
        }
        
        /// <summary>
        /// Builds the resulting route.
        /// </summary>
        /// <returns></returns>
        public static Route BuildRoute(this DirectedWeightMatrixAlgorithm algorithm, Tour tour)
        {
            Route route = null;
            // TODO: check what to do here, use the cached version or not?
            var weightHandler = algorithm.Profile.DefaultWeightHandler(algorithm.Router);
            foreach (var pair in tour.Pairs())
            {
                // TODO: extract more info at once!
                var pairFromDepartureId =  algorithm.SourcePaths[DirectedHelper.ExtractDepartureId(pair.From)];
                var pairToArrivalId = algorithm.TargetPaths[DirectedHelper.ExtractArrivalId(pair.To)];

                var pairFromEdgeId = algorithm.Router.Db.Network.GetEdges(pairFromDepartureId.From.Vertex).First(x => x.To == pairFromDepartureId.Vertex).IdDirected();
                var pairToEdgeId = algorithm.Router.Db.Network.GetEdges(pairToArrivalId.Vertex).First(x => x.To == pairToArrivalId.From.Vertex).IdDirected();

                var pairFromId = DirectedHelper.ExtractId(pair.From);
                var pairToId = DirectedHelper.ExtractId(pair.To);

                var fromRouterPoint = algorithm.RouterPoints[pairFromId];
                var toRouterPoint = algorithm.RouterPoints[pairToId];

                var localRouteRaw = algorithm.Router.TryCalculateRaw(algorithm.Profile, weightHandler, pairFromEdgeId, pairToEdgeId, null).Value;
                localRouteRaw.StripSource();
                localRouteRaw.StripTarget();

                var localRoute = algorithm.Router.BuildRoute(algorithm.Profile, weightHandler, fromRouterPoint, toRouterPoint, localRouteRaw).Value;
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
        public static List<Result<Route>> TryBuildRoutes(this DirectedWeightMatrixAlgorithm algorithm, Tour tour)
        {
            var routes = new List<Result<Route>>();
            // TODO: check what to do here, use the cached version or not?
            var weightHandler = algorithm.Profile.DefaultWeightHandler(algorithm.Router);
            foreach (var pair in tour.Pairs())
            {
                // TODO: extract more info at once!
                var pairFromDepartureId = algorithm.SourcePaths[DirectedHelper.ExtractDepartureId(pair.From)];
                var pairToArrivalId = algorithm.TargetPaths[DirectedHelper.ExtractArrivalId(pair.To)];

                var pairFromEdgeId = algorithm.Router.Db.Network.GetEdges(pairFromDepartureId.From.Vertex).First(x => x.To == pairFromDepartureId.Vertex).IdDirected();
                var pairToEdgeId = algorithm.Router.Db.Network.GetEdges(pairToArrivalId.Vertex).First(x => x.To == pairToArrivalId.From.Vertex).IdDirected();

                var pairFromId = DirectedHelper.ExtractId(pair.From);
                var pairToId = DirectedHelper.ExtractId(pair.To);

                var fromRouterPoint = algorithm.RouterPoints[pairFromId];
                var toRouterPoint = algorithm.RouterPoints[pairToId];

                var localRouteRaw = algorithm.Router.TryCalculateRaw(algorithm.Profile, weightHandler, pairFromEdgeId, pairToEdgeId, null).Value;
                localRouteRaw.StripSource();
                localRouteRaw.StripTarget();

                var localRoute = algorithm.Router.BuildRoute(algorithm.Profile, weightHandler, fromRouterPoint, toRouterPoint, localRouteRaw);
                routes.Add(localRoute);
            }
            return routes;
        }

        /// <summary>
        /// Builds the result route in segments divided by routes between customers.
        /// </summary>
        /// <returns></returns>
        public static List<Route> BuildRoutes(this DirectedWeightMatrixAlgorithm algorithm, Tour tour)
        {
            var routes = new List<Route>();
            // TODO: check what to do here, use the cached version or not?
            var weightHandler = algorithm.Profile.DefaultWeightHandler(algorithm.Router);
            foreach (var pair in tour.Pairs())
            {
                // TODO: extract more info at once!
                var pairFromDepartureId = algorithm.SourcePaths[DirectedHelper.ExtractDepartureId(pair.From)];
                var pairToArrivalId = algorithm.TargetPaths[DirectedHelper.ExtractArrivalId(pair.To)];

                var pairFromEdgeId = algorithm.Router.Db.Network.GetEdges(pairFromDepartureId.From.Vertex).First(x => x.To == pairFromDepartureId.Vertex).IdDirected();
                var pairToEdgeId = algorithm.Router.Db.Network.GetEdges(pairToArrivalId.Vertex).First(x => x.To == pairToArrivalId.From.Vertex).IdDirected();

                var pairFromId = DirectedHelper.ExtractId(pair.From);
                var pairToId = DirectedHelper.ExtractId(pair.To);

                var fromRouterPoint = algorithm.RouterPoints[pairFromId];
                var toRouterPoint = algorithm.RouterPoints[pairToId];

                var localRouteRaw = algorithm.Router.TryCalculateRaw(algorithm.Profile, weightHandler, pairFromEdgeId, pairToEdgeId, null).Value;
                localRouteRaw.StripSource();
                localRouteRaw.StripTarget();

                var localRoute = algorithm.Router.BuildRoute(algorithm.Profile, weightHandler, fromRouterPoint, toRouterPoint, localRouteRaw);
                if (localRoute.IsError)
                {
                    throw new Itinero.Exceptions.RouteNotFoundException(
                        string.Format("Part of the tour was not found: {0}[{1}] -> {2}[{3}] - {4}.",
                            pair.From, pairFromId, pair.To, pairToId, localRoute.ErrorMessage));
                }
                routes.Add(localRoute.Value);
            }
            return routes;
        }
    }
}