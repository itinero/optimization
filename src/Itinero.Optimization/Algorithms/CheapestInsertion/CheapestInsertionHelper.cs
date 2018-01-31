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
        /// Calculates the best position to insert any of the given visits.
        /// </summary>
        /// <param name="tour">The tour to insert into.</param>
        /// <param name="weights">The weights between visits.</param>
        /// <param name="visits">The visits to potentially insert.</param>
        /// <param name="location">The cheapest location to insert.</param>
        /// <param name="visit">The cheapest visit to insert.</param>
        /// <returns>The increase/decrease in weight.</returns>
        public static float CalculateCheapestAny(this ITour tour, float[][] weights, IEnumerable<int> visits, 
            out Pair location, out int visit)
        {
            // TODO: test performance if we turn this loop around, now it's tour and inner is target visits.
            var bestCost = float.MaxValue;
            location = new Pair(Constants.NOT_SET, Constants.NOT_SET);
            visit = Constants.NOT_SET;

            if (tour.Count == 1)
            {
                var first = tour.First;
                if (tour.IsClosed())
                {
                    visit = weights.CalculateCheapest(first, first, visits, out bestCost);
                    location = new Pair(first, first);
                }
                else
                {
                    visit = weights.CalculateCheapestTo(first, visits, out bestCost);
                    location = new Pair(first, int.MaxValue);
                }
            }
            else
            {
                foreach (var pair in tour.Pairs())
                {
                    float cost;
                    var localVisit = weights.CalculateCheapest(pair.From, pair.To, visits, out cost);
                    cost -= weights[pair.From][pair.To];
                    if (cost < bestCost)
                    {
                        bestCost = cost;
                        visit = localVisit;
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

        /// <summary>
        /// Returns the cheapest target to reach from the given source visit.
        /// </summary>
        /// <param name="weights">The weights between all visits.</param>
        /// <param name="source">The source visit.</param>
        /// <param name="targets">The target visits.</param>
        /// <param name="weight">The weight to the found target.</param>
        /// <returns>The cheapest target to reach from the given source visit.</returns>
        public static int CalculateCheapestTo(this float[][] weights, int source, IEnumerable<int> targets,
            out float weight)
        {
            return weights.CalculateCheapest(source, Constants.NOT_SET, targets, out weight);
        }

        /// <summary>
        /// Returns the cheapest source visit to reach the given target.
        /// </summary>
        /// <param name="weights">The weights between all visits.</param>
        /// <param name="target">The target visit.</param>
        /// <param name="sources">The source visits.</param>
        /// <param name="weight">The weight to the found target.</param>
        /// <returns>The cheapest target to reach from the given source visit.</returns>
        public static int CalculateCheapestFrom(this float[][] weights, int target, IEnumerable<int> sources,
            out float weight)
        {
            return weights.CalculateCheapest(Constants.NOT_SET, target, sources, out weight);
        }

        /// <summary>
        /// Returns the cheapest visit the complete the sequence source->visit->target from the visits given.
        /// </summary>
        /// <param name="weights">The weights between all visits.</param>
        /// <param name="source">The source visit, ignored if Constants.NOT_SET.</param>
        /// <param name="target">The target visit, ignored if Constants.NOT_SET.</param>
        /// <param name="visits">The visits to consider.</param>
        /// <param name="weight">The weight of the path of the cheapest visit, source->visit->target. The weight between source->target is NOT subtracted.</param>
        /// <returns>The cheapest visit the complete the sequence source->visit->target from the visits given.</returns>
        public static int CalculateCheapest(this float[][] weights, int source, int target, IEnumerable<int> visits,
            out float weight)
        {
            weight = float.MaxValue;
            var best = Constants.NOT_SET;

            foreach (var visit in visits)
            {
                var localWeight = 0f;
                if (source != Constants.NOT_SET)
                {
                    localWeight += weights[source][visit];
                }
                if (target != Constants.NOT_SET)
                {
                    localWeight += weights[visit][target];
                }
                if (localWeight < weight)
                {
                    weight = localWeight;
                    best = target;
                }
            }

            return best;
        }
    }
}