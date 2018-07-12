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

using Itinero.Optimization.Strategies;

namespace Itinero.Optimization.Solvers.CVRP_ND.Operators
{
    /// <summary>
    /// An operator that is also aware of the candidate structure having tours.
    /// </summary>
    internal abstract class TourAwareOperator : Operator<CVRPNDCandidate>
    {
        /// <summary>
        /// Applies this operator to the given candidate.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="t">The tour to focus on.</param>
        /// <returns>True if an improvement was found.</returns>
        /// <remarks>The candidate will be modified in-place but this should *only* happen in the case when there is an improvement or this operator is explicitly used as a mutation operator.</remarks>
        public abstract bool Apply(CVRPNDCandidate candidate, int t);
        
        /// <summary>
        /// Applies this operator to the given candidate.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="t1">The first tour to focus on.</param>
        /// <param name="t2">The second tour to focus on.</param>
        /// <returns>True if an improvement was found.</returns>
        /// <remarks>The candidate will be modified in-place but this should *only* happen in the case when there is an improvement or this operator is explicitly used as a mutation operator.</remarks>
        public abstract bool Apply(CVRPNDCandidate candidate, int t1, int t2);
    }
}