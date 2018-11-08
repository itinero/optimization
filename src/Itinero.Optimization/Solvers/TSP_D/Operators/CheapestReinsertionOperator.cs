// TODO: This is broken but could be repaired.

///*
// *  Licensed to SharpSoftware under one or more contributor
// *  license agreements. See the NOTICE file distributed with this work for 
// *  additional information regarding copyright ownership.
// * 
// *  SharpSoftware licenses this file to you under the Apache License, 
// *  Version 2.0 (the "License"); you may not use this file except in 
// *  compliance with the License. You may obtain a copy of the License at
// * 
// *       http://www.apache.org/licenses/LICENSE-2.0
// * 
// *  Unless required by applicable law or agreed to in writing, software
// *  distributed under the License is distributed on an "AS IS" BASIS,
// *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// *  See the License for the specific language governing permissions and
// *  limitations under the License.
// */
//
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Threading;
//using Itinero.Optimization.Solvers.Shared.Directed;
//using Itinero.Optimization.Solvers.Shared.Directed.CheapestInsertion;
//using Itinero.Optimization.Solvers.Tours;
//using Itinero.Optimization.Strategies;
//using Itinero.Optimization.Strategies.Random;
//
//namespace Itinero.Optimization.Solvers.TSP_D.Operators
//{
//    /// <summary>
//    /// A reinsertion operator, takes out a fraction of the visits and reinserts them using CI.
//    /// </summary>
//    public class CheapestReinsertionOperator : Operator<Candidate<TSPDProblem, Tour>>
//    {
//        private readonly float _fraction;
//
//        /// <summary>
//        /// Creates a new operator.
//        /// </summary>
//        /// <param name="fraction">The fraction in ]0, 1] of the visits that will be removed and reinserted.</param>
//        public CheapestReinsertionOperator(float fraction = 0.25f)
//        {
//            _fraction = fraction;
//        }
//
//        public override string Name => $"RE_CI_{_fraction}";
//
//        public override bool Apply(Candidate<TSPDProblem, Tour> candidate)
//        {
//            var tour = candidate.Solution;
//            var problem = candidate.Problem;
//
//            // select fraction of the visits and randomize the list. 
//            var visits = new List<(int visit, int directedVisit)>();
//            foreach (var directedVisit in candidate.Solution)
//            {
//                if (directedVisit == candidate.Solution.First) continue;
//                if (directedVisit == candidate.Solution.Last) continue;
//                if (Strategies.Random.RandomGenerator.Default.Generate(1f) < _fraction)
//                {
//                    visits.Add((DirectedHelper.ExtractVisit(directedVisit), directedVisit));
//                }
//            }
//            if (visits.Count == 0) return false;
//            
//            visits.Shuffle();
//
//            // remove selected visits.
//            var originalTour = tour.Clone() as Tour;
//            foreach (var visitPair in visits)
//            {
//                tour.Remove(visitPair.directedVisit);
//            }
//                
//            // add all visits by using cheapest insertion.
//            foreach (var visitPair in visits)
//            {
//                var result = tour.CalculateCheapestDirected(visitPair.visit, problem.Weight, problem.TurnPenalty);
//                tour.InsertAfter(result.location.From, result.turn.DirectedVisit(visitPair.visit));
//                var from = DirectedHelper.Extract(result.location.From);
//                var to = DirectedHelper.Extract(result.location.To);
//                var fromDirectedVisit = from.turn.ApplyDeparture(result.fromDepartureDirection)
//                    .DirectedVisit(from.visit);
//                var toDirectedVisit = to.turn.ApplyArrival(result.toArrivalDirection)
//                    .DirectedVisit(to.visit);
//                if (result.location.From != fromDirectedVisit)
//                {
//                    tour.Replace(result.location.From, fromDirectedVisit);
//                }
//                if (from.visit != to.visit &&
//                    result.location.To != toDirectedVisit)
//                {
//                    tour.Replace(result.location.To, toDirectedVisit);
//                }
//            }
//            var fitness = tour.WeightDirected(problem.Weight, problem.TurnPenalty);
//            if (fitness >= float.MaxValue)
//            { // tour became impossible.
//                candidate.Solution = originalTour;
//                return false;
//            }
//
//            var success = fitness < candidate.Fitness;
//            candidate.Fitness = fitness;
//            return success;
//        }
//        
//        private static readonly ThreadLocal<CheapestReinsertionOperator> DefaultLazy = new ThreadLocal<CheapestReinsertionOperator>(() => new CheapestReinsertionOperator());
//        public static CheapestReinsertionOperator Default => DefaultLazy.Value;
//    }
//}