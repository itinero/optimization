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

namespace Itinero.Optimization.Solvers.CVRP_ND
{
    /// <summary>
    /// Represents a solution to the CVRP no-depot.
    /// </summary>
    internal class CVRPNDSolution
    {
        private readonly List<Tour> _tours;

        /// <summary>
        /// Creates a new solution.
        /// </summary>
        public CVRPNDSolution()
        {
            _tours = new List<Tour>();
        }

        /// <summary>
        /// Adds a new tour.
        /// </summary>
        /// <param name="first">The first visit.</param>
        /// <param name="last">The last visit if any.</param>
        /// <returns>The index of the new tour.</returns>
        public int AddNew(int first, int? last)
        {
            if (last.HasValue)
            {
                _tours.Add(new Tour(new[] { first, last.Value}, last));
            }
            else
            {
                _tours.Add(new Tour(new[] { first }, null));
            }
            return _tours.Count - 1;
        }

        /// <summary>
        /// Updates the first visit in the given tour to use a new first.
        /// </summary>
        /// <param name="t">The tour.</param>
        /// <param name="first">The new first.</param>
        public void UpdateFirst(int t, int first)
        {
            _tours[t] = new Tour(_tours[t].From(first), first);
        }

        /// <summary>
        /// Removes the tour at the given index.
        /// </summary>
        /// <param name="t">The tour index.</param>
        public void Remove(int t)
        {
            _tours.RemoveAt(t);
        }

        /// <summary>
        /// Gets the tour at the given index.
        /// </summary>
        /// <param name="t">The tour index.</param>
        /// <returns>The tour.</returns>
        public Tour Tour(int t)
        {
            return _tours[t];
        }

        /// <summary>
        /// The number of tours in this solution.
        /// </summary>
        public int Count => _tours.Count;
    }
}