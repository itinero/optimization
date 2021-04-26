using System.Threading;
using Itinero.Logging;
using Itinero.Optimization.Solvers.Tours;
using Itinero.Optimization.Solvers.TSP_TW_D.Operators;
using Itinero.Optimization.Strategies;
using Itinero.Optimization.Strategies.GA;
using Itinero.Optimization.Strategies.Tools.Selectors;

namespace Itinero.Optimization.Solvers.TSP_TW_D.EAX
{
    /// <summary>
    /// A solver using a GA and the edge-assembly crossover.
    /// </summary> 
    internal class EAXSolver : Strategy<TSPTWDProblem, Candidate<TSPTWDProblem, Tour>>
    {
        private readonly GAStrategy<TSPTWDProblem, Candidate<TSPTWDProblem, Tour>> _gaStrategy;
        
        /// <summary>
        /// Creates a new EAX-solver.
        /// </summary>
        public EAXSolver(Strategy<TSPTWDProblem, Candidate<TSPTWDProblem, Tour>>? generator = null, 
            Operator<Candidate<TSPTWDProblem, Tour>>? mutation = null, GASettings? settings = null, ISelector<Candidate<TSPTWDProblem, Tour>>? selector = null)
        {
            generator ??= RandomSolver.Default;
            mutation ??= new Operator<Candidate<TSPTWDProblem, Tour>>[]
            {
                Random1ShiftPerturber.Default,
                //Local2OptOperator.Default, 
            }.ApplyRandom();
            settings ??= new GASettings()
            {
                PopulationSize = 100
            };
            var crossOver = EAXOperator.Default;
            
            _gaStrategy = new GAStrategy<TSPTWDProblem, Candidate<TSPTWDProblem, Tour>>(generator, crossOver, mutation, settings, selector,
                useParallel: false);
            
            this.Name = $"EAX_{_gaStrategy.Name}";
        }

        public override string Name { get; }

        public override Candidate<TSPTWDProblem, Tour> Search(TSPTWDProblem problem)
        {
            if (problem.Count < 5)
            { // use brute-force solver when problem is very small.
                return BruteForceSolver.Default.Search(problem);
            }
            if (problem.Last == null)
            { // the problem is 'open', we need to convert the problem and then solution.
                Logger.Log($"{typeof(EAXSolver)}.{nameof(Search)}", TraceEventType.Warning,
                    "Performance warning: EAX solver cannot be applied to 'open' TSPs, converting problem to a closed equivalent.");

                var convertedProblem = problem.ClosedEquivalent;
                var convertedCandidate = _gaStrategy.Search(convertedProblem);
                
                // TODO: instead of recalculating the fitness value *should* be identical, confirm this with more testing.
                var tour = new Tour(convertedCandidate.Solution, null);

                return new Candidate<TSPTWDProblem, Tour>()
                {
                    Problem = problem,
                    Solution = tour,
                    Fitness = tour.Weight(problem.Weight)
                };
            }
            else if (problem.First != problem.Last)
            { // the problem is 'closed' but has a fixed end point.
                Logger.Log($"{typeof(EAXSolver)}.{nameof(Search)}", TraceEventType.Warning,
                    "Performance warning: EAX solver cannot be applied to 'closed' TSPs with a fixed endpoint: converting problem to a closed equivalent.");

                var convertedProblem = problem.ClosedEquivalent;
                var convertedCandidate = _gaStrategy.Search(convertedProblem);
                
                // TODO: instead of recalculating the fitness adjust it to match the difference by adding last->problem.last.
                var tour = convertedCandidate.Solution;
                tour.InsertAfter(System.Linq.Enumerable.Last(tour), problem.Last.Value);
                tour = new Tour(tour, problem.Last.Value);
                return new Candidate<TSPTWDProblem, Tour>()
                {
                    Problem = problem,
                    Solution = tour,
                    Fitness = tour.Weight(problem.Weight)
                };
            }
            else
            {
                return _gaStrategy.Search(problem);
            }
        }
        
        private static readonly ThreadLocal<EAXSolver> DefaultLazy = new ThreadLocal<EAXSolver>(() => new EAXSolver());
        public static EAXSolver Default => DefaultLazy.Value;
    }
}