// Itinero.Optimization - Route optimization for .NET
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

using Itinero.Optimization.Algorithms.CheapestInsertion;
using Itinero.Optimization.Algorithms.Random;
using Itinero.Optimization.Algorithms.Solvers;
using Itinero.Optimization.Routes;
using System.Collections.Generic;
using System.Linq;

namespace Itinero.Optimization.STSP.Solvers
{
    /// <summary>
    /// A solver that generates random solutions.
    /// </summary>
    public sealed class RandomSolver : SolverBase<float, STSProblem, STSPObjective, Route, STSPFitness>
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
        public sealed override Route Solve(STSProblem problem, STSPObjective objective, out STSPFitness fitness)
        {
            // generate random pool to select customers from.
            if (_randomPool == null || _randomPool.Size < problem.Weights.Length)
            {
                _randomPool = new RandomPool(problem.Weights.Length);
            }
            else
            {
                _randomPool.Reset();
            }

            // keep adding customers until no more space is left or no more customers available.
            var route = new Route(new int[] { problem.First });
            fitness = new STSPFitness()
            {
                Weight = 0,
                Customers = 1
            };
            while(_randomPool.MoveNext())
            {
                var customer = _randomPool.Current;
                if (customer == problem.First)
                {
                    continue;
                }

                Pair location;
                var cost = CheapestInsertion.CalculateCheapest(route, problem.Weights, customer, out location);
                if (cost + fitness.Weight < problem.Max)
                {
                    route.InsertAfter(location.From, customer);

                    fitness.Weight = fitness.Weight + cost;
                    fitness.Customers = fitness.Customers + 1;
                }
            }

            // calculate fitness.
            fitness = objective.Calculate(problem, route);
            return route;
        }
    }
}
