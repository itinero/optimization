// Itinero.Logistics - Route optimization for .NET
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

using Itinero.Logistics.Algorithms;
using Itinero.Logistics.Routes;
using Itinero.Logistics.Solvers;
using System.Collections.Generic;

namespace Itinero.Logistics.Solutions.TSP.Random
{
    /// <summary>
    /// Just generates random solutions.
    /// </summary>
    public class RandomSolver<T> : SolverBase<T, ITSP<T>, ITSPObjective<T>, IRoute>
        where T : struct
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
        public override IRoute Solve(ITSP<T> problem, ITSPObjective<T> objective, out float fitness)
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
            var route = new Logistics.Routes.Route(customers, problem.Last);

            // calculate fitness.
            fitness = objective.Calculate(problem, route);
            return route;
        }
    }
}