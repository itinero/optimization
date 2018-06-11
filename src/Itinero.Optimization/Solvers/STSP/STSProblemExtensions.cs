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

namespace Itinero.Optimization.Solvers.STSP
{
    /// <summary>
    /// Contains extension methods related to STSP.
    /// </summary>
    internal static class STSProblemExtensions
    {
        /// <summary>
        /// Creates an initial empty tour, add fixed first and/or last customer.
        /// </summary>
        /// <returns></returns>
        public static Tour CreateEmptyTour(this STSProblem problem)
        {
            if (!problem.Last.HasValue)
            {
                return new Tours.Tour(new int[] { problem.First }, null);
            }
            else
            {
                if (problem.Last == problem.First)
                {
                    return new Tours.Tour(new int[] { problem.First }, problem.First);
                }
                return new Tour(new int[] { problem.First, problem.Last.Value }, problem.Last);
            }
        }
    }
}