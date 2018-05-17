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

namespace Itinero.Optimization.Strategies.GA
{
    /// <summary>
    /// Abstract representation of a selection strategy to select individuals.
    /// </summary>
    /// <typeparam name="TCandidate">The candidate type.</typeparam>
    public interface ISelector<in TCandidate>
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Selects a candidate from the given population.
        /// </summary>
        /// <param name="population">The population.</param>
        /// <param name="exclude">A function to ignore individuals.</param>
        /// <returns>The index of the selected candidate.</returns>
        int Select(TCandidate[] population, Func<int, bool> exclude);
    }
}