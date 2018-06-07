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

namespace Itinero.Optimization.Solvers.TSP
{
    /// <summary>
    /// Contains extension methods related to the TSP.
    /// </summary>
    internal static class TSProblemExtensions
    {
        /// <summary>
        /// Calculates the total weight for the given tour.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="tour">The tour.</param>
        /// <returns>Returns the weight.</returns>
        internal static float Weights(this TSProblem problem, Tour tour)
        {
            return problem.Weights(tour, tour.First == tour.Last);
        }

        /// <summary>
        /// Calculates the total weight for the given tour.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="tour">The tour.</param>
        /// <param name="closed">The when true also count the connection between last and visit visit.</param>
        /// <returns>Returns the weight.</returns>
        internal static float Weights(this TSProblem problem, IEnumerable<int> tour, bool closed)
        {
            var weight = 0f;
            var previous = Tour.NOT_SET;
            var first = Tour.NOT_SET;
            foreach (var visit in tour)
            {
                if (previous == Tour.NOT_SET)
                {
                    first = visit;
                    previous = visit;
                    continue;
                }

                weight += problem.Weight(previous, visit);
                previous = visit;
            }

            if (closed &&
                first != Tour.NOT_SET)
            {
                weight += problem.Weight(previous, first);
            }

            return weight;
        }
        
        /// <summary>
        /// Converts this problem to it's closed equivalent.
        /// </summary>
        /// <returns></returns>
        internal static TSProblem ToClosed(this TSProblem problem)
        {
            if (problem.Last == null)
            { // 'open' problem, just set weights to first to 0.
                // REMARK: weights already set in constructor.
                var weights = new float[problem.Count][];
                for (var x = 0; x < weights.Length; x++)
                {
                    weights[x] = new float[problem.Count];
                    for (var y = 0; y < weights.Length; y++)
                    {
                        weights[x][y] = problem.Weight(x, y);
                    }
                }
                return new TSProblem(problem.First, problem.First, weights);
            }
            else if (problem.First != problem.Last)
            { // 'open' problem but with fixed last.
                var weights = new float[problem.Count - 1][];
                for (var x = 0; x < problem.Count; x++)
                {
                    if (x == problem.Last)
                    { // skip last edge.
                        continue;
                    }
                    var xNew = x;
                    if (x > problem.Last)
                    { // decrease new index.
                        xNew = xNew - 1;
                    }

                    weights[xNew] = new float[problem.Count - 1];

                    for (var y = 0; y < problem.Count; y++)
                    {
                        if (y == problem.Last)
                        { // skip last edge.
                            continue;
                        }
                        var yNew = y;
                        if (y > problem.Last)
                        { // decrease new index.
                            yNew = yNew - 1;
                        }

                        if (yNew == xNew)
                        { // make not sense to keep values other than '0' and to make things easier to understand just use '0'.
                            weights[xNew][yNew] = 0;
                        }
                        else if (y == problem.First)
                        { // replace -> first with -> last.
                            weights[xNew][yNew] = problem.Weight(x, problem.Last.Value);
                        }
                        else
                        { // nothing special about this connection, yay!
                            weights[xNew][yNew] = problem.Weight(x, y);
                        }
                    }
                }
                return new TSProblem(problem.First, problem.First, weights);
            }
            return problem; // problem already closed with first==last.
        }
    }
}