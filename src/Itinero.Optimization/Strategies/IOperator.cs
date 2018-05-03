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

namespace Itinero.Optimization.Strategies
{
    /// <summary>
    /// Abstract definition of an operator.
    /// </summary>
    public interface IOperator<TCandidate>
        where TCandidate : class
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <returns></returns>
        string Name { get; }

        /// <summary>
        /// Applies this operator to the given candidate.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns>True if an improvement was found.</returns>
        /// <remarks>The candidate give should be modified in-place but this should *only* happen in the case when there is an improvement or this operator is explicitly used as a mutation operator.</remarks>
        bool Apply(TCandidate candidate);
    }

    /// <summary>
    /// An operator that does nothing.
    /// </summary>
    public class EmptyOperator<TCandidate> : IOperator<TCandidate>
        where TCandidate : class
    {
        /// <summary>
        /// Gets the name of this operator.
        /// </summary>
        /// <returns></returns>
        public string Name => "EMPTY";

        /// <summary>
        /// Applies this operator to the given candidate.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns>True if an improvement was found.</returns>
        public bool Apply(TCandidate candidate)
        {
            return false;
        }
    }
}