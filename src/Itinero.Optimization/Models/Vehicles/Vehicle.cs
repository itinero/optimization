using Itinero.Optimization.Models.Vehicles.Constraints;

namespace Itinero.Optimization.Models.Vehicles
{
    /// <summary>
    /// Represents a vehicle.
    /// </summary>
    public class Vehicle
    {
        /// <summary>
        /// Gets or sets the metric that has to be used for the travel times related to this vehicle.
        /// </summary>
        public string Metric { get; set; }

        /// <summary>
        /// Gets or sets the capacity constraints.
        /// </summary>
        public CapacityConstraint[] CapacityConstraints { get; set; }

        /// <summary>
        /// Gets or sets the departure location if fixed.
        /// </summary>
        /// <returns></returns>
        public int? Departure { get; set; }

        /// <summary>
        /// Gets or sets the arrival location if fixed.
        /// </summary>
        /// <returns></returns>
        public int? Arrival { get; set; }
    }
}