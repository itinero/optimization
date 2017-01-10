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

using Itinero.Optimization.Algorithms.CheapestInsertion;
using Itinero.Optimization.Algorithms.Directed;
using Itinero.Optimization.Algorithms.Random;
using Itinero.Optimization.Algorithms.Solvers;
using Itinero.Optimization.Tours;
using System.Collections.Generic;
using System.Linq;

namespace Itinero.Optimization.STSP.Directed.Solver
{
    /// <summary>
    /// A solver that generates random solutions.
    /// </summary>
    /// <remarks>
    /// Executes a few steps:
    /// - Initialize route based on problem definition with proper first/last customers.
    /// - Go over all customers not in the route in some random order and try to cheapest-insert them.
    /// </remarks>
    public sealed class RandomSolver : SolverBase<float, STSProblem, STSPObjective, Tour, STSPFitness>
    {
        /// <summary>
        /// Returns the name of this solver.
        /// </summary>
        public override string Name
        {
            get { return "RAN"; }
        }

        private RandomPool _randomPool;

        /// <summary>
        /// Solves the given problem.
        /// </summary>
        /// <returns></returns>
        public sealed override Tour Solve(STSProblem problem, STSPObjective objective, out STSPFitness fitness)
        {
            // generate empty route based on problem definition.
            var last = problem.Last;
            if (last.HasValue)
            {
                last = DirectedHelper.BuildDirectedId(last.Value, 0);
            }
            var route = new Tour(new int[] { DirectedHelper.BuildDirectedId(problem.First, 0) }, last);
            fitness = new STSPFitness()
            {
                Weight = 0,
                Customers = 1
            };

            // generate random pool to select customers from.
            if (_randomPool == null || _randomPool.Size < problem.Weights.Length)
            {
                _randomPool = new RandomPool(problem.Weights.Length / 2);
            }
            else
            {
                _randomPool.Reset();
            }

            // keep adding customers until no more space is left or no more customers available.
            while (_randomPool.MoveNext())
            {
                var customer = _randomPool.Current;
                if (customer == DirectedHelper.ExtractId(problem.First) ||
                    (problem.Last.HasValue && customer == DirectedHelper.ExtractId(problem.Last.Value)))
                { // customer is first or last.
                    continue;
                }
                
                var cost = CheapestInsertionHelper.InsertCheapestDirected(route, problem.Weights, problem.TurnPenalties,
                    customer, problem.Max - fitness.Weight);
                if (cost > 0)
                {
                    fitness.Customers++;
                    fitness.Weight += cost;
                }
            }

            // calculate fitness.
            fitness = objective.Calculate(problem, route);
            return route;
        }
    }
}