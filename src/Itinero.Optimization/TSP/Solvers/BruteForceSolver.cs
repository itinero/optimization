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

using Itinero.Optimization.Algorithms.Solvers;
using Itinero.Optimization.Tours;
using System.Collections.Generic;

namespace Itinero.Optimization.TSP.Solvers
{
    /// <summary>
    /// Implements a brute force solver by checking all possible combinations.
    /// </summary>
    public sealed class BruteForceSolver : SolverBase<float, ITSProblem, TSPObjective, Tour, float>
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
        public sealed override Tour Solve(ITSProblem problem, TSPObjective objective, out float fitness)
        {
            // initialize.
            var visits = new List<int>();
            for (var visit = 0; visit < problem.Count; visit++)
            {
                if (visit != problem.First &&
                    visit != problem.Last)
                {
                    visits.Add(visit);
                }
            }

            if (visits.Count < 2)
            { // a tiny problem.
                // build route.
                var withFirst = new List<int>(visits);
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
                visits.ToArray());
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