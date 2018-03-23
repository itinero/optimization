using System.Collections.Generic;
using Itinero.Optimization.Abstract.Tours;

namespace Itinero.Optimization.Abstract.Solvers.VRP.Solvers.SCI
{
    /// <summary>
    /// An abstract representation of an objective.
    /// </summary>
    public interface ISeededCheapestInsertionObjective<TProblem, TSolution>
        where TSolution : ISeededCheapestInsertionSolution
    {
        /// <summary>
        /// Creates a new and empty solution.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <returns>A new empty solution.</returns>
        TSolution NewSolution(TProblem problem);

        /// <summary>
        /// Gets a list of potential visits.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <returns>The list of visits to be visited, except potentially those uniquely used as seeds.</returns>
        IList<int> PotentialVisits(TProblem problem);

        /// <summary>
        /// Creates a new instance of the given problem that has tighter constraints.
        /// </summary>
        /// <param name="problem">The original problem.</param>
        /// <returns>A problem that has a tighter constraints.</returns>
        TProblem ProblemWithSlack(TProblem problem);

        // TODO: check if this is needed, the visit list could be derived from the solution or included there as 'unplaced' list or something like that.

        /// <summary>
        /// Seeds the next tour.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="solution">The solution.</param>
        /// <param name="visits">The visit list.</param>
        /// <returns></returns>
        int SeedNext(TProblem problem, TSolution solution, IList<int> visits);

        /// <summary>
        /// Tries to place any of the visits in the given visit list in the given tour.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="solution">The solution.</param>
        /// <param name="t">The tour.</param>
        /// <param name="visits">The visit list.</param>
        /// <returns></returns>
        bool TryPlaceAny(TProblem problem, TSolution solution, int t, IList<int> visits);

        /// <summary>
        /// /// Tries to place any of the visits in the given visit list.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="solution">The solution.</param>
        /// <param name="visits">The visits to try.</param>
        /// <returns></returns>
        bool TryPlaceAny(TProblem problem, TSolution solution, IList<int> visits);

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
        /// Calculates the fitness of the given solution.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="solution">The solution.</param>
        /// <returns></returns>
        float Calculate(TProblem problem, TSolution solution);
    }
}