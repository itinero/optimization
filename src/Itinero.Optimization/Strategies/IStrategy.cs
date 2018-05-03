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
    /// Abstract representation of a search strategy.
    /// </summary>
    public interface IStrategy<TProblem, TCandidate>
    {
        /// <summary>
        /// Gets the name of this strategy.
        /// </summary>
        /// <returns></returns>
        string Name { get; }

        /// <summary>
        /// Runs this strategy on the given problem and returns the best candidate.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <returns>A candidate.</returns>
        TCandidate Search(TProblem problem);

        /// <summary>
        /// A function called when a new candidate was found.
        /// </summary>
        Delegates.NewCandidate<TCandidate> IntermidiateResult
        {
            get;
            set;
        }
    }
}