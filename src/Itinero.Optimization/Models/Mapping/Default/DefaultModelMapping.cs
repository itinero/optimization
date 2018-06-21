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
using Itinero.Algorithms.Matrices;
using Itinero.Algorithms.Search;

namespace Itinero.Optimization.Models.Mapping.Default
{
    /// <summary>
    /// A default mapping.
    /// </summary>
    internal class DefaultModelMapping : IModelMapping
    {
        private readonly RouterBase _router;
        private readonly WeightMatrixAlgorithm _weightMatrixAlgorithm;
        private readonly MassResolvingAlgorithm _massResolvingAlgorithm;

        public DefaultModelMapping(RouterBase router, MassResolvingAlgorithm massResolvingAlgorithm,
            WeightMatrixAlgorithm weightMatrixAlgorithm)
        {
            _router = router;
            _weightMatrixAlgorithm = weightMatrixAlgorithm;
            _massResolvingAlgorithm = massResolvingAlgorithm;
            
            // TODO: compose errors.
        }

        /// <summary>
        /// Returns the mapped visit given the original visit.
        /// </summary>
        /// <param name="originalVisit">The original visit index.</param>
        /// <returns>The mapped visit index if any.</returns>
        public int? MappedVisit(int? originalVisit)
        {
            
        }
        
        /// <inheritdoc />
        public Result<IEnumerable<Route>> BuildRoutes(IEnumerable<(int vehicle, IEnumerable<int> tour)> solution)
        {
            
        }

        /// <inheritdoc />
        public IEnumerable<(int visit, string message)> Errors { get; }
    }
}