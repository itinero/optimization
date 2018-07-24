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
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using Itinero.Optimization.Solvers.Shared.CheapestInsertion;
using Itinero.Optimization.Solvers.Tours;
using Itinero.Optimization.Strategies.GA;
using Itinero.Optimization.Strategies.Random;

namespace Itinero.Optimization.Solvers.STSP.Operators
{
    internal class VisitExchangeOperator : CrossOverOperator<Candidate<STSProblem, Tour>>
    {
        public override string Name { get; } = "EX";
        
        public override Candidate<STSProblem, Tour> Apply(Candidate<STSProblem, Tour> candidate1, Candidate<STSProblem, Tour> candidate2)
        {
            // randomly take visits from candidate1 and candidate2 and try to place them until the target is full.
            
            var problem = candidate1.Problem;
            var tour = problem.CreateEmptyTour();
            var fitness = 0f;
            var visits1 = new List<int>(candidate1.Solution);
            visits1.Shuffle();
            var visits2 = new List<int>(candidate2.Solution);
            visits2.Shuffle();
            
            var placed = new HashSet<int>();
            foreach (var visit in tour)
            {
                placed.Add(visit);
            }
            while (visits1.Count > 0 &&
                   visits2.Count > 0)
            {
                if (visits1.Count > 0)
                {
                    var next = visits1[visits1.Count - 1];
                    visits1.RemoveAt(visits1.Count - 1);
                    if (!placed.Contains(next))
                    {
                        placed.Add(next);
                        var result = tour.CalculateCheapest(problem.Weight, next);
                        var cost = result.cost;
                        var location = result.location;
                        if (!(cost + fitness <= problem.Max)) continue;
                        tour.InsertAfter(location.From, next);
                        fitness += cost;
                    }
                }

                if (visits2.Count > 0)
                {
                    var next = visits2[visits2.Count - 1];
                    visits2.RemoveAt(visits2.Count - 1);
                    if (!placed.Contains(next))
                    {
                        placed.Add(next);
                        var result = tour.CalculateCheapest(problem.Weight, next);
                        var cost = result.cost;
                        var location = result.location;
                        if (!(cost + fitness <= problem.Max)) continue;
                        tour.InsertAfter(location.From, next);
                        fitness += cost;
                    }
                }
            }

            return new Candidate<STSProblem, Tour>()
            {
                Fitness = tour.Fitness(problem),
                Problem = problem,
                Solution = tour
            };
        }
        
        private static readonly ThreadLocal<VisitExchangeOperator> DefaultLazy = new ThreadLocal<VisitExchangeOperator>(() => new VisitExchangeOperator());
        public static VisitExchangeOperator Default => DefaultLazy.Value;
    }
}