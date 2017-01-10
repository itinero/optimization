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
            while (_randomPool.MoveNext())
            {
                var customer = _randomPool.Current;
                if (customer == DirectedHelper.ExtractId(problem.First))
                {
                    continue;
                }

                if (route.Count == 1)
                { // there is only one customer, the first one in the route.
                    if (problem.First == problem.Last)
                    { // there is one customer but it's both the start and the end.
                        int departureOffset1, arrivalOffset3, turn2;
                        var cost = DirectedHelper.CheapestInsert(problem.Weights, problem.TurnPenalties,
                            problem.First, customer, problem.Last.Value, out departureOffset1, out arrivalOffset3, out turn2);
                        if (cost + fitness.Weight <= problem.Max)
                        {
                            var newFirst = DirectedHelper.UpdateDepartureOffset(
                            DirectedHelper.UpdateArrivalOffset(route.First, arrivalOffset3), departureOffset1);
                            route.Replace(route.First, newFirst);
                            var customerDirectedId = DirectedHelper.BuildDirectedId(customer, turn2);

                            route.InsertAfter(route.First, customerDirectedId);

                            fitness.Customers++;
                            fitness.Weight += cost;
                        }
                    }
                    else
                    { // there is one customer, the last one is not set.
                        int departureOffset1, arrivalOffset2;
                        var cost = DirectedHelper.CheapestInsert(problem.Weights, problem.TurnPenalties,
                            problem.First, customer, out departureOffset1, out arrivalOffset2);
                        if (cost + fitness.Weight <= problem.Max)
                        {
                            var newFirst = DirectedHelper.UpdateDepartureOffset(
                                DirectedHelper.BuildDirectedId(route.First, 0), departureOffset1);
                            var customerDirectedId = DirectedHelper.UpdateArrivalOffset(
                                DirectedHelper.BuildDirectedId(customer, 0), arrivalOffset2);

                            route.InsertAfter(route.First, customerDirectedId);

                            fitness.Customers++;
                            fitness.Weight += cost;
                        }
                    }
                }
                else
                { // at least 2 customers already exist, insert a new one in between.
                    Pair location;
                    int departureOffsetFrom, arrivalOffsetTo, turn;
                    var cost = CheapestInsertion.CalculateCheapestDirected(route, problem.Weights, problem.TurnPenalties, customer, out location,
                        out departureOffsetFrom, out arrivalOffsetTo, out turn);
                    if (cost + fitness.Weight <= problem.Max)
                    {
                        route.InsertDirected(customer, location, departureOffsetFrom, arrivalOffsetTo, turn);

                        fitness.Weight = fitness.Weight + cost;
                        fitness.Customers = fitness.Customers + 1;
                    }
                }
            }

            // calculate fitness.
            fitness = objective.Calculate(problem, route);
            return route;
        }
    }
}
