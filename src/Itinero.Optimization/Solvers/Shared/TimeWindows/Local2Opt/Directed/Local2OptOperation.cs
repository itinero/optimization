using System;
using Itinero.Logging;
using Itinero.Optimization.Solvers.Shared.Directed;
using Itinero.Optimization.Solvers.Shared.LocalSearch.Local2Opt;
using Itinero.Optimization.Solvers.Tours;

namespace Itinero.Optimization.Solvers.Shared.TimeWindows.Local2Opt.Directed
{
    internal static class Local2OptOperation
    {
        /// <summary>
        /// Runs a local 2-opt local search in a first improvement strategy.
        /// </summary>
        /// <param name="tour">The tour.</param>
        /// <param name="weightFunc">The function to get weights.</param>
        /// <param name="turnPenaltyFunc">The function to get turn penalties.</param>
        /// <param name="windows">The time windows.</param>
        /// <returns>True if the operation succeeded and an improvement was found.</returns>
        /// <remarks>* 2-opt: Removes two edges and reconnects the two resulting paths in a different way to obtain a new tour.</remarks>
        public static bool Do2OptDirected(this Tour tour, Func<int, int, float> weightFunc, Func<TurnEnum, float> turnPenaltyFunc, TimeWindow[] windows)
        {
            float QuadWeight(Quad q)
            {
                return SequenceWeight(q.Visit1, q.Visit2, q.Visit3, q.Visit4);
            }

            float SequenceWeight(int v1, int v2, int v3, int v4)
            {
                return weightFunc(DirectedHelper.WeightIdDeparture(v1),
                    DirectedHelper.WeightIdArrival(v2)) +
                       weightFunc(DirectedHelper.WeightIdDeparture(v2),
                    DirectedHelper.WeightIdArrival(v3)) + 
                       weightFunc(DirectedHelper.WeightIdDeparture(v3),
                    DirectedHelper.WeightIdArrival(v4)) + 
                       turnPenaltyFunc(DirectedHelper.ExtractTurn(v2)) + 
                       turnPenaltyFunc(DirectedHelper.ExtractTurn(v3));
            }

            using var enumerator = tour.Get2OptImprovementsDirected(QuadWeight).GetEnumerator();
            if (!enumerator.MoveNext()) return false; // no improvements could be found.

            var move = enumerator.Current.move;
            
            tour.Apply2OptDirected(move);
            return true;
        }
    }
}