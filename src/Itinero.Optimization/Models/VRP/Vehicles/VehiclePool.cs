namespace Itinero.Optimization.Models.VRP.Vehicles
{
    /// <summary>
    /// Represents a pool of vehicles.
    /// </summary>
    public class VehiclePool
    {
        /// <summary>
        /// Gets or sets the vehicles.
        /// </summary>
        /// <returns></returns>
        public Vehicle[] Vehicles { get; set; }

        /// <summary>
        /// Gets or sets the reusable flag.
        /// </summary>
        public bool Reusable { get; set; }
    }
}