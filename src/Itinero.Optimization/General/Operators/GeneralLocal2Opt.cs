// Itinero.Optimization - Route optimization for .NET
// Copyright (C) 2016 Abelshausen Ben
// 
// This file is part of Itinero.
// 
// Itinero is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// Itinero is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Itinero. If not, see <http://www.gnu.org/licenses/>.

using Itinero.Optimization.Algorithms.Solvers;
using Itinero.Optimization.Routes;
using System;
using System.Collections.Generic;

namespace Itinero.Optimization.General.Operators
{
    /// <summary>
    /// A local 2-Opt* search.
    /// </summary>
    /// <remarks>* 2-Opt: Removes two edges and reconnects the two resulting paths in a different way to obtain a new tour.</remarks>
    public sealed class GeneralLocal2Opt<TProblem, TObjective> : IOperator<float, TProblem, TObjective, Route, float>
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
        public bool Apply(TProblem problem, TObjective objective, Route route, out float delta)
        {
            delta = 0;

            var weights = _getWeights(problem);

            var customers = new List<int>(weights.Length + 1);
            customers.AddRange(route);
            if (route.Last == route.First)
            { // add last customer at the end if it's the same as the first one.
                customers.Add(route.Last.Value);
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
                        route.ReplaceEdgeFrom(edge11, edge21);
                        route.ReplaceEdgeFrom(edge12, edge22);
                        // reverse intermediates.
                        for (var i = edge1 + 1; i < edge2; i++)
                        {
                            route.ReplaceEdgeFrom(customers[i + 1], customers[i]);
                        }

                        return true;
                    }
                }
            }
            return false;
        }
    }
}