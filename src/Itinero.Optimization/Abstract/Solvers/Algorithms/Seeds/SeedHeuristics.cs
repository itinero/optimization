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
using System.Collections.Generic;
using Itinero.Optimization.Algorithms.NearestNeighbour;

namespace Itinero.Optimization.Algorithms.Seeds
{
    /// <summary>
    /// A collection of common reusable seed heuristics.
    /// </summary>
    public static class SeedHeuristics
    {
        /// <summary>
        /// Select a visit farthest from the given visit. This is a deterministic heuristic.
        /// </summary>
        /// <param name="weights">The weights between the visits.</param>
        /// <param name="visitPool">The pool of visits to choose from.</param>
        /// <param name="visit">The reference visit.</param>
        /// <param name="direction">The direction flag for the weights to check, when null bidirectional, true forward and false backward.</param>
        /// <returns>The selected visit or Constant.NO_SET if no visit could be selected. Guarantees a valid return if there are visits in the given pool.</returns>
        public static int GetSeedFarthest(float[][] weights, IEnumerable<int> visitPool, int visit,
            bool? direction = null)
        {
            var distance = -1f;
            var seed = Constants.NOT_SET;
            foreach (var pooledVisit in visitPool)
            {
                var localDistance = 0f;
                if (direction == null || direction == true)
                {
                    localDistance += weights[visit][pooledVisit];
                }
                if (direction == null || direction == false)
                {
                    localDistance += weights[pooledVisit][visit];
                }

                if (localDistance > distance)
                {
                    seed = pooledVisit;
                    distance = localDistance;
                }
            }

            return seed;
        }

        /// <summary>
        /// Select a visit closest to a given visit. This is a deterministic heuristic.
        /// </summary>
        /// <param name="weights">The weights between the visits.</param>
        /// <param name="visitPool">The pool of visits to choose from.</param>
        /// <param name="visit">The reference visit.</param>
        /// <param name="direction">The direction flag for the weights to check, when null bidirectional, true forward and false backward.</param>
        /// <returns>The selected visit or Constant.NO_SET if no visit could be selected. Guarantees a valid return if there are visits in the given pool.</returns>
        public static int GetSeedClosest(float[][] weights, IEnumerable<int> visitPool, int visit,
            bool? direction = null)
        {
            var distance = float.MaxValue;
            var seed = Constants.NOT_SET;
            foreach (var pooledVisit in visitPool)
            {
                var localDistance = 0f;
                if (direction == null || direction == true)
                {
                    localDistance += weights[visit][pooledVisit];
                }
                if (direction == null || direction == false)
                {
                    localDistance += weights[pooledVisit][visit];
                }

                if (localDistance < distance)
                {
                    seed = pooledVisit;
                    distance = localDistance;
                }
            }

            return seed;
        }

        /// <summary>
        /// Select a random seed from the given visits pool.
        /// </summary>
        /// <param name="visitPool">The pool of visits to choose from.</param>
        /// <returns>The selected visit or Constant.NO_SET if no visit could be selected. Guarantees a valid return if there are visits in the given pool.</returns>
        public static int GetSeedRandom(IList<int> visitPool)
        {
            if (visitPool.Count == 0)
            {
                return Constants.NOT_SET;
            }
            return visitPool[Random.RandomGeneratorExtensions.GetRandom().Generate(visitPool.Count)];
        }

        /// <summary>
        /// Selects a seed with the minimum average weight to the closest 'n' nearest neighbours. This heuristic is deterministic if p = 1.
        /// </summary>
        /// <param name="weights">The weights.</param>
        /// <param name="nnArray">The nearest neighbour array.</param>
        /// <param name="visitPool">The pool of visits to choose from.</param>
        /// <param name="neighbourCount">The # of neighbours to count.</param>
        /// <param name="neighbourVisitedPentaly">The penalty to assign if on of the neigbours has already been visited.</param>
        /// <param name="p">The probablity a visit will be considered.</param>
        /// <returns>A selected seed from the visit pool.</returns>
        public static int GetSeedWithCloseNeighbours(float[][] weights, NearestNeigbourArray nnArray, IList<int> visitPool,
            int neighbourCount = 10, float neighbourVisitedPentaly = 0, float p = 1)
        {
            var visitPoolSet = new HashSet<int>(visitPool);
            var neighbours = new int[nnArray.N];

            var random = Itinero.Optimization.Algorithms.Random.RandomGeneratorExtensions.GetRandom();

            int seed = Constants.NOT_SET;
            var maxWeight = float.MaxValue;
            foreach (int pooledVisit in visitPool)
            {
                if (p < 1 && random.Generate(1f) > p)
                { // oeps, skip this one.
                    continue;
                }

                nnArray.CopyTo(pooledVisit, neighbours);

                var nearestNeighbourAverage = 0f;
                int neighbourCounted = 0;

                foreach (var neighbour in neighbours)
                {
                    if (visitPoolSet.Contains(neighbour))
                    {
                        if (neighbourCounted < neighbourCount)
                        {
                            neighbourCounted++;
                            nearestNeighbourAverage = nearestNeighbourAverage +
                                weights[pooledVisit][neighbour] + weights[neighbour][pooledVisit];
                        }
                        else
                        {
                            break;
                        }
                    }
                    else if (neighbourVisitedPentaly > 0)
                    { // just add this as a penalty.
                        nearestNeighbourAverage = nearestNeighbourAverage +
                            (weights[pooledVisit][neighbour] + weights[neighbour][pooledVisit]) * neighbourVisitedPentaly;
                    }
                }

                if (maxWeight > nearestNeighbourAverage)
                {
                    maxWeight = nearestNeighbourAverage;
                    seed = pooledVisit;
                }
            }

            if (seed == Constants.NOT_SET)
            { // if p is too small things get close to random anyway.
                // if there are but a few visits left.
                seed = SeedHeuristics.GetSeedRandom(visitPool);
            }

            return seed;
        }
    }
}