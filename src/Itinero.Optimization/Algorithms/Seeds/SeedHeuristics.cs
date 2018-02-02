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
        /// Selects a seed with the minimum average weight to the closest 'n' nearest neighbours. This heuristic is deterministic.
        /// </summary>
        /// <param name="weights">The weights.</param>
        /// <param name="visitPool">The pool of visits to choose from.</param>
        /// <param name="n">The # of nearest neighbours to calculate the average for.</param>
        /// <returns></returns>
        public static int GetSeedWithCloseNeighbours(float[][] weights, IList<int> visitPool)
        {
            int seed = Constants.NOT_SET;
            var maxWeight = float.MaxValue;
            foreach (int pooledVisit in visitPool)
            {
                var neighbours = new SortedDictionary<float, List<int>>();
                for (int idx = 0; idx < visitPool.Count; idx++)
                {
                    int visit = visitPool[idx];
                    if (visit != pooledVisit)
                    {
                        var weight = weights[pooledVisit][visit] +
                            weights[pooledVisit][visit];
                        List<int> visitList = null;
                        if (!neighbours.TryGetValue(weight, out visitList))
                        {
                            visitList = new List<int>();
                            neighbours.Add(weight, visitList);
                        }
                        visitList.Add(visit);
                    }
                }

                var nearestNeighbourAverage = 0f;
                int neirgbourCount = 20;
                int neighbourCounted = 0;
                foreach (var pair in neighbours)
                {
                    foreach (int customer in pair.Value)
                    {
                        if (neighbourCounted < neirgbourCount)
                        {
                            neighbourCounted++;
                            nearestNeighbourAverage = nearestNeighbourAverage +
                                pair.Key;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                if (maxWeight > nearestNeighbourAverage)
                {
                    maxWeight = nearestNeighbourAverage;
                    seed = pooledVisit;
                }
            }
            return seed;
        }
    }
}