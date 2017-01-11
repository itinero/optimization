// Itinero.Optimization - Route optimization for .NET
// Copyright (C) 2017 Abelshausen Ben
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

using Itinero.Optimization.Algorithms.CheapestInsertion;
using Itinero.Optimization.Algorithms.Random;
using Itinero.Optimization.Algorithms.Solvers;
using Itinero.Optimization.Tours;
using System.Collections.Generic;

namespace Itinero.Optimization.TSP.Directed.Solvers
{
    /// <summary>
    /// A solver that generates random solutions.
    /// </summary>
    public sealed class RandomSolver : SolverBase<float, TSProblem, TSPObjective, Tour, float>
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
        public sealed override Tour Solve(TSProblem problem, TSPObjective objective, out float fitness)
        {
            // generate random order for unplaced customers.
            var customers = new List<int>();
            for (var customer = 0; customer < problem.Weights.Length / 2; customer++)
            {
                if (customer != problem.First &&
                    customer != problem.Last)
                {
                    customers.Add(customer);
                }
            }
            customers.Shuffle<int>();

            // generate empty route based on problem definition.
            var route = problem.CreateEmptyTour();

            // add all customers by using cheapest insertion.
            for (var i = 0; i < customers.Count; i++)
            {
                CheapestInsertionDirectedHelper.InsertCheapestDirected(route, problem.Weights, problem.TurnPenalties,
                    customers[i], float.MaxValue);
            }

            // calculate fitness.
            fitness = objective.Calculate(problem, route);
            return route;
        }
    }
}
