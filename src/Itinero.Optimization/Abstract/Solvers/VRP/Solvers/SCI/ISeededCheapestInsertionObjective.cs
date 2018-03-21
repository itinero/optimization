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
        /// <returns></returns>
        TSolution NewSolution();

        /// <summary>
        /// Gets a list of potential visits.
        /// </summary>
        /// <returns></returns>
        IList<int> PotentialVisits();

        // TODO: check if this is needed, the visit list could be derived from the solution or included there as 'unplaced' list or something like that.

        /// <summary>
        /// Seeds the next tour.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="solution">The solution.</param>
        /// <param name="visits">The visit list.</param>
        /// <returns></returns>
        ITour SeedNext(TProblem problem, TSolution solution, IList<int> visits);

        /// <summary>
        /// Tries to place any of the visits in the given visit list in the given tour.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="tour">The problem.</param>
        /// <param name="visits">The visit list.</param>
        /// <returns></returns>
        bool TryPlaceAny(TProblem problem, ITour tour, IList<int> visits);

        /// <summary>
        /// /// Tries to place any of the visits in the given visit list.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="solution">The tour.</param>
        /// <param name="visits">The visits to try.</param>
        /// <returns></returns>
        bool TryPlaceAny(TProblem problem, TSolution solution, IList<int> visits);

        /// <summary>
        /// Builds a TSP for the given tour.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="tour">The tour.</param>
        /// <returns></returns>
        TSP.ITSProblem BuildSubTourTSP(TProblem problem, ITour tour);

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