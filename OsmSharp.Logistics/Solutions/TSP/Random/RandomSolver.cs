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
using System.Collections.Generic;

namespace OsmSharp.Logistics.Solutions.TSP.Random
{
    /// <summary>
    /// Just generates random solutions.
    /// </summary>
    public class RandomSolver : SolverBase<ITSP, ITSPObjective, IRoute>
    {
        /// <summary>
        /// Returns the name of this solver.
        /// </summary>
        public override string Name
        {
            get { return "RAN"; }
        }

        /// <summary>
        /// Solves the given problem.
        /// </summary>
        /// <returns></returns>
        public override IRoute Solve(ITSP problem, ITSPObjective objective, out double fitness)
        {
            // generate random solution.
            var customers = new List<int>();
            for (var customer = 0; customer < problem.Weights.Length; customer++)
            {
                if (customer != problem.First &&
                    customer != problem.Last)
                {
                    customers.Add(customer);
                }
            }
            customers.Shuffle<int>();
            customers.Insert(0, problem.First);
            if (problem.Last.HasValue && problem.First != problem.Last)
            { // the special case of a fixed last customer.
                customers.Add(problem.Last.Value);
            }
            var route = new Route(customers, problem.Last);

            // calculate fitness.
            fitness = objective.Calculate(problem, route);
            return route;
        }
    }
}