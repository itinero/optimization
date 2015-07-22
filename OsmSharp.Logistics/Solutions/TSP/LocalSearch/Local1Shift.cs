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
    /// A local 1-Shift search for the TSP.
    /// </summary>
    /// <remarks>* 1-shift: Remove a customer and relocate it somewhere.</remarks>
    public class Local1Shift : IOperator<ITSP, ITSPObjective, IRoute>
    {
        /// <summary>
        /// Returns the name of the operator.
        /// </summary>
        public string Name
        {
            get { return "LCL_1SHFT"; }
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
            var originalRoute = route;
            var originalProblem = problem;
            var originalFitness = 0.0;
            if (!problem.Last.HasValue)
            { // the problem is 'open', convert to a closed equivalent.
                originalFitness = objective.Calculate(problem, route);
                problem = problem.ToClosed();
                route = new Route(route, problem.Last);
            }

            delta = 0;
            var success = false;

            var bestDelta = 0.0;
            do
            {
                bestDelta = 0.0;

                // search the entire route for a customer that can be moved to improve it.
                var bestTriple = new Triple();
                var bestPair = new Pair();
                foreach (var triple in route.Triples())
                { // the middle customer of each triple is a candidate for re-insertion.
                    foreach (var pair in route.Pairs())
                    { // each pair is a candidate to recieve the candidate customers.
                        if (pair.From != triple.Along &&
                            pair.To != triple.Along)
                        { // this candidate may fit here.
                            var localDelta = objective.IfShiftAfter(problem, route, triple.Along, pair.From, triple.From, triple.To, pair.To);
                            if (localDelta < bestDelta)
                            { // this means a (better) improvement.
                                bestDelta = localDelta;
                                bestTriple = triple;
                                bestPair = pair;
                            }
                        }
                    }
                }

                if (bestDelta < 0)
                { // if an improvement was found, then apply it.
                    // make the changes.
                    route.ShiftAfter(bestTriple.Along, bestPair.From);
                    // store the delta.
                    delta = delta + bestDelta;
                    success = true;
                }
            } while (bestDelta < 0);

            if (!originalProblem.Last.HasValue)
            { // the original problem was open, convert the route again.
                originalRoute.Clear();
                foreach(var pair in route.Pairs())
                {
                    if (pair.To != problem.First)
                    {
                        originalRoute.InsertAfter(pair.From, pair.To);
                    }
                }

                var newFitness = objective.Calculate(problem, originalRoute);
                delta = newFitness - originalFitness;
            }
            return success;
        }
    }
}