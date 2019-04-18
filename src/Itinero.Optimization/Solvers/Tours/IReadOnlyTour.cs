using System.Collections.Generic;

namespace Itinero.Optimization.Solvers.Tours
{
    /// <summary>
    /// Abstract representation of a tour enumerable.
    /// </summary>
    public interface IReadOnlyTour : IEnumerable<int>
    {
        /// <summary>
        /// Gets the first visit.
        /// </summary>
        int First { get; }
        
        /// <summary>
        /// Gets the last visit (if fixed).
        /// </summary>
        int? Last { get; }
        
        /// <summary>
        /// Gets the number of visits.
        /// </summary>
        int Count { get; }
    }
}