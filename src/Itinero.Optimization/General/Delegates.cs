
namespace Itinero.Optimization.General
{
    /// <summary>
    /// Contains a collection of generally reusable delegates.
    /// </summary>
    public static class Delegates
    {
        /// <summary>
        /// Returns true if the two given tours overlap.
        /// </summary>
        /// <param name="problem">The problem these tours are for.</param>
        /// <param name="tour1">The first tour.</param>
        /// <param name="tour2">The second tour.</param>
        /// <returns>True if the two tours geographically overlap.</returns>        
        public delegate bool OverlapsFunc<TProblem, TTour>(TProblem problem, TTour tour1, TTour tour2);
    }
}