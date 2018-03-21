using System.Collections.Generic;
using Itinero.Optimization.Abstract.Tours;

namespace Itinero.Optimization.Abstract.Solvers.VRP.Operators.Exchange.Multi
{
    /// <summary>
    /// An abstract definition of an objective.
    /// </summary>
    public interface IMultiExchangeObjective<TProblem, TSolution>
    {
        /// <summary>
        /// Enumerates all sequence of the given sizes.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="tour">The tour.</param>
        /// <param name="minSize">The minimum size.</param>
        /// <param name="maxSize">The maximum size.</param>
        /// <returns>An enumerable with sequences.</returns>
        IEnumerable<Seq> SeqAndSmaller(TProblem problem, IEnumerable<int> tour, int minSize, int maxSize);

        /// <summary>
        /// Reverses the given sequence.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="sequence">The sequence.</param>
        /// <returns>The reversed sequence.</returns>
        Seq Reverse(TProblem problem, Seq sequence);

        /// <summary>
        /// Tries to swap the given sequences between the two given tours.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="solution">The solution.</param>
        /// <param name="t1">The first tour.</param>
        /// <param name="t2">The second tour.</param>
        /// <param name="s1">The sequence from tour1.</param>
        /// <param name="s2">The sequence from tour2.</param>
        /// <param name="delta">The difference in visit.</param>
        /// <returns>True if the swap succeeded.</returns>        
        bool TrySwap(TProblem problem, TSolution solution, int t1, int t2, Seq s1, Seq s2, out float delta);
    }
}