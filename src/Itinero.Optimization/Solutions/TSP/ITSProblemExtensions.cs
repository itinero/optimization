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

namespace Itinero.Optimization.Solutions.TSP
{
    /// <summary>
    /// Contains extension method for the ITSProblem.
    /// </summary>
    public static class ITSProblemExtensions
    {
        /// <summary>
        /// Solves this TSP using a default solver.
        /// </summary>
        /// <returns></returns>
        public static ITour Solve(this ITSProblem problem)
        {
            return problem.Solve(new Solvers.EAXSolver(Algorithms.Solvers.GA.GASettings.Default));
        }

        /// <summary>
        /// Solvers this problem using the given solver.
        /// </summary>
        public static ITour Solve(this ITSProblem problem, Algorithms.Solvers.ISolver<float, ITSProblem, TSPObjective, ITour, float> solver)
        {
            return solver.Solve(problem, new TSPObjective());
        }

        /// <summary>
        /// Converts a tour (could be a sub-tour) and a set of weights into a TSP.
        /// </summary>
        /// <param name="tour"></param>
        /// <param name="weights"></param>
        /// <returns></returns>
        public static ITSProblem ToTSProblem(this ITour tour, float[][] weights)
        {
            return new TSPSubProblem(tour, weights);
        }
        
        /// <summary>
        /// Converts this problem to it's closed equivalent.
        /// </summary>
        /// <returns></returns>
        public static ITSProblem ToClosed(this ITSProblem problem)
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
