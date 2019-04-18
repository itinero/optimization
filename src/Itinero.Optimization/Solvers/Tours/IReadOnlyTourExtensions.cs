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
    }
}