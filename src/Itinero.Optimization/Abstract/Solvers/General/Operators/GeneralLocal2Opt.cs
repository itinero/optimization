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
using Itinero.Optimization.Abstract.Tours;
using System;
using System.Collections.Generic;

namespace Itinero.Optimization.General.Operators
{
    /// <summary>
    /// A local 2-Opt* search.
    /// </summary>
    /// <remarks>* 2-Opt: Removes two edges and reconnects the two resulting paths in a different way to obtain a new tour.</remarks>
    public sealed class GeneralLocal2Opt<TProblem, TObjective> : IOperator<float, TProblem, TObjective, Tour, float>
    {
        private readonly Func<TProblem, float[][]> _getWeights;

        /// <summary>
        /// Creates a new local 2-Opt* search.
        /// </summary>
        public GeneralLocal2Opt(Func<TProblem, float[][]> getWeights)
        {
            _getWeights = getWeights;
        }

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
        public bool Supports(TObjective objective)
        {
            return true;
        }

        /// <summary>
        /// Returns true if there was an improvement, false otherwise.
        /// </summary>
        /// <returns></returns>
        public bool Apply(TProblem problem, TObjective objective, Tour tour, out float delta)
        {
            delta = 0;

            var weights = _getWeights(problem);

            var customers = new List<int>(weights.Length + 1);
            customers.AddRange(tour);
            if (tour.Last == tour.First)
            { // add last customer at the end if it's the same as the first one.
                customers.Add(tour.Last.Value);
            }

            var betweenForward = 0.0f;
            var betweenBackward = 0.0f;
            var existing = 0.0f;
            var potential = 0.0f;
            for (var edge1 = 0; edge1 < customers.Count - 3; edge1++)
            { // iterate over all from-edges.
                var edge11 = customers[edge1];
                var edge12 = customers[edge1 + 1];

                betweenBackward = 0;
                betweenForward = 0;
                for (var edge2 = edge1 + 2; edge2 < customers.Count - 1; edge2++)
                { // iterate over all to-edges.
                    var edge20 = customers[edge2 - 1];
                    var edge21 = customers[edge2];
                    var edge22 = customers[edge2 + 1];

                    betweenForward += weights[edge20][edge21];
                    betweenBackward += weights[edge21][edge20];

                    existing = weights[edge11][edge12] + betweenForward +
                        weights[edge21][edge22];
                    potential = weights[edge11][edge21] + betweenBackward +
                        weights[edge12][edge22];

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
            return false;
        }
    }
}