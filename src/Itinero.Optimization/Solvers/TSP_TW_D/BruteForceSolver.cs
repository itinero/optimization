using System.Collections.Generic;
using System.Threading;
using Itinero.Optimization.Solvers.Tours;
using Itinero.Optimization.Strategies;

namespace Itinero.Optimization.Solvers.TSP_TW_D
{

    /// <summary>
    /// Implements a brute force solver by checking all possible combinations.
    /// </summary>
    internal sealed class BruteForceSolver : Strategy<TSPTWDProblem, Candidate<TSPTWDProblem, Tour>>
    {
        public override string Name => "BF";

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
                return new Candidate<TSPTWDProblem, Tour>()
                {
                    Solution = tour,
                    Problem = problem,
                    Fitness = tour.Weight(problem.Weight)
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
                var localFitness = localRoute.Fitness(problem);
                if (!(localFitness < bestFitness)) continue; // the best weight has improved.
                bestFitness = localFitness;
                bestSolution = localRoute;
            }
            return new Candidate<TSPTWDProblem, Tour>()
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