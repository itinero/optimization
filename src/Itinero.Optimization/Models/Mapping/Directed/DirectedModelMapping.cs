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

using System;
using System.Collections.Generic;
using System.Linq;
using Itinero.Algorithms;
using Itinero.Algorithms.Matrices;
using Itinero.Algorithms.Search;
using Itinero.Algorithms.Weights;
using Itinero.Data.Network;
using Itinero.Optimization.Solvers.Shared.Directed;

namespace Itinero.Optimization.Models.Mapping.Directed
{
    /// <summary>
    /// A default mapping.
    /// </summary>
    internal class DirectedModelMapping : IModelMapping
    {
        private readonly MappedModel _mappedModel;
        private readonly DirectedWeightMatrixAlgorithm _weightMatrixAlgorithm;

        public DirectedModelMapping(MappedModel mappedModel, DirectedWeightMatrixAlgorithm weightMatrixAlgorithm)
        {
            _mappedModel = mappedModel;
            _weightMatrixAlgorithm = weightMatrixAlgorithm;
        }
        
        /// <inheritdoc />
        public IEnumerable<Result<Route>> BuildRoutes(IEnumerable<(int vehicle, IEnumerable<int> tour)> solution)
        {
            foreach (var vehicleAndTour in solution)
            {
                yield return this.BuildRoute(vehicleAndTour);
            }
        }

        private Result<Route> BuildRoute((int vehicle, IEnumerable<int> tour) vehicleAndTour)
        {
            try
            {
                // TODO: we need a better more generic way of handling this. Current rules are:
                // - use vehicle departure and arrival to close a tour or not.
                // - close a tour by default when both departure and arrival are null.
                // PROBLEM: this doesn't support the case where we have a vehicle with arrival and departure null and we generate open-tours.
                // SUGGESTED FIX: look at the enumerable differently include all visits, even if that means closing the tour in the enumerable.
                var vehicle = _mappedModel.VehiclePool.Vehicles[vehicleAndTour.vehicle];
                
                Route route = null;
                var previous = -1;
                var first = -1;
                foreach (var v in vehicleAndTour.tour)
                {
                    if (first < 0)
                    {
                        first = v;
                    }
                    if (previous < 0)
                    {
                        previous = v;
                        continue;
                    }

                    var localResult = AppendRoute(route, previous, v);
                    if (localResult.IsError)
                    {
                        return localResult;
                    }

                    route = localResult.Value;
                    previous = v;
                }

                if (vehicle.Arrival.HasValue &&
                    vehicle.Departure.HasValue &&
                    vehicle.Arrival == vehicle.Departure &&
                    previous != 0)
                {
                    var localResult = AppendRoute(route, previous, vehicle.Departure.Value);
                    if (localResult.IsError)
                    {
                        return localResult;
                    }
                    
                    route = localResult.Value;
                }

                if (!vehicle.Arrival.HasValue &&
                    !vehicle.Departure.HasValue &&
                    previous != 0)
                {
                    var localResult = AppendRoute(route, previous, first);
                    if (localResult.IsError)
                    {
                        return localResult;
                    }
                    
                    route = localResult.Value;
                }
                
                return new Result<Route>(route);
            }
            catch (Exception e)
            {
                return new Result<Route>(e.Message);
            }
        }

        private Result<Route> AppendRoute(Route route, int directedVisit1, int directedVisit2)
        {
            var weightHandler = _weightMatrixAlgorithm.Profile.DefaultWeightHandler(_weightMatrixAlgorithm.Router);
            var pairFromDepartureId = _weightMatrixAlgorithm.SourcePaths[DirectedHelper.WeightIdDeparture(directedVisit1)];
            var pairToArrivalId = _weightMatrixAlgorithm.TargetPaths[DirectedHelper.WeightIdArrival(directedVisit2)];
                
            if (pairFromDepartureId == null)
            {
                return new Result<Route>($"No source path found at departure id for {directedVisit1}->{directedVisit2}.");
            }                
            if (pairToArrivalId == null)
            {
                return new Result<Route>($"No target path found at arrival id for {directedVisit1}->{directedVisit2}.");
            }
            
            var pairFromEdgeId = _weightMatrixAlgorithm.Router.Db.Network.GetEdges(pairFromDepartureId.From.Vertex).First(x => x.To == pairFromDepartureId.Vertex).IdDirected();
            var pairToEdgeId = _weightMatrixAlgorithm.Router.Db.Network.GetEdges(pairToArrivalId.Vertex).First(x => x.To == pairToArrivalId.From.Vertex).IdDirected();

            var pairFromId = DirectedHelper.ExtractVisit(directedVisit1);
            var pairToId = DirectedHelper.ExtractVisit(directedVisit2);
            
            var fromRouterPoint = _weightMatrixAlgorithm.RouterPoints[pairFromId];
            var toRouterPoint = _weightMatrixAlgorithm.RouterPoints[pairToId];
            
            var localRouteRawResult = _weightMatrixAlgorithm.Router.TryCalculateRaw(_weightMatrixAlgorithm.Profile, weightHandler, pairFromEdgeId, pairToEdgeId, null);
            if (localRouteRawResult.IsError)
            {
                return localRouteRawResult.ConvertError<Route>();
            }

            var localRouteRaw = localRouteRawResult.Value;
            localRouteRaw.StripSource();
            localRouteRaw.StripTarget();

            var localRoute = _weightMatrixAlgorithm.Router.BuildRoute(_weightMatrixAlgorithm.Profile, weightHandler, fromRouterPoint, toRouterPoint, localRouteRaw);
            
            route = route == null ? localRoute.Value : route.Concatenate(localRoute.Value);
            return new Result<Route>(route);
        }

        /// <inheritdoc />
        public IEnumerable<(int visit, string message)> Errors
        {
            get
            {
                if (_weightMatrixAlgorithm.Errors != null)
                {
                    foreach (var error in _weightMatrixAlgorithm.Errors)
                    {
                        yield return (error.Key, $"{error.Value.Code} - {error.Value.Message}");
                    }
                }
            }
        }
    }
}