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

using Itinero.Optimization.Algorithms.Random;
using Itinero.Optimization.Algorithms.Solvers;
using Itinero.Optimization.Algorithms.Solvers.Objective;
using Itinero.Optimization.Tours;
using System.Collections.Generic;

namespace Itinero.Optimization.TSP.TimeWindows.Directed.Solvers
{
    /// <summary>
    /// A solver that generates random solutions.
    /// </summary>
    public sealed class RandomSolver<TObjective> : SolverBase<float, TSPTWProblem, TObjective, Tour, float>
        where TObjective : ObjectiveBase<TSPTWProblem, Tour, float>
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
        public sealed override Tour Solve(TSPTWProblem problem, TObjective objective, out float fitness)
        {
            // TODO: convert this to a cheapest-insertion scheme similar to STSP.
            var random = RandomGeneratorExtensions.GetRandom();

            // generate random solution.
            var customers = new List<int>();
            for (var customer = 0; customer < problem.Times.Length / 2; customer++)
            {
                if (customer != problem.First &&
                    customer != problem.Last)
                {
                    customers.Add(customer);
                }
            }
            customers.Shuffle();
            customers.Insert(0, problem.First);
            if (problem.Last.HasValue && problem.First != problem.Last)
            { // the special case of a fixed last customer.
                customers.Add(problem.Last.Value);
            }

            // convert customers to directed customer id's.
            for (var i = 0; i < customers.Count; i++)
            {
                customers[i] = Algorithms.Directed.DirectedHelper.BuildDirectedId(customers[i],
                    random.Generate(4));
            }

            // build the route.
            Tour route = null;
            if (problem.Last.HasValue)
            {
                if (problem.Last == problem.First)
                {
                    route = new Tours.Tour(customers, customers[0]);
                }
                else
                {
                    route = new Tours.Tour(customers, customers[customers.Count - 1]);
                }
            }
            else
            {
                route = new Tours.Tour(customers, null);
            }

            // calculate fitness.
            fitness = objective.Calculate(problem, route);
            return route;
        }
    }
}