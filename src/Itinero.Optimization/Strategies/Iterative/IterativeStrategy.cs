using System;

namespace Itinero.Optimization.Strategies.Iterative
{
    /// <summary>
    /// A strategy that uses another strategy a number of times and selects the best solution.
    /// </summary>
    /// <typeparam name="TProblem">The problem type.</typeparam>
    /// <typeparam name="TCandidate">The candidate type.</typeparam>
    internal sealed class IterativeStrategy<TProblem, TCandidate> : Strategy<TProblem, TCandidate>
    {
        private readonly Strategy<TProblem, TCandidate> _strat;
        private readonly int _n;

        /// <summary>
        /// Creates a new strategy.
        /// </summary>
        /// <param name="strat">The strategy.</param>
        /// <param name="n">The numer of times to repeat.</param>
        public IterativeStrategy(Strategy<TProblem, TCandidate> strat, int n)
        {
            _strat = strat;
            _n = n;

            Name = strat.Name + "x" + _n.ToString();
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name { get; }

        /// <summary>
        /// Uses this strategy on the given problem and returns the best candidate.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <returns>A candidate.</returns>
        public override TCandidate Search(TProblem problem)
        {
            return Iterate(_strat, problem, _n);
        }

        internal static TCandidate Iterate(Strategy<TProblem, TCandidate> strategy, TProblem p, int n)
        {
            var best = strategy.Search(p);
            while (n > 1)
            {
                var next = strategy.Search(p);
                if (CandidateComparison.Compare(best, next) > 0)
                {
                    best = next;
                }
                n--;
            }
            return best;
        }
    }
}