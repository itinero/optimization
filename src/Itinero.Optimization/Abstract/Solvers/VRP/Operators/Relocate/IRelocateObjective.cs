using Itinero.Optimization.Abstract.Tours;

namespace Itinero.Optimization.Abstract.Solvers.VRP.Operators.Relocate
{
    /// <summary>
    /// An abstract definition of an objective.
    /// </summary>
    public interface IRelocateObjective<TProblem, TSolution>
        where TProblem : IRelocateProblem
        where TSolution : IRelocateSolution
    {
        /// <summary>
        /// Tries to move the given visit (the middle of the triple) from t1 -> t2.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="solution">The solution.</param>
        /// <param name="t1">The first tour.</param>
        /// <param name="t2">The second tour.</param>
        /// <param name="visit">The visit.</param>
        /// <param name="delta">The difference in visit.</param>
        /// <returns></returns>
        bool TryMove(TProblem problem, TSolution solution, int t1, int t2, Triple visit, out float delta);
    }
}