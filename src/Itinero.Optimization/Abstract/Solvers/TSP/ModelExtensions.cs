using Itinero.Optimization.Abstract.Models;

namespace Itinero.Optimization.Solutions.TSP
{
    /// <summary>
    /// TSP related extensions to the models.
    /// </summary>
    public static class ModelExtensions
    {
        /// <summary>
        /// Returns true if the given model is a TSP.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static bool IsTSP(this AbstractModel model)
        {
            string reasonIfNot;
            return model.IsTSP(out reasonIfNot);
        }

        /// <summary>
        /// Returns true if the given model is a TSP.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="reasonIfNot">The reason if it's not considered a TSP.</param>
        /// <returns></returns>
        public static bool IsTSP(this AbstractModel model, out string reasonIfNot)
        {
            if (!model.IsValid(out reasonIfNot))
            {
                reasonIfNot = "Model is invalid: " + reasonIfNot;
                return false;
            }

            if (model.VehiclePool.Reusable ||
                model.VehiclePool.Vehicles.Length > 1)
            {
                reasonIfNot = "More than one vehicle or vehicle reusable.";
                return false;
            }
            var vehicle = model.VehiclePool.Vehicles[0];
            if (vehicle.CapacityConstraints != null &&
                vehicle.CapacityConstraints.Length > 0)
            {
                reasonIfNot = "At least one capacity constraint was found.";
                return false;
            }
            if (model.TimeWindows != null &&
                model.TimeWindows.Length > 0)
            {
                // TODO: check if timewindows are there but are all set to max.
                reasonIfNot = "Timewindows detected.";
                return false;
            }
            return true;
        }

        /// <summary>
        /// Converts the given abstract model to a TSP.
        /// </summary>
        public static ITSProblem ToTSP(this AbstractModel model, out string reasonWhenFailed)
        {
            if (!model.IsTSP(out reasonWhenFailed))
            {
                reasonWhenFailed = "Model is not a TSP: " + reasonWhenFailed;
                return null;
            }

            var vehicle = model.VehiclePool.Vehicles[0];
            var metric = vehicle.Metric;
            var weights = model.GetForMetric(metric);
            int first = 0;
            
            var problem = new TSProblem(first, weights.Costs);
            if (vehicle.Departure.HasValue)
            {
                problem.First = vehicle.Departure.Value;
            }
            if (vehicle.Arrival.HasValue)
            {
                problem.Last = vehicle.Arrival.Value;
            }
            return problem;
        }
    }
}