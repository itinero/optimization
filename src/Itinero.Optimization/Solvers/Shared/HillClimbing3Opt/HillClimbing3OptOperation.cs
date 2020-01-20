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
using System.Linq;
using System.Runtime.CompilerServices;
using Itinero.Logging;
using Itinero.Optimization.Solvers.Shared.NearestNeighbours;
using Itinero.Optimization.Solvers.Tours;

[assembly: InternalsVisibleTo("Itinero.Optimization.Tests")]
[assembly: InternalsVisibleTo("Itinero.Optimization.Tests.Benchmarks")]
namespace Itinero.Optimization.Solvers.Shared.HillClimbing3Opt
{
    /// <summary>
    /// Contains the core of the 3-OPT hill climbing algorithm. 
    /// </summary>
    public static class HillClimbing3OptOperation
    {
        private const float E = 0.1f;
        
        /// <summary>
        /// Returns true if there was an improvement, false otherwise.
        /// </summary>
        /// <param name="weightFunc">The weight function.</param>
        /// <param name="candidate">The candidate tour.</param>
        /// <param name="size">The size of the weight matrix.</param>
        /// <param name="nearestNeighbours">The nearest neighbours, not used if null.</param>
        /// <param name="useDontLookBits">The don't look bits flag.</param>
        /// <returns>True if there was an improvement and the associated cost.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static (bool improved, float delta) Do3Opt(this Tour candidate, Func<int, int, float> weightFunc,
            int size, NearestNeighbourArray nearestNeighbours = null, bool useDontLookBits = true)
        {
            float delta;
            
            // TODO: check and see if we can apply this to an open problem anyway. This would remove the need to convert existing TSP's.
            if (!candidate.Last.HasValue)
            {
                Logger.Log($"{typeof(HillClimbing3OptOperation)}.{nameof(Do3Opt)}", TraceEventType.Warning,
                    "Cannot apply this operator to an open problem, skipping.");
                delta = 0;
                return (false, delta);
            }

            if (candidate.First != candidate.Last)
            {
                Logger.Log($"{typeof(HillClimbing3OptOperation)}.{nameof(Do3Opt)}", TraceEventType.Warning,
                    "Cannot apply this operator to an open tour, skipping.");
                delta = 0;
                return (false, delta);
            }

            if ((candidate.First == candidate.Last) != candidate.Last.HasValue)
            {
                throw new ArgumentException("Tour has to be closed.");
            }

            // TODO: [PERFORMANCE] can we prevent recreating this all the time?
            var dontLookBits = useDontLookBits ? new bool[size] : null;

            // loop over all customers.
            var anyImprovement = false;
            var improvement = true;
            delta = 0f;
            while (improvement)
            {
                improvement = false;
                foreach (var v1 in candidate)
                {
                    if (Check(dontLookBits, v1)) continue;
                    if (Try3OptMoves(weightFunc, candidate, dontLookBits, v1, out var moveDelta, nearestNeighbours))
                    {
                        anyImprovement = true;
                        improvement = true;
                        delta = delta + moveDelta;
                        break;
                    }

                    Set(dontLookBits, v1, true);
                }
            }

            return (anyImprovement, delta);
        }

        /// <summary>
        /// Tries all 3Opt Moves for the neighbourhood of v1.
        /// </summary>
        /// <returns></returns>
        private static bool Try3OptMoves(Func<int, int, float> weightFunc, Tour tour, bool[] dontLookBits, int v1, out float delta,
            NearestNeighbourArray nearestNeighbours)
        {
            // get v_2.
            var v2 = tour.GetNeigbour(v1);
            if (v2 < 0)
            {
                delta = 0;
                return false;
            }

            var betweenV2V1 = tour.Between(v2, v1);
            var weightV1V2 = weightFunc(v1, v2); // weights[v1][v2];
            if (v2 == tour.First && !tour.Last.HasValue)
            {
                // set to zero if not closed.
                weightV1V2 = 0;
            }

            var v3 = -1;
            // TODO: performance test when neighbours array is a hashset.
            var neighbours = nearestNeighbours?[v1];
            
            foreach (var v4 in betweenV2V1)
            {
                if (v3 >= 0 && v3 != v1)
                {
                    if (neighbours == null || neighbours.Contains(v4)) // TODO: this is now linq, ouwch, check TODO above.
                    {
                        var weightV1V4 = weightFunc(v1, v4); // weights[v1][v4];
                        var weightV3V4 = weightFunc(v3, v4); // weights[v3][v4];
                        if (v4 == tour.First && !tour.Last.HasValue)
                        {
                            // set to zero if not closed.
                            weightV1V4 = 0;
                            weightV3V4 = 0;
                        }

                        var weightV1V2PlusV3V4 = weightV1V2 + weightV3V4;
                        // var weightsV3 = weights[v3];
                        if (Try3OptMoves(weightFunc, tour, dontLookBits, v1, v2, v3, v4, weightV1V2PlusV3V4, weightV1V4,
                            out delta))
                        {
                            return true;
                        }
                    }
                }

                v3 = v4;
            }

            delta = 0;
            return false;
        }

        /// <summary>
        /// Tries all 3Opt Moves for the neighbourhood of v_1 containing v_3.
        /// </summary>
        /// <returns></returns>
        private static bool Try3OptMoves(Func<int, int, float> weightFunc, Tour tour, bool[] dontLookBits,
            int v1, int v2, float weightV1V2,
            int v3, out float delta)
        {
            // get v_4.
            var v4 = tour.GetNeigbour(v3);
            var weightV1V2PlusV3V4 = weightV1V2 + weightFunc(v3, v4); // weights[v3][v4];
            var weightV1V4 = weightFunc(v1, v4); // weights[v1][v4];
            if (v4 == tour.First && !tour.Last.HasValue)
            {
                // set to zero if not closed.
                weightV1V4 = 0;
            }

            //var weightsV3 = weights[v3];
            return Try3OptMoves(weightFunc, tour, dontLookBits, v1, v2, v3, v4, weightV1V2PlusV3V4, weightV1V4, out delta);
        }

        /// <summary>
        /// Tries all 3Opt Moves for the neighbourhood of v_1 containing v_3.
        /// </summary>
        /// <returns></returns>
        private static bool Try3OptMoves(Func<int, int, float> weightFunc, Tour tour, bool[] dontLookBits,
            int v1, int v2, int v3, int v4, float weightV1V2PlusV3V4, float weightV1V4, out float delta)
        {
            var betweenV4V1Enumerator = tour.Between(v4, v1).GetEnumerator();
            if (betweenV4V1Enumerator.MoveNext())
            {
                var v5 = betweenV4V1Enumerator.Current;
                if (v5 != v1)
                {
                    while (betweenV4V1Enumerator.MoveNext())
                    {
                        var v6 = betweenV4V1Enumerator.Current;
                        var weightV3V6 = weightFunc(v3, v6); // weightsV3[v6];
                        var weightV5V2 = weightFunc(v5, v2); // weights[v5][v2];
                        var weightV5V6 = weightFunc(v5, v6); // weights[v5][v6];
                        if (v6 == tour.First && !tour.Last.HasValue)
                        {
                            // set to zero if not closed.
                            weightV3V6 = 0;
                            weightV5V6 = 0;
                        }

                        if (v2 == tour.First && !tour.Last.HasValue)
                        {
                            // set to zero if not closed.
                            weightV5V2 = 0;
                        }

                        // calculate the total weight of the 'new' arcs.
                        var weightNew = weightV1V4 + weightV3V6 + weightV5V2;

                        // calculate the total weights.
                        var weight = weightV1V2PlusV3V4 + weightV5V6;

                        if (weight - weightNew > E)
                        {
                            // actually do replace the vertices.
                            var countBefore = tour.Count;
                            delta = weightNew - weight;
                            
                            tour.ReplaceEdgeFrom(v1, v4);
                            tour.ReplaceEdgeFrom(v3, v6);
                            tour.ReplaceEdgeFrom(v5, v2);
                            
                            Console.WriteLine($"{weightNew}-{weight}={delta}: {v1}->{v4}/{v3}->{v6}/{v5}->{v2}");

                            // set bits.
                            Set(dontLookBits, v3, false);
                            Set(dontLookBits, v5, false);

                            return true; // move succeeded.
                        }

                        v5 = v6;
                    }
                }
            }

            delta = 0;
            return false;
        }

        private static bool Check(bool[] dontLookBits, int visit)
        {
            if (dontLookBits != null)
            {
                return dontLookBits[visit];
            }

            return false;
        }

        private static void Set(bool[] dontLookBits, int visit, bool value)
        {
            if (dontLookBits != null)
            {
                dontLookBits[visit] = value;
            }
        }
    }
}