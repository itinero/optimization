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
    /// Contains some generic delegates used by some of the strategies.
    /// </summary>
    public static class Delegates
    {
        /// <summary>
        /// Delegate used to report on a new candidate. When false is returned the search is supposed to be stopped.
        /// </summary>
        /// <param name="candidate">The new candidate.</param>
        /// <returns>True if the search has to continue, false otherwise.</returns>
        public delegate bool NewCandidate<in TCandidate>(TCandidate candidate);
        
        /// <summary>
        /// Delegate used as a stop condition.
        /// </summary>
        /// <param name="candidate">The current candidate.</param>
        /// <param name="iteration">The iteration count.</param>
        /// <param name="level">The level.</param>
        /// <returns>True if the search has to stop.</returns>
        public delegate bool StopConditionDelegate<in TCandidate>(TCandidate candidate, int iteration, int level);
    }
}