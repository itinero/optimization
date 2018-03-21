using Itinero.Optimization.Algorithms.Random;

namespace Itinero.Optimization.Abstract.Solvers.VRP.Operators.Exchange
{
    /// <summary>
    /// An exchange inter improvement operator.
    /// </summary>
    /// <remarks>
    /// This follows a 'stop on first'-improvement strategy and this operator will only modify the solution when it improves things. 
    /// 
    /// The algorithm works as follows:
    /// 
    /// - Select 2 random tours.
    /// - Loop over all visits in tour1.
    ///   - Loop over all visits in tour2.
    ///     - Check if a swap between tours improves things.
    /// 
    /// The search stops from the moment any improvement is found.
    /// </remarks>
    public class ExchangeOperator<TObjective, TProblem, TSolution> : IInterTourImprovementOperator<float, TProblem, TObjective, TSolution, float>
        where TObjective : IExchangeObjective<TProblem, TSolution>
        where TProblem : IExchangeProblem
        where TSolution : IExchangeSolution
    {
        /// <summary>
        /// Gets the name of this operator.
        /// </summary>
        public string Name => "EX";

        /// <summary>
        /// Returns true if it doesn't matter if tour indexes are switched.
        /// </summary>
        public bool IsSymmetric => true;

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
            // check if solution has at least two tours.
            if (solution.Count < 2)
            {
                delta = 0;
                return false;
            }

            // choose two random routes.
            var random = RandomGeneratorExtensions.GetRandom();
            var tourIdx1 = random.Generate(solution.Count);
            var tourIdx2 = random.Generate(solution.Count - 1);
            if (tourIdx2 >= tourIdx1)
            {
                tourIdx2++;
            }

            return Apply(problem, objective, solution, tourIdx1, tourIdx2, out delta);
        }

        /// <summary>
        /// Applies this operator.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="objective">The objective.</param>
        /// <param name="solution">The solution.</param>
        /// <param name="delta">The difference in fitness.</param>
        /// <param name="t1">The first tour.</param>
        /// <param name="t2">The second tour.</param>
        /// <returns></returns>
        public bool Apply(TProblem problem, TObjective objective, TSolution solution, int t1, int t2, out float delta)
        { 
            var tour1 = solution.Tour(t1);
            var tour2 = solution.Tour(t2);

            foreach (var triple1 in tour1.Triples())
            {
                foreach (var triple2 in tour2.Triples())
                {
                    if (objective.TrySwap(problem, solution, t1, t2, triple1, triple2, out float localDelta))
                    { // swap succeeded.
                        delta = localDelta;
                        return true;
                    }
                }
            }

            delta = 0;
            return false;
        }

        /// <summary>
        /// Returns true if the given objective is supported by this operator.
        /// </summary>
        /// <param name="objective"></param>
        /// <returns></returns>
        public bool Supports(TObjective objective)
        {
            return true;
        }
    }
}