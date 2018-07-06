using System.Threading;
using Itinero.Optimization.Solvers.CVRP.Operators;
using Itinero.Optimization.Solvers.CVRP.SCI;
using Itinero.Optimization.Strategies;
using Itinero.Optimization.Strategies.GA;

namespace Itinero.Optimization.Solvers.CVRP.GA
{
    /// <summary>
    /// A solver using a GA and the edge-assembly crossover.
    /// </summary> 
    internal class GASolver : Strategy<CVRProblem, CVRPCandidate>
    {
        private readonly GAStrategy<CVRProblem, CVRPCandidate> _gaStrategy;
        
        /// <summary>
        /// Creates a new GA solver.
        /// </summary>
        /// <param name="generator">The generator strategy, if any.</param>
        /// <param name="mutation">The mutation operator, if any.</param>
        /// <param name="crossOver">The cross over operator, if any.</param>
        /// <param name="settings">The settings, if not default.</param>
        public GASolver(Strategy<CVRProblem, CVRPCandidate> generator = null, Operator<CVRPCandidate> mutation = null,
            CrossOverOperator<CVRPCandidate> crossOver = null, GASettings settings = null)
        {
            generator = generator ?? new SeededCheapestInsertionStrategy();
            mutation = mutation ?? new RedoPlacementOperator();
            crossOver = crossOver ?? new TourExchangeCrossOverOperator();
            settings = settings ?? GASettings.Default;

            _gaStrategy = new GAStrategy<CVRProblem, CVRPCandidate>(
                generator, crossOver, mutation, settings);
        }

        public override string Name => _gaStrategy.Name;

        public override CVRPCandidate Search(CVRProblem problem)
        {
            return _gaStrategy.Search(problem);
        }
        
        private static readonly ThreadLocal<GASolver> DefaultLazy = new ThreadLocal<GASolver>(() => new GASolver());
        public static GASolver Default => DefaultLazy.Value;
    }
}