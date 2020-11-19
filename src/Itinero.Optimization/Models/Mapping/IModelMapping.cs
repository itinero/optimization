using System.Collections.Generic;

namespace Itinero.Optimization.Models.Mapping
{
    /// <summary>
    /// Represents the link between a model and the actual routing network.
    /// </summary>
    public interface IModelMapping
    {
        /// <summary>
        /// Gets the routes between the visits in the given tour.
        /// </summary>
        /// <param name="tourAndVehicle">The tour and the vehicle.</param>
        /// <returns>The routes between all visit pairs.</returns>
        IEnumerable<Result<Route>> BuildRoutesBetweenVisits((int vehicle, IEnumerable<int> tour) tourAndVehicle);

        /// <summary>
        /// Gets the router point for the given mapped visit.
        /// </summary>
        /// <param name="mappedVisit">The mapped visit.</param>
        /// <returns>The router point.</returns>
        public RouterPoint GetVisitSnapping(int mappedVisit);

        /// <summary>
        /// Gets the visit in the unmapped model.
        /// </summary>
        /// <param name="mappedVisit">The mapped visit.</param>
        /// <returns>The unmapped visit.</returns>
        public int GetOriginalVisit(int mappedVisit);

        /// <summary>
        /// Gets the mapped visit index.
        /// </summary>
        /// <param name="originalVisit">The unmapped visit index.</param>
        /// <returns>The mapped index, null when the visit isn't mapped.</returns>
        public int? GetMappedIndex(int originalVisit);

        /// <summary>
        /// Gets the router db.
        /// </summary>
        internal RouterDb RouterDb { get; }

        /// <summary>
        /// Gets errors in the mapping if any.
        /// </summary>
        IEnumerable<(int visit, string message)> Errors { get; }
    }
}