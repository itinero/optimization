using System.Collections.Generic;
using Itinero.Optimization.Abstract.Tours;

namespace Itinero.Optimization.Abstract.Solvers.VRP.Operators.Relocate.Multi
{
    /// <summary>
    /// An abstract definition of an objective.
    /// </summary>
    public interface IMultiRelocateObjective<TProblem, TSolution>
    {
        /// <summary>
        /// Enumerates all sequence of the given sizes.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="tour">The tour.</param>
        /// <param name="minSize">The minimum size.</param>
        /// <param name="maxSize">The maximum size.</param>
        /// <param name="wrap">If true, sequences where the beginning node is in the middle will be included too</param>
        /// <returns>An enumerable with sequences.</returns>
        IEnumerable<Seq> SeqAndSmaller(TProblem problem, IEnumerable<int> tour, int minSize, int maxSize, bool wrap);

        /// <summary>
        /// Tries to move the given sequence from t1 in between the given pair in t2.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="solution">The solution.</param>
        /// <param name="t1">The first tour.</param>
        /// <param name="t2">The second tour.</param>
        /// <param name="seq">The sequence.</param>
        /// <param name="pair">The pair.</param>
        /// <param name="delta">The difference in visit.</param>
        /// <returns></returns>
        bool TryMove(TProblem problem, TSolution solution, int t1, int t2, Seq seq, Pair pair, out float delta);
    }
}