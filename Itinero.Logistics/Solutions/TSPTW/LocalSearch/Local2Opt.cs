// Itinero - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
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

using Itinero.Logistics.Routes;
using Itinero.Logistics.Solutions.TSPTW;
using Itinero.Logistics.Solutions.TSPTW.Objectives;
using Itinero.Logistics.Solvers;
using System;
using System.Collections.Generic;

namespace Itinero.Logistics.Solutions.TSPTW.LocalSearch
{
    /// <summary>
    /// A local 2-Opt* search for the TSP-TW.
    /// </summary>
    /// <remarks>* 2-Opt: Removes two edges and reconnects the two resulting paths in a different way to obtain a new tour.</remarks>
    public class Local2Opt : IOperator<ITSPTW, ITSPTWObjective, IRoute>
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
        public bool Supports(ITSPTWObjective objective)
        {
            return objective.Name == MinimumWeightObjective.MinimumWeightObjectiveName;
        }

        /// <summary>
        /// Returns true if there was an improvement, false otherwise.
        /// </summary>
        /// <returns></returns>
        public bool Apply(ITSPTW problem, ITSPTWObjective objective, IRoute route, out double delta)
        {
            delta = 0;

            var customers = new List<int>(problem.Weights.Length + 1);
            customers.AddRange(route);
            if (route.Last == route.First)
            { // add last customer at the end if it's the same as the first one.
                customers.Add(route.Last.Value);
            }

            var betweenForward = 0.0;
            var existing = 0.0;
            var potential = 0.0;
            var weight12 = 0.0;
            if (problem.Windows[customers[0]].Min > weight12)
            { // wait here!
                weight12 = problem.Windows[customers[0]].Min;
            }
            for (var edge1 = 0; edge1 < customers.Count - 3; edge1++)
            { // iterate over all from-edges.
                var edge11 = customers[edge1];
                var edge12 = customers[edge1 + 1];

                var weight11 = weight12;
                weight12 += problem.Weights[edge11][edge12];
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
                    var currentWeight = weight11 + problem.Weights[edge11][edge21];
                    if (problem.Windows[edge21].Min > currentWeight)
                    { // wait here!
                        currentWeight = problem.Windows[edge21].Min;
                    }
                    var previous = edge21;
                    for (var i = edge2 - 1; i > edge1; i--)
                    {
                        var current = customers[i];
                        currentWeight += problem.Weights[previous][current];
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
                    potential = currentWeight + problem.Weights[edge12][edge22];

                    if(feasible)
                    { // new reverse is feasible.
                        // calculate existing value of the part 11->12->...->21->22.
                        // @ 22: no need to take minimum of window into account, is valid now, will stay valid on reduction of arrival-time.
                        betweenForward += problem.Weights[edge20][edge21];
                        if (betweenForward + weight12 < problem.Windows[edge21].Min)
                        { // take into account minimum-window constraint.
                            betweenForward = problem.Windows[edge21].Min - weight12;
                        }
                        existing = weight12 + betweenForward + problem.Weights[edge21][edge22];
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
            }
            return false;
        }
    }
}