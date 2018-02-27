using Itinero.Optimization.Models.Costs;
using Itinero.Optimization.Models.TimeWindows;
using Itinero.Optimization.Models.Vehicles;

namespace Itinero.Optimization.Models
{
    /// <summary>
    /// Represents a model for a vehicle routing problem.
    /// </summary>
    public class Model
    {
        /// <summary>
        /// Gets or sets the visits (including any depots).
        /// </summary>
        /// <returns></returns>
        public Visit[] Visits { get; set; }

        /// <summary>
        /// Gets or sets the time windows.
        /// </summary>
        /// <returns></returns>
        public TimeWindow[] TimeWindows { get; set; }
    
        /// <summary>
        /// Gets or sets the visit costs.
        /// </summary>
        /// <returns></returns>
        public VisitCosts[] VisitCosts { get; set; }

        /// <summary>
        /// Gets or sets the vehicle pool.
        /// </summary>
        /// <returns></returns>
        public VehiclePool Vehicles { get; set; }
    }
}