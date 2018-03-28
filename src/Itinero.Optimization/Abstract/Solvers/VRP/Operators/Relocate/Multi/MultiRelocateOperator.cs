using Itinero.Optimization.Algorithms.Random;

namespace Itinero.Optimization.Abstract.Solvers.VRP.Operators.Relocate.Multi
{
    /// <summary>
    /// Implements a relocate operator, tries to improve the existing tours by re-inserting a sequence of visits from one tour into another.
    /// </summary>
    /// <remarks>
    /// This follows stop on first-improvement strategy and this operator will only modify the solution when it improves things. 
    /// 
    /// The algorithm works as follows:
    /// 
    /// - Select 2 random tours.
    /// - Try relocating from tour1 -> tour2:
    ///   - Loop over sequences of visits in tour1.
    ///   - Check if they are cheaper to visit in tour2.
    /// - Try relocating from tour2 -> tour2:
    ///   - (see above)
    /// 
    /// The search stops from the moment any improvement is found.
    /// </remarks>
    public class MultiRelocateOperator<TObjective, TProblem, TSolution> : IInterTourImprovementOperator<float, TProblem, TObjective, TSolution, float>
        where TObjective : IMultiRelocateObjective<TProblem, TSolution>
        where TSolution : IMultiRelocateSolution
    {
        private readonly int _minWindowSize = 2;
        private readonly int _maxWindowSize = 10;
        private readonly bool _tryReverse = true;

        /// <summary>
        /// Creates a new operator.
        /// </summary>
        /// <param name="minWindowSize">The minimum window size to search for sequences to relocate.</param>
        /// <param name="maxWindowSize">The maximum window size to search for sequences to relocate.</param>
        /// <param name="tryReverse">When true the reverse sequence will also be attempted.</param>
        public MultiRelocateOperator(int minWindowSize = 2, int maxWindowSize = 5, bool tryReverse = true)
        {
            _maxWindowSize = minWindowSize;
            _maxWindowSize = maxWindowSize;
        }

        /// <summary>
        /// Gets the name of this operator.
        /// </summary>
        public string Name
        {
            get
            {
                var name = string.Format("REL-MUL-{0}-{1}", _minWindowSize, _maxWindowSize);
                return name;
            }
        }

        /// <summary>
        /// Returns true if it doesn't matter if tour indexes are switched.
        /// </summary>
        public bool IsSymmetric => false;

        /// <summary>
        /// Applies this operator.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="objective">The objective.</param>
        /// <param name="solution">The solution.</param>
        /// <param name="delta">The difference in fitness.</param>
        /// <returns></returns>
        public bool Apply(TProblem problem, TObjective objective, TSolution solution, out float delta)
        {
            int t1, t2;
            if (!RandomGeneratorExtensions.randomRoutes(solution.Count, out t1, out t2))
            {
                delta = 0;
                return false;
            }

            return Apply(problem, objective, solution, t1, t2, out delta);
        }

        /// <summary>
        /// Applies this operator.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="objective">The objective.</param>
        /// <param name="solution">The solution.</param>
        /// <param name="delta">The difference in fitness.</param>
        /// <param name="t1">The first tour.</param>
        /// /// <param name="t2">The second tour.</param>
        /// <returns></returns>        
        public bool Apply(TProblem problem, TObjective objective, TSolution solution, int t1, int t2, out float delta)
        {
            var tour1 = solution.Tour(t1);
            var tour2 = solution.Tour(t2);

            // wrap around is false; thus depots can not be switched
            var tour1Enumerable = objective.SeqAndSmaller(problem, tour1, _minWindowSize + 2, _maxWindowSize + 2, false);
            foreach (var s in tour1Enumerable)
            {
                foreach (var pair in tour2.Pairs())
                {
                    if (objective.TryMove(problem, solution, t1, t2, s, pair, out float localDelta))
                    { // move succeeded.
                        delta = localDelta;
                        return true;
                    }
                }
            }
            if (_tryReverse)
            { // also try t2 -> t1
                var tour2Enumerable = objective.SeqAndSmaller(problem, tour2, _minWindowSize + 2, _maxWindowSize + 2, false);
                foreach (var s in tour2Enumerable)
                {
                    foreach (var pair in tour1.Pairs())
                    {
                        if (objective.TryMove(problem, solution, t2, t1, s, pair, out float localDelta))
                        { // move succeeded.
                            delta = localDelta;
                            return true;
                        }
                    }
                }
            }
            delta = 0;
            return false;
        }

        /// <summary>
        /// Returns true if the given objective is supported.
        /// </summary>
        /// <param name="objective">The objective.</param>
        /// <returns></returns>
        public bool Supports(TObjective objective)
        {
            return true;
        }
    }
}