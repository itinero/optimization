﻿/*
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
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Itinero.Optimization.Solvers.Tours;

[assembly: InternalsVisibleTo("Itinero.Optimization.Tests")]
[assembly: InternalsVisibleTo("Itinero.Optimization.Tests.Benchmarks")]
namespace Itinero.Optimization.Solvers.Shared.CheapestInsertion
{
    /// <summary>
    /// Contains shared methods to do 'cheapest insertion' or 'best placement'. 
    /// </summary>
    internal static class CheapestInsertionHelper
    {
        /// <summary>
        /// Calculates the best position to insert a given visit.
        /// </summary>
        /// <param name="tour">The tour to insert into.</param>
        /// <param name="weightFunc">The function to get the travel weights.</param>
        /// <param name="visits">The visits to insert.</param>
        /// <param name="insertionCostHeuristic">The cost function, if any, to influence insertion cost for visits.</param>
        /// <param name="canPlace">A function to determine if a visit can be place given it's cost (use this to check constraints).</param>
        /// <returns>The increase/decrease in weight and the location.</returns>
        public static (float cost, Pair location, int visit) CalculateCheapest(this Tour tour, Func<int, int, float> weightFunc,
            IEnumerable<int> visits,
            Func<int, float> insertionCostHeuristic = null, Func<float, int, bool> canPlace = null)
        {
            (float cost, Pair location, int visit) best = (float.MaxValue, new Pair(int.MaxValue, int.MaxValue), -1);

            foreach (var visit in visits)
            {
                var insertDetails = tour.CalculateCheapest(weightFunc, visit, insertionCostHeuristic, canPlace);
                if (insertDetails.cost < best.cost)
                {
                    best = (insertDetails.cost, insertDetails.location, visit);
                }
            }

            return best;
        }

        /// <summary>
        /// Calculates the best position to insert a given visit.
        /// </summary>
        /// <param name="tour">The tour to insert into.</param>
        /// <param name="weightFunc">The function to get the travel weights.</param>
        /// <param name="visit">The visit insert.</param>
        /// <param name="insertionCostHeuristic">The cost function, if any, to influence insertion cost for visits.</param>
        /// <param name="canPlace">A function to determine if a visit can be place given it's cost (use this to check constraints).</param>
        /// <returns>The increase/decrease in weight and the location.</returns>
        public static (float cost, Pair location) CalculateCheapest(this Tour tour, Func<int, int, float> weightFunc, int visit,
            Func<int, float> insertionCostHeuristic = null, Func<float, int, bool> canPlace = null)
        {
            (float cost, Pair location) best = (float.MaxValue, new Pair(int.MaxValue, int.MaxValue));

            if (tour.Count == 1)
            {
                var first = tour.First;
                if (tour.IsClosed())
                {
                    var cost = (weightFunc(first, visit) +
                                weightFunc(visit, first));
                    if ((!(canPlace?.Invoke(cost, visit) ?? true))) return best;
                    
                    cost += (insertionCostHeuristic?.Invoke(visit) ?? 0);
                    var location = new Pair(first, first);
                    best = (cost, location);
                }
                else
                {
                    var cost = weightFunc(first, visit);
                    if ((!(canPlace?.Invoke(cost, visit) ?? true))) return best;
                    
                    cost += (insertionCostHeuristic?.Invoke(visit) ?? 0);
                    var location = new Pair(first, first);
                    best = (cost, location);
                }
            }
            else
            {
                foreach (var pair in tour.Pairs())
                {
                    var cost = weightFunc(pair.From, visit) +
                               weightFunc(visit, pair.To) -
                               weightFunc(pair.From, pair.To);
                    if ((!(canPlace?.Invoke(cost, visit) ?? true))) continue;
                    
                    cost += (insertionCostHeuristic?.Invoke(visit) ?? 0);
                    if (cost < best.cost)
                    {
                        best = (cost, pair);
                    }
                }
            }
            
            return best;
        }
    }
}