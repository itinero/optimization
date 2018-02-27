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
using Itinero.Optimization.Algorithms.Random;
using Itinero.Optimization.Algorithms.Solvers;
using Itinero.Optimization.Tours;
using System.Collections.Generic;

namespace Itinero.Optimization.Solutions.TSP.Directed.Solvers
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
            if (problem.Weights.Length <= 2)
            {
                fitness = 0;
                return new Tour(new int[] { problem.First });
            }

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
