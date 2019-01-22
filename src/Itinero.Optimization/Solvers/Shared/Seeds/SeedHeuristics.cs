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
using System.Linq;
using Itinero.Optimization.Solvers.Shared.NearestNeighbours;
using Itinero.Optimization.Solvers.Tours;
using Itinero.Optimization.Strategies.Random;

namespace Itinero.Optimization.Solvers.Shared.Seeds
{
    /// <summary>
    /// Contains seed heuristics.
    /// </summary>
    public static class SeedHeuristics
    {
        /// <summary>
        /// Select a random seed from the given visits pool.
        /// </summary>
        /// <param name="visitPool">The pool of visits to choose from.</param>
        /// <returns>The selected visit or Constant.NO_SET if no visit could be selected. Guarantees a valid return if there are visits in the given pool.</returns>
        public static int GetSeedRandom(ICollection<int> visitPool)
        {
            if (visitPool.Count == 0)
            {
                return Tour.NOT_SET;
            }

            var pos = Strategies.Random.RandomGenerator.Default.Generate(visitPool.Count);
            return visitPool.ElementAt(pos);
        }

        /// <summary>
        /// Selects a fixed number of seeds from the visits pool.
        /// </summary>
        /// <param name="visitPool">The pool to select from.</param>
        /// <param name="vehicleCount">The # of vehicles.</param>
        /// <param name="weightFunc">The weight function.</param>
        /// <param name="nearestNeighbourArray">The nearest neighbour array.</param>
        /// <returns>A set of seeds.</returns>
        public static int[] GetSeeds(ICollection<int> visitPool, int vehicleCount, Func<int, int, float> weightFunc, NearestNeighbourArray nearestNeighbourArray)
        {
            var seeds = new int[vehicleCount];
            
            // select random seeds.
            var i = 0;
            var best = seeds.Clone() as int[];
            var bestFitness = float.MaxValue; 
            while (i < 100)
            {
                RandomGenerator.Default.SelectRandomFrom(visitPool, ref best);
                
                // calculate fitness.
                var fitness = SeedsFitness(best, weightFunc, nearestNeighbourArray);
                if (fitness < bestFitness)
                {
                    best.CopyTo(seeds, 0);
                    bestFitness = fitness;
                }

                i++;
            }

            return seeds;
        }

        private static float SeedsFitness(int[] seeds, Func<int, int, float> weightFunc,
            NearestNeighbourArray nearestNeighbourArray)
        {

            // calculate 'fitness' based on density and proximity to other seeds.
            // calculate proximity, the average weight between seeds.
            var proximity = float.MaxValue;
            for (var i = 0; i < seeds.Length; i++)
            {
                for (var j = 0; j < i; j++)
                {
                    var local = weightFunc(seeds[i], seeds[j]);
                    if (proximity > local)
                    {
                        proximity = local;
                    }
                }
            }
            //proximity = proximity / ((seeds.Length - 1) * (seeds.Length - 1) / 2f);
            
            // calculate density, the average weight to the nearest neighbours.
            var density = 0f;
            var densityCount = 0;
            for (var i = 0; i < seeds.Length; i++)
            {
                foreach (var neighbour in nearestNeighbourArray[seeds[i]])
                {
                    density += weightFunc(seeds[i], neighbour);
                    densityCount++;
                }
            }
            density = density / densityCount;
            
            return 1 / proximity;
        }
    }
}