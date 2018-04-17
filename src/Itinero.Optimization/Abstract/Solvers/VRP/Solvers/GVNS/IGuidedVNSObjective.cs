using Itinero.Optimization.Abstract.Tours;

namespace Itinero.Optimization.Abstract.Solvers.VRP.Solvers.GVNS
{
    /// <summary>
    /// Abstract representation of an objective.
    /// </summary>
    public interface IGuidedVNSObjective<TProblem, TSolution, TPenalty>
    {
        /// <summary>
        /// Builds a TSP for the given tour.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="tour">The tour.</param>
        /// <returns></returns>
        TSP.TSPSubProblem BuildSubTourTSP(TProblem problem, ITour tour);

        /// <summary>
        /// Returns true if the two tours could benifit from inter-improvement optimizations.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="solution">The solution.</param>
        /// <param name="t1">The first tour.</param>
        /// <param name="t2">The second tour.</param>
        /// <returns></returns>
        bool HaveToTryInter(TProblem problem, TSolution solution, int t1, int t2);

        /// <summary>
        /// Applies a penalty to the given problem.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="pentalty">The penalty.</param>
        /// <param name="solution">The solution.</param>
        /// <param name="t1">The first tour.</param>
        /// <param name="t2">The second tour.</param>
        /// <returns>True if a penalty was applied, false otherwise.</returns>
        bool ApplyPenalty(TProblem problem, TSolution solution, int t1, int t2, TPenalty pentalty);

        /// <summary>
        /// Resets the penalty applied to the given problem.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="pentalty">The penalty.</param>
        void ResetPenalty(TProblem problem, TPenalty pentalty);

        /// <summary>
        /// Updates the solution after the problem has changed.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="solution">The solution.</param>
        void UpdateSolution(TProblem problem, TSolution solution);
    }
}