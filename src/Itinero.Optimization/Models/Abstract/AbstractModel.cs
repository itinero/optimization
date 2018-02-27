using System;
using Itinero.Optimization.Models.Abstract.Costs;
using Itinero.Optimization.Models.Costs;
using Itinero.Optimization.Models.TimeWindows;
using Itinero.Optimization.Models.Vehicles;

namespace Itinero.Optimization.Models.Abstract
{
    /// <summary>
    /// Represents a model of a problem to solve.
    /// </summary>
    public class AbstractModel
    {
        /// <summary>
        /// Gets or sets the costs of travel between visits.
        /// </summary>
        /// <returns></returns>
        public TravelCostMatrix[] TravelCosts { get; set; }

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

        /// <summary>
        /// Returns a travel cost for the given metric or throws an exception of not found.
        /// </summary>
        /// <param name="metric">The metric to get costs for.</param>
        public TravelCostMatrix GetForMetric(string metric)
        {
            for (var i = 0; i < this.TravelCosts.Length; i++)
            {
                if (this.TravelCosts[i].Name == metric)
                {
                    return this.TravelCosts[i];
                }
            }
            throw new ArgumentOutOfRangeException(nameof(metric), metric, 
                string.Format("No travel cost found for the given metric."));
        }
    }

    /// <summary>
    /// Contains extension methods related to models.
    /// </summary>
    public static class ModelExtensions
    {
        /// <summary>
        /// Returns true if this model is valid.
        /// </summary>
        public static bool IsValid(this AbstractModel model)
        {
            string reasonIfNot;
            return model.IsValid(out reasonIfNot);
        }

        /// <summary>
        /// Returns true if this model is valid.
        /// </summary>
        public static bool IsValid(this AbstractModel model, out string reasonIfNot)
        {
            if (model.Vehicles == null)
            {
                reasonIfNot = "No vehicles defined.";
                return false;
            }
            if (model.TravelCosts == null ||
                model.TravelCosts.Length == 0)
            {
                reasonIfNot = "No travel costs defined.";
                return false;
            }
            reasonIfNot = string.Empty;
            return true;
        }
    }
}