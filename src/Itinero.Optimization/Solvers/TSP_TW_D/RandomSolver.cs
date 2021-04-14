using System.Collections.Generic;
using System.Threading;
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

            visits.Shuffle();
            visits.Insert(0, problem.First);
            if (problem.Last.HasValue && problem.First != problem.Last)
            { // the special case of a fixed last customer.
                visits.Add(problem.Last.Value);
            }
            var tour = new Tour(visits, problem.Last);

            return new Candidate<TSPTWDProblem, Tour>()
            {
                Problem = problem,
                Solution = tour,
                Fitness = tour.Fitness(problem)
            };
        }
        
        private static readonly ThreadLocal<RandomSolver> DefaultLazy = new ThreadLocal<RandomSolver>(() => new RandomSolver());
        public static RandomSolver Default => DefaultLazy.Value;
    }
}