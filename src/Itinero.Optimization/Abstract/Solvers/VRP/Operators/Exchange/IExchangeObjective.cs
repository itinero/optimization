using Itinero.Optimization.Abstract.Tours;

namespace Itinero.Optimization.Abstract.Solvers.VRP.Operators.Exchange
{
    /// <summary>
    /// An abstract definition of an objective.
    /// </summary>
    public interface IExchangeObjective<TProblem>
        where TProblem : IExchangeProblem
    {
        /// <summary>
        /// Tries to swap the given visits between the two given tours.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="t1">The first tour.</param>
        /// <param name="t2">The second tour.</param>
        /// <param name="visit1">The visit from tour1.</param>
        /// <param name="visit2">The visit from tour2.</param>
        /// <param name="delta">The difference in visit.</param>
        /// <returns></returns>
        bool TrySwap(TProblem problem, int t1, int t2, Triple visit1, Triple visit2, out float delta);
    }
}