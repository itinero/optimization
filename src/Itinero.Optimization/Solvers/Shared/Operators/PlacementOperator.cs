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
using Itinero.Optimization.Strategies;

namespace Itinero.Optimization.Solvers.Shared.Operators
{
    /// <summary>
    /// An operator that can be used to place unplaced visits in a candidate solution. 
    /// </summary>
    /// <typeparam name="TCandidate"></typeparam>
    public abstract class PlacementOperator<TCandidate> : Operator<TCandidate>
        where TCandidate : class
    {
        /// <summary>
        /// Applies this operator to the given candidate, removes the visits that have been place from the visits collection given.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="visits">The visits to place.</param>
        /// <returns>True if at least one visit was placed.</returns>
        /// <remarks>The candidate will be modified in-place but this should *only* happen in the case when there is an improvement or this operator is explicitly used as a mutation operator.</remarks>
        public abstract bool Apply(TCandidate candidate, ICollection<int> visits);
    }
}