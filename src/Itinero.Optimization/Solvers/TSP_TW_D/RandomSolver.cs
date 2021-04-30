using System.Collections.Generic;
using System.Threading;
using Itinero.Optimization.Solvers.Shared.Directed;
using Itinero.Optimization.Solvers.Shared.Directed.CheapestInsertion;
using Itinero.Optimization.Solvers.Tours;
using Itinero.Optimization.Strategies;
using Itinero.Optimization.Strategies.Random;

namespace Itinero.Optimization.Solvers.TSP_TW_D
{
    /// <summary>
    /// A solver that generates random solutions.
    /// </summary>
    internal sealed class RandomSolver : Strategy<TSPTWDProblem, Candidate<TSPTWDProblem, Tour>>
    {
        public override string Name => "RAN";
        
        public override Candidate<TSPTWDProblem, Tour> Search(TSPTWDProblem problem)
        {
            // create an empty tour.
            Tour? tour = null;
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
                return new Candidate<TSPTWDProblem, Tour>()
                {
                    Problem = problem,
                    Solution = tour,
                    Fitness = tour.Fitness(problem)
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
                
                return new Candidate<TSPTWDProblem, Tour>()
                {
                    Problem = problem,
                    Solution = tour,
                    Fitness = tour.Fitness(problem)
                };
            }
        }
        
        private static readonly ThreadLocal<RandomSolver> DefaultLazy = new ThreadLocal<RandomSolver>(() => new RandomSolver());
        public static RandomSolver Default => DefaultLazy.Value;
    }
}