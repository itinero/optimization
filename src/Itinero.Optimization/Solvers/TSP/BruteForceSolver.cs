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

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Itinero.Optimization.Solvers.Tours;
using Itinero.Optimization.Strategies;

[assembly: InternalsVisibleTo("Itinero.Optimization.Tests")]
namespace Itinero.Optimization.Solvers.TSP
{
    /// <summary>
    /// Implements a brute force solver by checking all possible combinations.
    /// </summary>
    internal sealed class BruteForceSolver : Strategy<TSProblem, Candidate<TSProblem, Tour>>
    {
        public override string Name => "BF";

        public override Candidate<TSProblem, Tour> Search(TSProblem problem)
        {
            // initialize.
            var visits = new List<int>();
            foreach (var visit in problem.Visits)
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
                var tour = new Tour(withFirst, problem.Last);
                return new Candidate<TSProblem, Tour>()
                {
                    Solution = tour,
                    Problem = problem,
                    Fitness = problem.Weights(tour)
                };
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
                var localFitness = problem.Weights(localRoute);
                if (!(localFitness < bestFitness)) continue; // the best weight has improved.
                bestFitness = localFitness;
                bestSolution = localRoute;
            }
            return new Candidate<TSProblem, Tour>()
            {
                Solution = bestSolution,
                Problem = problem,
                Fitness = bestFitness
            };
        }
        
        private static readonly ThreadLocal<BruteForceSolver> DefaultLazy = new ThreadLocal<BruteForceSolver>(() => new BruteForceSolver());
        public static BruteForceSolver Default => DefaultLazy.Value;
    }
}