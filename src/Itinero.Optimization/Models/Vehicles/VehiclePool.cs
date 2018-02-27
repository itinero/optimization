namespace Itinero.Optimization.Models.Vehicles
{
    /// <summary>
    /// Represents a vehicle pool.
    /// </summary>
    public class VehiclePool
    {
        /// <summary>
        /// Gets or sets the vehicles in the pool.
        /// </summary>
        /// <returns></returns>
        public Vehicle[] Vehicles { get; set; }

        /// <summary>
        /// Gets or sets the reusable flag, when false there is no limit on vehicle reuse.
        /// </summary>
        /// <returns></returns>
        public bool Reusable { get; set; }
    }
}