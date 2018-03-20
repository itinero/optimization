using Itinero.Optimization.Algorithms.Random;
using Itinero.Optimization.Algorithms.Solvers;

namespace Itinero.Optimization.Abstract.Solvers.VRP.Operators.Relocate
{
    /// <summary>
    /// Implements a relocate operator, tries to improve the existing tours by re-inserting a visit from one tour into another.
    /// </summary>
    /// <remarks>
    /// This follows stop on first-improvement strategy and this operator will only modify the solution when it improves things. 
    /// 
    /// The algorithm works as follows:
    /// 
    /// - Select 2 random tours.
    /// - Try relocating from tour1 -> tour2:
    ///   - Loop over all visits in tour1.
    ///   - Check if they are cheaper to visit in tour2.
    /// - Try relocating from tour2 -> tour2:
    ///   - (see above)
    /// 
    /// The search stops from the moment any improvement is found.
    /// </remarks>

    public class RelocateOperator<TObjective, TProblem, TSolution> : IInterTourImprovementOperator<float, TProblem, TObjective, TSolution, float>
        where TObjective : IRelocateObjective<TProblem, TSolution>
        where TProblem : IRelocateProblem
        where TSolution : IRelocateSolution
    {
        private readonly bool _tryBothDirections;

        /// <summary>
        /// Creates a new relocate operator.
        /// </summary>
        /// <param name="tryBothDirections">Relocate in both directions when true.</param>
        public RelocateOperator(bool tryBothDirections = true)
        {
            _tryBothDirections = tryBothDirections;
        }

        /// <summary>
        /// Gets the name of this operator.
        /// </summary>
        /// <returns></returns>
        public string Name => "REL";

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
            // check if solution has at least two tours.
            if (solution.Count < 2)
            {
                delta = 0;
                return false;
            }

            // choose two random routes.
            var random = RandomGeneratorExtensions.GetRandom();
            var t1 = random.Generate(solution.Count);
            var t2 = random.Generate(solution.Count - 1);
            if (t2 >= t1)
            {
                t2++;
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
        /// <param name="t2">The second tour.</param>
        /// <returns></returns>
        public bool Apply(TProblem problem, TObjective objective, TSolution solution, int t1, int t2, out float delta)
        {
            var tour1 = solution.Tour(t1);
            
            // try t1 -> t2.
            foreach (var triple in tour1.Triples())
            {
                if (objective.TryMove(problem, solution, t1, t2, triple, out float localDelta))
                { // move succeeded.
                    delta = localDelta;
                    return true;
                }
            }

            if (_tryBothDirections)
            { // try t2 -> t1.
                var tour2 = solution.Tour(t2);
                foreach (var triple in tour2.Triples())
                {
                    if (objective.TryMove(problem, solution, t2, t1, triple, out float localDelta))
                    { // move succeeded.
                        delta = localDelta;
                        return true;
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