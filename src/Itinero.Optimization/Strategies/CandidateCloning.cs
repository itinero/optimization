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

namespace Itinero.Optimization.Strategies
{
    /// <summary>
    /// Contains functionality to clone candidates.
    /// </summary>
    internal static class CandidateCloning
    {
        /// <summary>
        /// Clones the given candidate using one of the default methods.
        /// </summary>
        /// <param name="candidate">The candidate to clone.</param>
        /// <typeparam name="TCandidate">The candidate type.</typeparam>
        /// <returns>A deep-copy of the given candidate.</returns>
        public static TCandidate Clone<TCandidate>(this TCandidate candidate)
            where TCandidate : class
        {
            switch (candidate)
            {
                case ICloneable<TCandidate> clonableTyped:
                    return clonableTyped.Clone();
                case ICloneable clonable:
                    return clonable.Clone() as TCandidate;
            }
            // TODO: we can provide perhaps other faster methods here via DI.
            throw new InvalidOperationException($"Cannot clone candidates, no cloning method found for candidates of type {typeof(TCandidate)}.");
        }
    }

    /// <summary>
    /// Abstract representation of a cloneable object.
    /// </summary>
    /// <typeparam name="TCandidate"></typeparam>
    public interface ICloneable<out TCandidate>
    {
        /// <summary>
        /// Returns clone, a deep-copy, of this object.
        /// </summary>
        /// <returns></returns>
        TCandidate Clone();
    }
}