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

using Itinero.Optimization.Algorithms.NearestNeighbour;

namespace Itinero.Optimization.Abstract.Solvers.TSP
{
    /// <summary>
    /// A abstract representation of a TSP.
    /// </summary>
    public interface ITSProblem
    {
        /// <summary>
        /// Gets the weight between the two given visits.
        /// </summary>
        float Weight(int from, int to);

        /// <summary>
        /// Gets the first visit.
        /// </summary>
        int First { get; }

        /// <summary>
        /// Gets the last visit.
        /// </summary>
        int? Last { get; }

        /// <summary>
        /// Gets the # of visits.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Gets the nearest neighbour cache.
        /// </summary>
        NearestNeighbourCache NearestNeighbourCache
        {
            get;
        }
    }
}