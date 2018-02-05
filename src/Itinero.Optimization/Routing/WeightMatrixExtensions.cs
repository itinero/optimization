/*
 *  Licensed to SharpSoftware under one or more contributor
 *  license agreements. See the NOTICE file distributed with this work for 
 *  additional information regarding copyright ownership.
 * 
 *  SharpSoftware licenses this file to you under the Apache License, 
 *  Version 2.0 (the "License"); you may not use this file except in 
 *  compliance with the License. You may obtain a copy of the License at
 * 
 *       http://www.apache.org/licenses/LICENSE-2.0
 * 
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */

using Itinero.Algorithms;
using Itinero.Algorithms.Matrices;
using Itinero.Algorithms.Weights;
using Itinero.Data.Network;
using Itinero.Optimization.Algorithms.Directed;
using Itinero.Optimization.Tours;
using System.Collections.Generic;
using System.Linq;
using Itinero.LocalGeo;

namespace Itinero.Optimization.Routing
{

    /// <summary>
    /// Contains extension methods for weight matrix algorithms.
    /// </summary>
    public static class WeightMatrixExtensions
    {
        /// <summary>
        /// Returns true if the boundingboxes of the two tours overlap.
        /// </summary>
        /// <param name="algorithm">The weight matrix algorithm.</param>
        /// <param name="tour1">The first tour.</param>
        /// <param name="tour2">The second tour.</param>
        /// <returns></returns>
        public static bool ToursOverlap<T>(this IWeightMatrixAlgorithm<T> algorithm, ITour tour1, ITour tour2)
        {
            var box1 = algorithm.BoundingBox(tour1);
            var box2 = algorithm.BoundingBox(tour2);

            return box1.Overlaps(box2);
        }

        /// <summary>
        /// Calculates the boundingbox around the given tour.
        /// </summary>
        /// <param name="algorithm">The weight matrix algorithm.</param>
        /// <param name="tour">The tour.</param>
        /// <returns></returns>
        public static Box BoundingBox<T>(this IWeightMatrixAlgorithm<T> algorithm, ITour tour)
        {
            Box? box = null;
            foreach (var visit in tour)
            {
                var visitLocation = algorithm.LocationOnNetwork(tour.First);
                if (box == null)
                {
                    box = new Box(visitLocation, visitLocation);
                }
                else
                {
                    box = box.Value.ExpandWith(visitLocation.Latitude, visitLocation.Longitude);
                }
            }
            return box.Value;
        }
        
        /// <summary>
        /// Gets the location on het routing network for a given visit.
        /// </summary>
        /// <param name="algorithm">The weight matrix algorithm.</param>
        /// <param name="visit">The visit.</param>
        /// <returns></returns>
        public static Coordinate LocationOnNetwork<T>(this IWeightMatrixAlgorithm<T> algorithm, int visit)
        {
            var originalIdx = algorithm.OriginalIndexOf(visit);
            var routerPoint = algorithm.RouterPoints[originalIdx];

            return routerPoint.LocationOnNetwork(algorithm.Router.Db);
        }

        /// <summary>
        /// Builds a route from a given tour.
        /// </summary>
        /// <returns></returns>
        public static Route BuildRoute<T>(this IWeightMatrixAlgorithm<T> algorithm, ITour tour)
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
        public static List<Result<Route>> TryBuildRoutes<T>(this IWeightMatrixAlgorithm<T> algorithm, Tour tour)
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
        public static List<Route> BuildRoutes<T>(this IWeightMatrixAlgorithm<T> algorithm, Tour tour)
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
        /// Builds the resulting route.
        /// </summary>
        /// <returns></returns>
        public static Route BuildRoute<T>(this IDirectedWeightMatrixAlgorithm<T> algorithm, Tour tour)
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
        public static List<Result<Route>> TryBuildRoutes<T>(this IDirectedWeightMatrixAlgorithm<T> algorithm, Tour tour)
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
        public static List<Route> BuildRoutes<T>(this IDirectedWeightMatrixAlgorithm<T> algorithm, Tour tour)
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