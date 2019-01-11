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
using System.Threading;
using Itinero.Optimization.Solvers.Shared.Directed;
using Itinero.Optimization.Solvers.Shared.Directed.CheapestInsertion;
using Itinero.Optimization.Solvers.Tours;
using Itinero.Optimization.Strategies;
using Itinero.Optimization.Strategies.Random;

namespace Itinero.Optimization.Solvers.TSP_D
{
    /// <summary>
    /// A solver that generates random solutions.
    /// </summary>
    internal sealed class RandomSolver : Strategy<TSPDProblem, Candidate<TSPDProblem, Tour>>
    {
        public override string Name => "RAN";
        
        public override Candidate<TSPDProblem, Tour> Search(TSPDProblem problem)
        {
            // create an empty tour.
            Tour tour = null;
            if (problem.Last.HasValue)
            {
                var last = TurnEnum.ForwardForward.DirectedVisit(problem.Last.Value);
                tour = new Tour(new [] { TurnEnum.ForwardForward.DirectedVisit(problem.First), last }, last);
            }
            else
            {
                tour = new Tour(new [] { TurnEnum.ForwardForward.DirectedVisit(problem.First) }, null);
            }
            
            
            if (problem.WeightsSize < 2)
            { // there is only one visit.
                return new Candidate<TSPDProblem, Tour>()
                {
                    Problem = problem,
                    Solution = tour,
                    Fitness = tour.Weight(problem.Weight)
                };
            }
            else
            { // randomize visits and insert them.
                var visits = new List<int>();
                foreach (var visit in problem.Visits)
                {
                    if (visit != problem.First &&
                        visit != problem.Last)
                    {
                        visits.Add(visit);
                    }
                }

                visits.Shuffle();
                
                // add all visits by using cheapest insertion.
                for (var i = 0; i < visits.Count; i++)
                {
                    var result = tour.CalculateCheapestDirected(visits[i], problem.Weight, problem.TurnPenalty);
                    tour.InsertAfter(result.location.From, result.turn.DirectedVisit(visits[i]));
                    var from = DirectedHelper.Extract(result.location.From);
                    var to = DirectedHelper.Extract(result.location.To);
                    var fromDirectedVisit = from.turn.ApplyDeparture(result.fromDepartureDirection)
                        .DirectedVisit(from.visit);
                    var toDirectedVisit = to.turn.ApplyArrival(result.toArrivalDirection)
                        .DirectedVisit(to.visit);
                    if (result.location.From != fromDirectedVisit)
                    {
                        tour.Replace(result.location.From, fromDirectedVisit);
                    }
                    if (from.visit != to.visit &&
                        result.location.To != toDirectedVisit)
                    {
                        tour.Replace(result.location.To, toDirectedVisit);
                    }
                }
                
                return new Candidate<TSPDProblem, Tour>()
                {
                    Problem = problem,
                    Solution = tour,
                    Fitness = tour.WeightDirected(problem.Weight, problem.TurnPenalty)
                };
            }
        }
        
        private static readonly ThreadLocal<RandomSolver> DefaultLazy = new ThreadLocal<RandomSolver>(() => new RandomSolver());
        public static RandomSolver Default => DefaultLazy.Value;
    }
}