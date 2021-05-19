namespace Itinero.Optimization.Solvers.Tours
{
    /// <summary>
    /// Enumerates the types of tours.
    /// </summary>
    public enum TourTypeEnum
    {
        /// <summary>
        /// The first and last visit are fixed, the visits in between can be adjusted.
        /// </summary>
        Fixed,
        /// <summary>
        /// The first visit is fixed and the last visits returns again to the first.
        /// </summary>
        Closed,
        /// <summary>
        /// The first visit is fixed but the last visit doesn't return to the first.
        /// </summary>
        Open
    }
}