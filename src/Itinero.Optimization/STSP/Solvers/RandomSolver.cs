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

namespace Itinero.Optimization.STSP.Solvers
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
                _randomPool = new RandomPool(problem.Weights.Length);
            }
            else
            {
                _randomPool.Reset();
            }

            // keep adding customers until no more space is left or no more customers available.
            var tour = problem.CreateEmptyTour();
            fitness = new STSPFitness()
            {
                Weight = 0,
                Customers = 1
            };
            while(_randomPool.MoveNext())
            {
                var customer = _randomPool.Current;
                if (customer == problem.First ||
                    customer == problem.Last)
                {
                    continue;
                }

                Pair location;
                var cost = CheapestInsertionHelper.CalculateCheapest(tour, problem.Weights, customer, out location);
                if (cost + fitness.Weight < problem.Max)
                {
                    tour.InsertAfter(location.From, customer);

                    fitness.Weight = fitness.Weight + cost;
                    fitness.Customers = fitness.Customers + 1;
                }
            }

            // calculate fitness.
            fitness = objective.Calculate(problem, tour);
            return tour;
        }
    }
}
