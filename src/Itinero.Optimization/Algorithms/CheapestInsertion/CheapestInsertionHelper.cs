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

using Itinero.Optimization.Tours;

namespace Itinero.Optimization.Algorithms.CheapestInsertion
{
    /// <summary>
    /// Contains extension methods to do cheapest insertion.
    /// </summary>
    public static class CheapestInsertionHelper
    {
        /// <summary>
        /// Inserts the given visit at the best location.
        /// </summary>
        /// <param name="tour">The tour to insert into.</param>
        /// <param name="weights">The weights between visits.</param>
        /// <param name="visit">The visit insert.</param>
        public static void InsertCheapest(this ITour tour, float[][] weights, int visit)
        {
            Pair bestLocation;
            if (tour.CalculateCheapest(weights, visit, out bestLocation) < float.MaxValue)
            {
                tour.InsertAfter(bestLocation.From, visit);
            }
        }

        /// <summary>
        /// Calculates the best position to insert a given visit.
        /// </summary>
        /// <param name="tour">The tour to insert into.</param>
        /// <param name="weights">The weights between visits.</param>
        /// <param name="visit">The visit insert.</param>
        /// <param name="location">The cheapest location to insert.</param>
        /// <returns>The increase/decrease in weight.</returns>
        public static float CalculateCheapest(this ITour tour, float[][] weights, int visit, out Pair location)
        {
            var bestCost = float.MaxValue;
            location = new Pair(int.MaxValue, int.MaxValue);

            if (tour.Count == 1)
            {
                var first = tour.First;
                if (tour.IsClosed())
                {
                    bestCost = weights[first][visit] +
                            weights[visit][first];
                    location = new Pair(first, first);
                }
                else
                {
                    bestCost = weights[first][visit];
                    location = new Pair(first, int.MaxValue);
                }
            }
            else
            {
                foreach (var pair in tour.Pairs())
                {
                    var cost = weights[pair.From][visit] +
                        weights[visit][pair.To] -
                        weights[pair.From][pair.To];
                    if (cost < bestCost)
                    {
                        bestCost = cost;
                        location = pair;
                    }
                }
            }
            return bestCost;
        }

        /// <summary>
        /// Calculates the cheapest location to insert a sub-path that consists of (from) -> ... -> (to).
        /// </summary>
        /// <param name="tour">The tour to insert into.</param>
        /// <param name="weights">The weights between visits.</param>
        /// <param name="from">The first visit in the path to insert.</param>
        /// <param name="to">The last visit in the path to insert.</param>
        /// <param name="location">The cheapest location to insert.</param>
        /// <returns>The increase/decrease in weight not including the weight of the to be inserted sub-path.</returns>
        public static float CalculateCheapest(this ITour tour, float[][] weights, int from, int to, out Pair location)
        {
            var bestCost = float.MaxValue;
            location = new Pair(int.MaxValue, int.MaxValue);

            if (tour.Count == 1)
            {
                var first = tour.First;
                if (tour.IsClosed())
                {
                    bestCost = weights[first][from] +
                            weights[to][first];
                    location = new Pair(first, first);
                }
                else
                {
                    bestCost = weights[first][from];
                    location = new Pair(first, int.MaxValue);
                }
            }
            else
            {
                foreach (var pair in tour.Pairs())
                {
                    var cost = weights[pair.From][from] +
                        weights[to][pair.To] -
                        weights[pair.From][pair.To];
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
