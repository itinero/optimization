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

namespace Itinero.Optimization.Strategies.VNS
{
    /// <summary>
    /// Abstract definition of a perturber, an operator that tries to change search neighbourhoods for VNS search.
    /// </summary>
    public abstract class Perturber<TCandidate> : Operator<TCandidate>
        where TCandidate : class
    {
        /// <inheritdoc />
        public override bool Apply(TCandidate candidate)
        {
            return this.Apply(candidate, 0);
        }

        /// <summary>
        /// Applies this perturber to the given candidate.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="level">The level.</param>
        /// <returns>True if an improvement was found.</returns>
        /// <remarks>The candidate will be modified in-place .</remarks>
        public abstract bool Apply(TCandidate candidate, int level);

        /// <summary>
        /// Define an implicit type conversion from a function to a perturber instance.
        /// </summary>
        /// <param name="func">The function to use.</param>
        /// <returns>The perturber instance.</returns>
        public static implicit operator Perturber<TCandidate>(Func<TCandidate, int, bool> func)
        {
            return new FuncPerturber<TCandidate>(func);
        }

        /// <summary>
        /// Define an implicit type conversion from a function to an perturber instance.
        /// </summary>
        /// <param name="op">The perturber instance.</param>
        /// <returns>The function.</returns>
        public static implicit operator Func<TCandidate, int,  bool>(Perturber<TCandidate> op)
        {
            return op.Apply;
        }
    }
}