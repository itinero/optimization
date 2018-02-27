namespace Itinero.Optimization.Models.Vehicles.Constraints
{
    /// <summary>
    /// Represents a capacity constraint.
    /// </summary>
    public class CapacityConstraint
    {
        /// <summary>
        /// Gets or sets the name of the metric used in this constraint.
        /// </summary>
        /// <returns></returns>
        public string Name { get; set; }

        /// <summary>
        /// Gets or set the maximum capacity.
        /// </summary>
        /// <returns></returns>
        public float Capacity { get; set; }
    }
}