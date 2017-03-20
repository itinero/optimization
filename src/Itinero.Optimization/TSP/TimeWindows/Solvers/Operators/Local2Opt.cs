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

using Itinero.Optimization.Algorithms.Solvers;
using Itinero.Optimization.Tours;
using System.Collections.Generic;

namespace Itinero.Optimization.TSP.TimeWindows.Solvers.Operators
{
    /// <summary>
    /// A local 2-Opt* search for the TSP-TW.
    /// </summary>
    /// <remarks>* 2-Opt: Removes two edges and reconnects the two resulting paths in a different way to obtain a new tour.</remarks>
    public class Local2Opt : IOperator<float, TSPTWProblem, TSPTWObjective, Tour, float>
    {
        /// <summary>
        /// Returns the name of the operator.
        /// </summary>
        public string Name
        {
            get { return "2OPT"; }
        }

        /// <summary>
        /// Returns true if the given objective is supported.
        /// </summary>
        /// <returns></returns>
        public bool Supports(TSPTWObjective objective)
        {
            return true;
        }

        /// <summary>
        /// Returns true if there was an improvement, false otherwise.
        /// </summary>
        /// <returns></returns>
        public bool Apply(TSPTWProblem problem, TSPTWObjective objective, Tour tour, out float delta)
        {
            delta = 0;

            var customers = new List<int>(problem.Times.Length + 1);
            customers.AddRange(tour);
            if (tour.Last == tour.First)
            { // add last customer at the end if it's the same as the first one.
                customers.Add(tour.Last.Value);
            }

            var betweenForward = 0.0f;
            var existing = 0.0f;
            var potential = 0.0f;
            var weight12 = 0.0f;
            if (problem.Windows[customers[0]].Min > weight12)
            { // wait here!
                weight12 = problem.Windows[customers[0]].Min;
            }
            for (var edge1 = 0; edge1 < customers.Count - 3; edge1++)
            { // iterate over all from-edges.
                var edge11 = customers[edge1];
                var edge12 = customers[edge1 + 1];

                var weight11 = weight12;
                weight12 += problem.Times[edge11][edge12];
                if (problem.Windows[edge12].Min > weight12)
                { // wait here!
                    weight12 = problem.Windows[edge12].Min;
                }

                betweenForward = 0;
                for (var edge2 = edge1 + 2; edge2 < customers.Count - 1; edge2++)
                { // iterate over all to-edges.
                    var edge20 = customers[edge2 - 1];
                    var edge21 = customers[edge2];
                    var edge22 = customers[edge2 + 1];

                    // calculate existing value of the part 11->21->(reverse)->12->22.
                    // @ 22: no need to take minimum of window into account, is valid now, will stay valid on reduction of arrival-time.
                    // completely re-calculate between-backward (because window min may be violated) and determine feasible at the same time.
                    var feasible = true;
                    var currentWeight = weight11 + problem.Times[edge11][edge21];
                    if (problem.Windows[edge21].Min > currentWeight)
                    { // wait here!
                        currentWeight = problem.Windows[edge21].Min;
                    }
                    var previous = edge21;
                    for (var i = edge2 - 1; i > edge1; i--)
                    {
                        var current = customers[i];
                        currentWeight += problem.Times[previous][current];
                        if (problem.Windows[current].Min > currentWeight)
                        { // wait here!
                            currentWeight = problem.Windows[current].Min;
                        }
                        var window = problem.Windows[current];
                        if (window.Max < currentWeight)
                        {
                            feasible = false;
                            break;
                        }
                        previous = current;
                    }
                    potential = currentWeight + problem.Times[edge12][edge22];

                    if (feasible)
                    { // new reverse is feasible.
                        // calculate existing value of the part 11->12->...->21->22.
                        // @ 22: no need to take minimum of window into account, is valid now, will stay valid on reduction of arrival-time.
                        betweenForward += problem.Times[edge20][edge21];
                        if (betweenForward + weight12 < problem.Windows[edge21].Min)
                        { // take into account minimum-window constraint.
                            betweenForward = problem.Windows[edge21].Min - weight12;
                        }
                        existing = weight12 + betweenForward + problem.Times[edge21][edge22];
                        if (existing > potential)
                        { // we found an improvement.
                            delta = existing - potential;
                            tour.ReplaceEdgeFrom(edge11, edge21);
                            tour.ReplaceEdgeFrom(edge12, edge22);
                            // reverse intermediates.
                            for (var i = edge1 + 1; i < edge2; i++)
                            {
                                tour.ReplaceEdgeFrom(customers[i + 1], customers[i]);
                            }
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}