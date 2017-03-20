/*
 *  Licensed to SharpSoftware under one or more contributor
 *  license agreements. See the NOTICE file distributed with this work for 
 *  additional information regarding copyright ownership.
 * 
 *  SharpSoftware licenses this file to you under the Apache License, 
 *  Version 2.0 (the "License"); you may not use this file except in 
 *  compliance with the License. You may obtain a copy of the License at
 * 
 *       http://www.apache.org/licenses/LICENSE-2.0
 * 
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */

using Itinero.Optimization.Algorithms.CheapestInsertion;
using Itinero.Optimization.Algorithms.Directed;
using Itinero.Optimization.Algorithms.Random;
using Itinero.Optimization.Algorithms.Solvers;
using Itinero.Optimization.Tours;

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
            var route = problem.CreateEmptyTour();
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
                if (customer == DirectedHelper.ExtractId(route.First) ||
                    (route.Last.HasValue && customer == DirectedHelper.ExtractId(route.Last.Value)))
                { // customer is first or last.
                    continue;
                }
                
                var cost = CheapestInsertionDirectedHelper.InsertCheapestDirected(route, problem.Weights, problem.TurnPenalties,
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