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
using Itinero.Algorithms.Matrices;
using Itinero.Algorithms.Search;

namespace Itinero.Optimization.Models.Mapping.Default
{
    /// <summary>
    /// A default mapping.
    /// </summary>
    internal class DefaultModelMapping : IModelMapping
    {
        private readonly MappedModel _mappedModel;
        private readonly WeightMatrixAlgorithm _weightMatrixAlgorithm;

        public DefaultModelMapping(MappedModel mappedModel, WeightMatrixAlgorithm weightMatrixAlgorithm)
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
                var index = -1;
                var previous = -1;
                var first = -1;
                foreach (var v in vehicleAndTour.tour)
                {
                    index++;

                    if (first < 0)
                    {
                        first = v;
                    }
                    if (previous < 0)
                    {
                        previous = v;
                        continue;
                    }

                    var localResult = AppendRoute(route, index - 1, previous, index, v);
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
                    var localResult = AppendRoute(route, index, previous, 0, vehicle.Departure.Value);
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
                    var localResult = AppendRoute(route, index, previous, 0, first);
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

        private Result<Route> AppendRoute(Route route, int visit1Index, int visit1, int visit2Index, int visit2)
        {
            var visit1RouterPoint = _weightMatrixAlgorithm.OriginalIndexOf(visit1);
            var visit2RouterPoint = _weightMatrixAlgorithm.OriginalIndexOf(visit2);
            var routerPoint1 = _weightMatrixAlgorithm.RouterPoints[visit1RouterPoint];
            var routerPoint2 = _weightMatrixAlgorithm.RouterPoints[visit2RouterPoint];
            var localRouteResult = _weightMatrixAlgorithm.Router.TryCalculate(_weightMatrixAlgorithm.Profile,
                routerPoint1, routerPoint2);
            if (localRouteResult.IsError)
            {
                var originalVisit1 = _weightMatrixAlgorithm.OriginalLocationIndex(visit1);
                var originalVisit2 = _weightMatrixAlgorithm.OriginalLocationIndex(visit2);
                return new Result<Route>(
                    $"Route could not be calculated between visit {visit1}->{visit2} " +
                    $"between routerpoints {visit1RouterPoint}({routerPoint1})->{visit2RouterPoint}({routerPoint2})");
            }
            var localRoute = localRouteResult.Value;

            if (localRoute.Stops != null &&
                localRoute.Stops.Length == 2)
            {
                var visit1Stop = localRoute.Stops[0];
                var visit2Stop = localRoute.Stops[1];

                // add visit1 costs & order to the stop meta data.
                visit1Stop.Attributes.AddOrReplace("order", visit1Index.ToInvariantString());
                var visit1Costs = _mappedModel.Visits[visit1].VisitCosts;
                if (visit1Costs != null)
                {
                    foreach (var visitCost in visit1Costs)
                    {
                        visit1Stop.Attributes.AddOrReplace("cost_" + visitCost.Metric.ToLowerInvariant(), visitCost.Value.ToInvariantString());
                    }

                    if (route == null)
                    { // this is the first route, update visit1.
                        var visit1Cost = visit1Costs.FirstOrDefault(x => x.Metric == Metrics.Time)?.Value;
                        if (visit1Cost.HasValue)
                        {
                            if (localRoute.ShapeMeta != null &&
                                localRoute.ShapeMeta.Length > 0)
                            {
                                for (var sm = 0; sm < localRoute.ShapeMeta.Length; sm++)
                                {
                                    localRoute.ShapeMeta[sm].Time += visit1Cost.Value;
                                }
                            }
                            localRoute.TotalTime += visit1Cost.Value;
                        }
                    }
                }

                // add visit2 costs & order to the stop meta data.
                visit2Stop.Attributes.AddOrReplace("order", visit2Index.ToInvariantString());
                var visit2Costs = _mappedModel.Visits[visit2].VisitCosts;
                if (visit2Costs != null)
                {
                    foreach (var visitCost in visit2Costs)
                    {
                        visit2Stop.Attributes.AddOrReplace("cost_" + visitCost.Metric.ToLowerInvariant(), visitCost.Value.ToInvariantString());
                    }

                    // if there is a cost with travel time also add the travel time.
                    var visit2Cost = visit2Costs.FirstOrDefault(x => x.Metric == Metrics.Time)?.Value;
                    if (visit2Cost.HasValue)
                    { // this is the second stop, add the travel time at the end.
                        if (localRoute.ShapeMeta != null &&
                            localRoute.ShapeMeta.Length > 0)
                        {
                            localRoute.ShapeMeta[localRoute.ShapeMeta.Length - 1].Time += visit2Cost.Value;
                        }
                        localRoute.TotalTime += visit2Cost.Value;
                    }
                }
            }

            route = route == null ? localRoute : route.Concatenate(localRoute);
            
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