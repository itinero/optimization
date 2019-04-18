using System.Collections.Generic;

namespace Itinero.Optimization.Solvers.Tours
{
    /// <summary>
    /// Contains extension methods on top if the read only tour interface.
    /// </summary>
    public static class IReadOnlyTourExtensions
    {
        /// <summary>
        /// Returns true if the given route is closed.
        /// </summary>
        public static bool IsClosed(this IReadOnlyTour tour)
        {
            return tour.Last.HasValue &&
                   tour.Last.Value == tour.First;
        }
        
        /// <summary>
        /// Gets an enumerable enumerating pairs in the tour enumerable.
        /// </summary>
        /// <param name="tourEnumerable">A tour enumerable.</param>
        /// <returns>An enumerable enumerating pairs.</returns>
        public static IEnumerable<Pair> Pairs(this IReadOnlyTour tourEnumerable)
        {
            return new PairEnumerable<IReadOnlyTour>(tourEnumerable, tourEnumerable.IsClosed());
        }
    }
}