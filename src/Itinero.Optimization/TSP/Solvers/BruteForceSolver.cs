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
using Itinero.Optimization.Tours;
using System.Collections.Generic;

namespace Itinero.Optimization.TSP.Solvers
{
    /// <summary>
    /// Implements a brute force solver by checking all possible combinations.
    /// </summary>
    public sealed class BruteForceSolver : SolverBase<float, TSProblem, TSPObjective, Tour, float>
    {
        /// <summary>
        /// Returns a new for this solver.
        /// </summary>
        public override string Name
        {
            get
            {
                return "BF";
            }
        }

        /// <summary>
        /// Solves the given problem.
        /// </summary>
        /// <returns></returns>
        public sealed override Tour Solve(TSProblem problem, TSPObjective objective, out float fitness)
        {
            // initialize.
            var solution = new List<int>();
            for (int customer = 0; customer < problem.Weights.Length; customer++)
            { // add each customer again.
                if (customer != problem.First &&
                    customer != problem.Last)
                {
                    solution.Add(customer);
                }
            }

            if (solution.Count < 2)
            { // a tiny problem.
                // build route.
                var withFirst = new List<int>(solution);
                withFirst.Insert(0, problem.First);
                if (problem.Last.HasValue && problem.First != problem.Last)
                { // the special case of a fixed last customer.
                    withFirst.Add(problem.Last.Value);
                }
                var route = new Tour(withFirst, problem.Last);
                fitness = objective.Calculate(problem, route);
                return route;
            }

            // keep on looping until all the permutations 
            // have been considered.
            var enumerator = new PermutationEnumerable<int>(
                solution.ToArray());
            Tour bestSolution = null;
            var bestFitness = float.MaxValue;
            foreach (var permutation in enumerator)
            {
                // build route from permutation.
                var withFirst = new List<int>(permutation);
                withFirst.Insert(0, problem.First);
                if (problem.Last.HasValue && problem.First != problem.Last)
                { // the special case of a fixed last customer.
                    withFirst.Add(problem.Last.Value);
                }
                var localRoute = new Tour(withFirst, problem.Last);

                // calculate fitness.
                var localFitness = objective.Calculate(problem, localRoute);
                if (localFitness < bestFitness)
                { // the best weight has improved.
                    bestFitness = localFitness;
                    bestSolution = localRoute;
                }
            }
            fitness = bestFitness;
            return bestSolution;
        }
    }
}