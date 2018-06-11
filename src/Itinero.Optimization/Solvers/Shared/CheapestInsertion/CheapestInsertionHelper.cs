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
using Itinero.Optimization.Solvers.Tours;

namespace Itinero.Optimization.Solvers.Shared.CheapestInsertion
{
    /// <summary>
    /// Contains shared methods to do 'cheapest insertion' or 'best placement'. 
    /// </summary>
    public static class CheapestInsertionHelper
    {
        /// <summary>
        /// Calculates the best position to insert a given visit.
        /// </summary>
        /// <param name="tour">The tour to insert into.</param>
        /// <param name="weightFunc">The function to get weights.</param>
        /// <param name="visit">The visit insert.</param>
        /// <param name="location">The cheapest location to insert.</param>
        /// <returns>The increase/decrease in weight.</returns>
        public static float CalculateCheapest(this Tour tour, Func<int, int, float> weightFunc, int visit, out Pair location)
        {
            var bestCost = float.MaxValue;
            location = new Pair(int.MaxValue, int.MaxValue);

            if (tour.Count == 1)
            {
                var first = tour.First;
                if (tour.IsClosed())
                {
                    bestCost = weightFunc(first, visit) +
                               weightFunc(visit, first);
                    location = new Pair(first, first);
                }
                else
                {
                    bestCost = weightFunc(first, visit);
                    location = new Pair(first, int.MaxValue);
                }
            }
            else
            {
                foreach (var pair in tour.Pairs())
                {
                    var cost = weightFunc(pair.From, visit) +
                               weightFunc(visit, pair.To) -
                               weightFunc(pair.From, pair.To);
                    if (cost < bestCost)
                    {
                        bestCost = cost;
                        location = pair;
                    }
                }
            }
            return bestCost;
        }
    }
}