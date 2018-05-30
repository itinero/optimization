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

using System.Collections.Generic;
using System.Linq;
using Itinero.Algorithms;
using Itinero.Algorithms.Matrices;
using Itinero.Algorithms.Weights;
using Itinero.Data.Network;
using Itinero.LocalGeo;
using Itinero.Optimization.Abstract.Tours;
using Itinero.Optimization.Algorithms.Directed;
using Itinero.Attributes;
using System;
using Itinero.Logging;

namespace Itinero.Optimization.Models.Mapping
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
                var visitLocation = algorithm.LocationOnNetwork(visit);
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
            //var originalIdx = algorithm.OriginalIndexOf(visit);
            var routerPoint = algorithm.RouterPoints[visit];

            return routerPoint.LocationOnNetwork(algorithm.Router.Db);
        }

        /// <summary>
        /// Returns true if the boundingboxes of the two tours overlap.
        /// </summary>
        /// <param name="algorithm">The weight matrix algorithm.</param>
        /// <param name="tour1">The first tour.</param>
        /// <param name="tour2">The second tour.</param>
        /// <returns></returns>
        public static bool ToursOverlap<T>(this IDirectedWeightMatrixAlgorithm<T> algorithm, ITour tour1, ITour tour2)
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
        public static Box BoundingBox<T>(this IDirectedWeightMatrixAlgorithm<T> algorithm, ITour tour)
        {
            Box? box = null;
            foreach (var visit in tour)
            {
                var visitLocation = algorithm.LocationOnNetwork(visit);
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
        public static Coordinate LocationOnNetwork<T>(this IDirectedWeightMatrixAlgorithm<T> algorithm, int visit)
        {
            var originalIdx = algorithm.OriginalIndexOf(visit);
            var routerPoint = algorithm.RouterPoints[originalIdx];

            return routerPoint.LocationOnNetwork(algorithm.Router.Db);
        }

        /// <summary>
        /// Builds a route from a given tour.
        /// </summary>
        /// <returns></returns>
        public static Route BuildRoute<T>(this IWeightMatrixAlgorithm<T> algorithm, ITour tour, 
            Func<int, IAttributeCollection, float> customizeVisit = null)
        {
            // TODO: what about costs and tours that are closed?
            Route route = null;
            foreach (var pair in tour.Pairs())
            {
                var localRoute = algorithm.Router.Calculate(algorithm.Profile, algorithm.RouterPoints[pair.From],
                    algorithm.RouterPoints[pair.To]);
                if (localRoute.Stops != null &&
                    localRoute.Stops.Length == 2 &&
                    customizeVisit != null)
                { 
                    // customize stops that represent visits.

                    // include first extra time because it's not already included in the previous section.
                    var extraTime = customizeVisit(pair.From, localRoute.Stops[0].Attributes);
                    if (route == null)
                    { 
                        if (localRoute.ShapeMeta != null &&
                            localRoute.ShapeMeta.Length > 0)
                        {
                            for (var sm = 0; sm < localRoute.ShapeMeta.Length; sm++)
                            {
                                localRoute.ShapeMeta[sm].Time += extraTime;
                            }
                        }
                        localRoute.TotalTime += extraTime;
                    }

                    // add any extra travel time for the last stop.
                    extraTime = customizeVisit(pair.To, localRoute.Stops[1].Attributes);
                    if (extraTime > 0)
                    {
                        if (localRoute.ShapeMeta != null &&
                            localRoute.ShapeMeta.Length > 0)
                        {
                            localRoute.ShapeMeta[localRoute.ShapeMeta.Length - 1].Time += extraTime;
                        }
                        localRoute.TotalTime += extraTime;
                    }
                }

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
        /// Builds routes from a given multitour.
        /// </summary>
        /// <returns></returns>
        public static List<Route> BuildRoutes<T>(this IWeightMatrixAlgorithm<T> algorithm, IMultiTour tours)
        {
            var routes = new List<Route>();
            for (var t = 0; t < tours.Count; t++)
            {
                routes.Add(algorithm.BuildRoute(tours.Tour(t)));
            }
            return routes;
        }

        /// <summary>
        /// Builds the routes in segments divided by routes between customers in the given tour.
        /// </summary>
        /// <returns></returns>
        public static List<Result<Route>> TryBuildRoutes<T>(this IWeightMatrixAlgorithm<T> algorithm, ITour tour)
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
        public static List<Route> BuildRoutes<T>(this IWeightMatrixAlgorithm<T> algorithm, ITour tour)
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
        public static Route BuildRoute<T>(this IDirectedWeightMatrixAlgorithm<T> algorithm, ITour tour)
        {
            Route route = null;
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

                var localRoute = algorithm.Router.BuildRoute(algorithm.Profile, weightHandler, fromRouterPoint, toRouterPoint, localRouteRaw).Value;
                if (localRoute.Stops != null &&
                    localRoute.Stops.Length == 2)
                {
                    localRoute.Stops[0].Attributes.AddOrReplace("stop",
                        pair.From.ToInvariantString());
                    localRoute.Stops[1].Attributes.AddOrReplace("stop",
                        pair.To.ToInvariantString());
                }

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
        public static List<Result<Route>> TryBuildRoutes<T>(this IDirectedWeightMatrixAlgorithm<T> algorithm, ITour tour)
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
        public static List<Route> BuildRoutes<T>(this IDirectedWeightMatrixAlgorithm<T> algorithm, ITour tour)
        {
            var routes = new List<Route>();
            // TODO: check what to do here, use the cached version or not?
            var weightHandler = algorithm.Profile.DefaultWeightHandler(algorithm.Router);
            foreach (var pair in tour.Pairs())
            {
                // TODO: extract more info at once!
                var pairFromDepartureId = algorithm.SourcePaths[DirectedHelper.ExtractDepartureId(pair.From)];
                var pairToArrivalId = algorithm.TargetPaths[DirectedHelper.ExtractArrivalId(pair.To)];
                
                if (pairFromDepartureId == null)
                {
                    Itinero.Logging.Logger.Log("WeightMatrixExtensions", TraceEventType.Warning, 
                        $"No source path found at departure id for {pair.From}->{pair.To}, returning an empty route.");
                    routes.Add(null);
                    continue;
                }                
                if (pairToArrivalId == null)
                {
                    Itinero.Logging.Logger.Log("WeightMatrixExtensions", TraceEventType.Warning, 
                        $"No target path found at arrival id for {pair.From}->{pair.To}, returning an empty route.");
                    routes.Add(null);
                    continue;
                }

                var pairFromEdgeId = algorithm.Router.Db.Network.GetEdges(pairFromDepartureId.From.Vertex).First(x => x.To == pairFromDepartureId.Vertex).IdDirected();
                var pairToEdgeId = algorithm.Router.Db.Network.GetEdges(pairToArrivalId.Vertex).First(x => x.To == pairToArrivalId.From.Vertex).IdDirected();

                var pairFromId = DirectedHelper.ExtractId(pair.From);
                var pairToId = DirectedHelper.ExtractId(pair.To);

                var fromRouterPoint = algorithm.RouterPoints[pairFromId];
                var toRouterPoint = algorithm.RouterPoints[pairToId];

                var localRouteRawResult = algorithm.Router.TryCalculateRaw(algorithm.Profile, weightHandler, pairFromEdgeId, pairToEdgeId, null);
                if (localRouteRawResult.IsError)
                {
                    Itinero.Logging.Logger.Log("WeightMatrixExtensions", TraceEventType.Warning, $"Route was not found between {pair.From}->{pair.To}, returning an empty route.");
                    routes.Add(null);
                    continue;
                }

                var localRouteRaw = localRouteRawResult.Value;
                localRouteRaw.StripSource();
                localRouteRaw.StripTarget();

                var localRoute = algorithm.Router.BuildRoute(algorithm.Profile, weightHandler, fromRouterPoint, toRouterPoint, localRouteRaw);
                if (localRoute.IsError)
                {
                    throw new Itinero.Exceptions.RouteNotFoundException(
                        $"Part of the tour was not found: {pair.From}[{pairFromId}] -> {pair.To}[{pairToId}] - {localRoute.ErrorMessage}.");
                }
                routes.Add(localRoute.Value);
            }
            return routes;
        }

        /// <summary>
        /// Adjusts the given array of weights to an array that matches the calculate weight matrix.
        /// </summary>
        /// <param name="algorithm">The weight matrix.</param>
        /// <param name="weights">The weights.</param>
        /// <returns>Weights adjusted to the weight matrix, exclusing unresolvable or unroutable locations.</returns>
        public static W[] AdjustToMatrix<T, W>(this IWeightMatrixAlgorithm<T> algorithm, W[] weights)
        {
            var newWeights = new W[algorithm.Weights.Length];
            for (var i = 0; i < newWeights.Length; i++)
            {
                newWeights[i] = weights[algorithm.OriginalLocationIndex(i)];
            }
            return newWeights;
        }

        /// <summary>
        /// Returns an adjusted version of the given array to an array that matches the calculated weight matrix.
        /// </summary>
        /// <param name="algorithm">The weight matrix.</param>
        /// <param name="a">The orginal array.</param>
        /// <param name="convert">The conversion function, a function applied to every element.</param>
        /// <returns>Array adjusted to the weight matrix, exclusing unresolvable or unroutable locations.</returns>
        public static B[] AdjustToMatrix<T, A, B>(this IWeightMatrixAlgorithm<T> algorithm, A[] a,
            System.Func<A, B> convert)
        {
            var newWeights = new B[algorithm.Weights.Length];
            for (var i = 0; i < newWeights.Length; i++)
            {
                newWeights[i] = convert(a[algorithm.OriginalLocationIndex(i)]);
            }
            return newWeights;
        }

        /// <summary>
        /// Adjusts the given array of weights to an array that matches the calculate weight matrix.
        /// </summary>
        /// <param name="algorithm">The weight matrix.</param>
        /// <param name="weights">The weights.</param>
        /// <returns>Weights adjusted to the weight matrix, exclusing unresolvable or unroutable locations.</returns>
        public static W[] AdjustToMatrix<T, W>(this IDirectedWeightMatrixAlgorithm<T> algorithm, W[] weights)
        {
            var newWeights = new W[algorithm.Weights.Length / 2];
            for (var i = 0; i < newWeights.Length; i++)
            {
                newWeights[i] = weights[algorithm.OriginalLocationIndex(i)];
            }
            return newWeights;
        }

        /// <summary>
        /// Builds a travel cost matrix from the given weight matrix.
        /// </summary>
        /// <param name="weightMatrix">The weight matrix.</param>
        /// <returns></returns>
        public static Abstract.Models.Costs.TravelCostMatrix BuildTravelCostMatrix(this IWeightMatrixAlgorithm<float> weightMatrix)
        {
            return new Abstract.Models.Costs.TravelCostMatrix()
            {
                Directed = false,
                    Costs = weightMatrix.Weights,
                    Name = weightMatrix.Profile.Profile.Metric.ToModelMetric()
            };
        }

        /// <summary>
        /// Returns the weight index of the given location if any.
        /// </summary>
        /// <param name="weightMatrix">The weight matrix.</param>
        /// <param name="locationIdx">The original location index.</param>
        /// <returns></returns>
        public static int? WeightIndexNullable<T>(this IWeightMatrixAlgorithm<T> weightMatrix, int? locationIdx)
        {
            if (!locationIdx.HasValue)
            {
                return null;
            }
            return weightMatrix.WeightIndex(locationIdx.Value);
        }
    }
}