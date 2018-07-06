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
using System.Linq;
using Itinero.Optimization.Solvers.Tours;

namespace Itinero.Optimization.Solvers.Shared.Seeds
{
    /// <summary>
    /// Contains seed heuristics.
    /// </summary>
    internal static class SeedHeuristics
    {
        /// <summary>
        /// Select a random seed from the given visits pool.
        /// </summary>
        /// <param name="visitPool">The pool of visits to choose from.</param>
        /// <returns>The selected visit or Constant.NO_SET if no visit could be selected. Guarantees a valid return if there are visits in the given pool.</returns>
        public static int GetSeedRandom(ICollection<int> visitPool)
        {
            if (visitPool.Count == 0)
            {
                return Tour.NOT_SET;
            }

            var pos = Strategies.Random.RandomGenerator.Default.Generate(visitPool.Count);
            return visitPool.ElementAt(pos);
        }
    }
}