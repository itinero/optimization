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

using System.Collections.Generic;
using Itinero.Optimization.Solvers.Tours;
using Itinero.Optimization.Strategies;
using Itinero.Optimization.Strategies.Random;

namespace Itinero.Optimization.Solvers.TSP
{
    /// <summary>
    /// A solver that generates random solutions.
    /// </summary>
    internal sealed class RandomSolver : Strategy<TSProblem, Candidate<TSProblem, Tour>>
    {
        public override string Name => "RAN";
        
        public override Candidate<TSProblem, Tour> Search(TSProblem problem)
        {
            // generate random solution.
            var visits = new List<int>();
            for (var visit = 0; visit < problem.Count; visit++)
            {
                if (visit != problem.First &&
                    visit != problem.Last)
                {
                    visits.Add(visit);
                }
            }

            visits.Shuffle();
            visits.Insert(0, problem.First);
            if (problem.Last.HasValue && problem.First != problem.Last)
            { // the special case of a fixed last customer.
                visits.Add(problem.Last.Value);
            }
            var tour = new Tour(visits, problem.Last);

            return new Candidate<TSProblem, Tour>()
            {
                Problem = problem,
                Solution = tour,
                Fitness = problem.Weights(tour)
            };
        }
    }
}