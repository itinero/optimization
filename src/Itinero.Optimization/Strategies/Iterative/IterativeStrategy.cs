using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

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
        private readonly bool _useParallel = true;
        private readonly Func<TCandidate, bool> _stop;

        /// <summary>
        /// Creates a new strategy.
        /// </summary>
        /// <param name="strat">The strategy.</param>
        /// <param name="n">The number of times to repeat.</param>
        /// <param name="stop">The stop condition if any.</param>
        /// <param name="useParallel">Flag to control parallelism.</param>
        public IterativeStrategy(Strategy<TProblem, TCandidate> strat, int n, Func<TCandidate, bool> stop = null, bool useParallel = false)
        {
            _strat = strat;
            _n = n;
            _useParallel = useParallel;
            _stop = stop;

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
            return Iterate(_strat, problem, _n, _stop, _useParallel);
        }

        internal static TCandidate Iterate(Strategy<TProblem, TCandidate> strategy, TProblem p, int n, Func<TCandidate, bool> stop = null, bool useParallel = false)
        {
            if (useParallel && n > 1) 
            {
                var l = new object();
                var stopped = false;
                var best = default(TCandidate);
                var bestExists = false;
                Parallel.For((long) 0, n, (i) =>
                {
                    if (stopped) return;
                    var next = strategy.Search(p);
                    lock (l)
                    {
                        if (!bestExists)
                        {
                            best = next;
                            bestExists = true;
                            if (stop != null && stop(best))
                            {
                                stopped = true;
                            }

                            return;
                        }
                        
                        if (CandidateComparison.Compare(best, next) > 0)
                        {
                            best = next;
                            if (stop != null && stop(best))
                            {
                                stopped = true;
                            }
                        }
                    }
                });

                return best;
            }
            else
            {
                var best = strategy.Search(p);
                if (stop != null && stop(best))
                {
                    return best;
                }
                while (n > 1)
                {
                    var next = strategy.Search(p);
                    if (CandidateComparison.Compare(best, next) > 0)
                    {
                        best = next;

                        if (stop != null && stop(best))
                        {
                            return best;
                        }
                    }
                    n--;
                }
                return best;
            }
        }
    }
}