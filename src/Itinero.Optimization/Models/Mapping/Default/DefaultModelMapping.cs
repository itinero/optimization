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
                return new Result<Route>("Not implemented");
            }
            catch (Exception e)
            {
                return new Result<Route>(e.Message);
            }
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