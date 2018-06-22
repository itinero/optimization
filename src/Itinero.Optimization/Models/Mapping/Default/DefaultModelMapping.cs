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
                Route route = null;
                var previous = -1;
                foreach (var v in vehicleAndTour.tour)
                {
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
                return new Result<Route>(route);
            }
            catch (Exception e)
            {
                return new Result<Route>(e.Message);
            }
        }

        private Result<Route> AppendRoute(Route route, int visit1, int visit2)
        {
            var visit1RouterPoint = _weightMatrixAlgorithm.OriginalIndexOf(visit1);
            var visit2RouterPoint = _weightMatrixAlgorithm.OriginalIndexOf(visit2);
            var routerPoint1 = _weightMatrixAlgorithm.RouterPoints[visit1RouterPoint];
            var routerPoint2 = _weightMatrixAlgorithm.RouterPoints[visit2RouterPoint];
            var localRoute = _weightMatrixAlgorithm.Router.TryCalculate(_weightMatrixAlgorithm.Profile,
                routerPoint1, routerPoint2);
            if (localRoute.IsError)
            {
                var originalVisit1 = _weightMatrixAlgorithm.OriginalLocationIndex(visit1);
                var originalVisit2 = _weightMatrixAlgorithm.OriginalLocationIndex(visit2);
                return new Result<Route>(
                    $"Route could not be calculated between visit {visit1}->{visit2} " +
                    $"between routerpoints {visit1RouterPoint}({routerPoint1})->{visit2RouterPoint}({routerPoint2})");
            }

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