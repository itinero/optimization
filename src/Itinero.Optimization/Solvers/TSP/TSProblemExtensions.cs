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
using Itinero.Optimization.Solvers.Tours;

namespace Itinero.Optimization.Solvers.TSP
{
    /// <summary>
    /// Contains extension methods related to the TSP.
    /// </summary>
    internal static class TSProblemExtensions
    {
        /// <summary>
        /// Calculates the total weight for the given tour.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="tour">The tour.</param>
        /// <returns>Returns the weight.</returns>
        internal static float Weights(this TSProblem problem, IEnumerable<int> tour)
        {
            var weight = 0f;
            var previous = Tour.NOT_SET;
            foreach (var visit in tour)
            {
                if (previous == Tour.NOT_SET)
                {
                    previous = visit;
                    continue;
                }

                weight += problem.Weight(previous, visit);
                previous = visit;
            }

            return weight;
        }
    }
}