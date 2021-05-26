using System;
using System.Collections.Generic;
using Itinero.Optimization.Solvers.Tours;

namespace Itinero.Optimization.Solvers.Shared.LocalSearch.Local2Opt
{
    internal static class Local2OptOperationDirected
    {
        /// <summary>
        /// Gets all possible unique pairs of edges. Each pair of edges occurs only ones, if (edge1, edge2) occurs, (edge2, edge1) won't.
        /// </summary>
        /// <param name="tour">The tour.</param>
        /// <returns>An enumerable of edge pairs.</returns>
        public static IEnumerable<(Pair edge1, Pair edge2)> GetDisjointEdgePairs(this Tour tour)
        {
            foreach (var edge1 in tour.Pairs())
            foreach (var edge2 in tour.Pairs(edge1.To))
            {
                if (edge1.To == edge2.From) continue;
                if (edge1.To == edge2.To) continue;
                if (edge1.From == edge2.To) continue;
                if (edge1.From == edge2.From) continue;
                
                yield return (edge1, edge2);
            }
        }

        /// <summary>
        /// Enumerates all 2-Opt moves that lead to an improvement.
        /// </summary>
        /// <param name="tour">The tour.</param>
        /// <param name="weightFunc">The weight function.</param>
        /// <returns>A 2-Opt move, if any.</returns>
        public static IEnumerable<(Pair edge1, Pair edge2, Pair newEdge1, Pair newEdge2, float weightDecrease)> Get2OptImprovements(this Tour tour,
            Func<int, int, float> weightFunc)
        {
            foreach (var (edge1, edge2) in tour.GetDisjointEdgePairs())
            {
                var currentWeight = weightFunc(edge1.From, edge1.To) +
                                    weightFunc(edge2.From, edge2.To);
                
                // consider edge1 -> edge2.
                var edge1To2 = weightFunc(edge1.From, edge2.From) +
                               weightFunc(edge1.To, edge2.To);
                if (edge1To2 < currentWeight)
                {
                    yield return (edge1: edge1, edge2: edge2,
                        new Pair(edge1.From, edge2.From),
                        new Pair(edge1.To, edge2.To), currentWeight - edge1To2);
                }

                // consider edge2 -> edge1.
                var edge2To1 = weightFunc(edge2.From, edge1.From) +
                               weightFunc(edge2.To, edge1.To);
                if (edge2To1 < currentWeight)
                {
                    yield return (edge1: edge1, edge2: edge2,
                        new Pair(edge2.From, edge1.From),
                        new Pair(edge2.To, edge1.To), currentWeight - edge2To1);
                }
            }
        }
    }
}