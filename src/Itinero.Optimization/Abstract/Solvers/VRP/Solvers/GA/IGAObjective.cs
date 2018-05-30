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

namespace Itinero.Optimization.Abstract.Solvers.VRP.Solvers.GA
{
    /// <summary>
    /// Abstract representation of an objective.
    /// </summary>
    /// <typeparam name="TProblem">The problem type.</typeparam>
    /// <typeparam name="TSolution">The solution type.</typeparam>
    public interface IGAObjective<in TProblem, TSolution>
    {
        /// <summary>
        /// Creates a new and empty solution.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <returns>A new empty solution.</returns>
        TSolution NewSolution(TProblem problem);

        /// <summary>
        /// Selects a good candidate tour to be place in the target tour from the source tour.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <returns>True if a good tour was found.</returns>
        bool SelectTour(TProblem problem, TSolution source, TSolution target);

        /// <summary>
        /// Place all the remaining unplaced visits in the given solution.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="solution">The solution.</param>
        void PlaceRemaining(TProblem problem, TSolution solution);

        /// <summary>
        /// Gets a list of unplaced visits.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="solution">The current solution if any.</param>
        /// <returns>The list of visits to be visited, except potentially those uniquely used as seeds.</returns>
        IList<int> PotentialVisits(TProblem problem, TSolution solution = default(TSolution));
    }
}