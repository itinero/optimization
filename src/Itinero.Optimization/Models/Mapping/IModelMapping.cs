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

namespace Itinero.Optimization.Models.Mapping
{
    /// <summary>
    /// Represents the link between a model and the actual routing network.
    /// </summary>
    public interface IModelMapping
    {
        /// <summary>
        /// Converts a solution to the mapped model into routes.
        /// </summary>
        /// <param name="solution">The solution.</param>
        /// <returns>The routes that are represented by the solution.</returns>
        IEnumerable<Result<Route>> BuildRoutes(IEnumerable<(int vehicle, IEnumerable<int> tour)> solution);

        internal RouterPoint GetVisitSnapping(int visit);

        internal RouterDb RouterDb { get; }
        
        /// <summary>
        /// Gets errors in the mapping if any.
        /// </summary>
        IEnumerable<(int visit, string message)> Errors { get; }
    }
}