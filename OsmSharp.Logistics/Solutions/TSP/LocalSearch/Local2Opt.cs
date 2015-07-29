// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

using OsmSharp.Logistics.Routes;
using OsmSharp.Logistics.Solvers;
using System;
using System.Collections.Generic;

namespace OsmSharp.Logistics.Solutions.TSP.LocalSearch
{
    /// <summary>
    /// A local 2-Opt* search for the TSP.
    /// </summary>
    /// <remarks>* 2-Opt: Removes two edges and reconnects the two resulting paths in a different way to obtain a new tour.</remarks>
    public class Local2Opt : IOperator<ITSP, ITSPObjective, IRoute>
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
        public bool Supports(ITSPObjective objective)
        {
            return objective.Name == MinimumWeightObjective.MinimumWeightObjectiveName;
        }

        /// <summary>
        /// Returns true if there was an improvement, false otherwise.
        /// </summary>
        /// <returns></returns>
        public bool Apply(ITSP problem, ITSPObjective objective, IRoute route, out double delta)
        {
            delta = 0;

            var customers = new List<int>(problem.Weights.Length + 1);
            customers.AddRange(route);
            if(route.Last == route.First)
            { // add last customer at the end if it's the same as the first one.
                customers.Add(route.Last.Value);
            }

            var betweenForward = 0.0;
            var betweenBackward = 0.0;
            var forward = 0.0;
            var backward = 0.0;
            for (var edge1 = 0; edge1 < customers.Count - 3; edge1++)
            { // iterate over all from-edges.
                var edge11 = customers[edge1];
                var edge12 = customers[edge1 + 1];
                var edge13 = customers[edge1 + 2];

                betweenBackward = 0;
                betweenForward = 0;
                for (var edge2 = edge1 + 2; edge2 < customers.Count - 2; edge2++)
                { // iterate over all to-edges.
                    var edge20 = customers[edge2 - 1];
                    var edge21 = customers[edge2];
                    var edge22 = customers[edge2 + 1];
                    var edge23 = customers[edge2 + 2];

                    if (edge2 - edge1 == 2)
                    { // when diff = 3 
                        forward = problem.Weights[edge11][edge12] + problem.Weights[edge12][edge21] +
                            problem.Weights[edge21][edge22] + problem.Weights[edge22][edge23];
                        backward = problem.Weights[edge11][edge22] + problem.Weights[edge22][edge21] +
                            problem.Weights[edge21][edge12] + problem.Weights[edge12][edge23];

                        if (forward > backward)
                        { // we found an improvement.
                            delta = forward - backward;
                            route.ReplaceEdgeFrom(edge11, edge22);
                            route.ReplaceEdgeFrom(edge22, edge21);
                            route.ReplaceEdgeFrom(edge21, edge12);
                            route.ReplaceEdgeFrom(edge12, edge23);
                            return true;
                        }
                    }
                    else if(edge2 - edge1 == 3)
                    { // when diff = 4
                        forward = problem.Weights[edge11][edge12] + problem.Weights[edge12][edge13] + problem.Weights[edge13][edge21] +
                            problem.Weights[edge21][edge22] + problem.Weights[edge22][edge23];
                        backward = problem.Weights[edge11][edge22] + problem.Weights[edge22][edge13] + problem.Weights[edge13][edge21] +
                            problem.Weights[edge21][edge12] + problem.Weights[edge12][edge23];

                        if (forward > backward)
                        { // we found an improvement.
                            delta = forward - backward;

                            return true;
                        }
                    }
                    else if (edge2 - edge1 > 3)
                    { // when diff = 4
                        betweenForward += problem.Weights[edge20][edge21];
                        betweenBackward += problem.Weights[edge21][edge20];
    
                        forward = problem.Weights[edge11][edge12] + problem.Weights[edge12][edge13] +
                            problem.Weights[edge21][edge22] + problem.Weights[edge22][edge23] + betweenForward;
                        backward = problem.Weights[edge11][edge22] + problem.Weights[edge22][edge13] +
                            problem.Weights[edge21][edge12] + problem.Weights[edge12][edge23] + betweenBackward;

                        if (forward > backward)
                        { // we found an improvement.
                            delta = forward - backward;

                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}