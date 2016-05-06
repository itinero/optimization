// Itinero.Logistics - Route optimization for .NET
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

using Itinero.Logistics.Collections;
using Itinero.Logistics.Routes;
using Itinero.Logistics.Solutions.Algorithms;
using Itinero.Logistics.Solvers;

namespace Itinero.Logistics.Solutions.STSP.Random
{
    /// <summary>
    /// Just generates random feasible solutions.
    /// </summary>
    public class RandomSolver : SolverBase<ISTSP, ISTSPObjective, IRoute>
    {
        private const int MAX_SECOND_ATTEMPTS = 10000;

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
        public override IRoute Solve(ISTSP problem, ISTSPObjective objective, out double fitness)
        {
            // create initial empty solution
            var route = problem.EmptyRoute();

            fitness = objective.Calculate(problem, route);
            if (fitness > problem.Max)
            { // no solution possible.
                Itinero.Logistics.Logging.Logger.Log("RandomSolver", Logging.TraceEventType.Error, "Problem is unfeasible.");
                return null;
            }

            // randomly insert and best-place customer.
            var pool = new RandomSet<int>();
            for (var customer = 0; customer < problem.Weights.Length; customer++)
            {
                if (customer != problem.First &&
                    customer != problem.Last)
                {
                    pool.Add(customer);
                }
            }

            // if route is only one customer insert a rand customer to begin with.
            if (route.Count == 1)
            {
                var attempt = 0;
                while (true)
                {
                    var c = pool.GetRandom();
                    var cost = problem.Weights[route.First][c] +
                        problem.Weights[c][route.First];
                    if (cost < problem.Max)
                    {
                        route.InsertAfter(route.First, c);
                        pool.Remove(c);
                        fitness = cost;
                        break;
                    }
                    if (attempt == MAX_SECOND_ATTEMPTS)
                    { // do not keep trying forever, limit the amount of tries to catch unsolvable instances.
                        return route;
                    }
                    attempt++;
                }
            }

            // select random customer.
            while(pool.Count > 0)
            {
                var c = pool.RemoveRandom();
                Pair location;
                var cost = route.CalculateCheapest(problem, c, out location);
                if (cost != float.MaxValue &&
                    cost + fitness < problem.Max)
                {
                    route.InsertAfter(location.From, c);
                    fitness += cost;
                }
            }

            // calculate fitness.
            fitness = objective.Calculate(problem, route);
            return route;
        }
    }
}