using Itinero.Algorithms.Matrices;
using Itinero.Optimization.Models.Mapping;
using Itinero.Profiles;

namespace Itinero.Optimization.Models
{
    /// <summary>
    /// Extension methods related to model and abstract model to make conversion easier.
    /// </summary>
    public static class ModelAbstractExtensions
    {
        /// <summary>
        /// Converts a capacity constraint to it's abstract equivalent.
        /// </summary>
        /// <param name="constraint">The constraint to convert.</param>
        /// <returns></returns>
        public static Abstract.Models.Vehicles.Constraints.CapacityConstraint ToAbstract(this Models.Vehicles.Constraints.CapacityConstraint constraint)
        {
            return new Abstract.Models.Vehicles.Constraints.CapacityConstraint()
            {
                Name = constraint.Name,
                Capacity = constraint.Capacity
            };
        }

        /// <summary>
        /// Converts an array of capacity constraints to it's abstract equivalents.
        /// </summary>
        /// <param name="constraints">The constraints to convert.</param>
        /// <returns></returns>
        public static Abstract.Models.Vehicles.Constraints.CapacityConstraint[] ToAbstract(this Models.Vehicles.Constraints.CapacityConstraint[] constraints)
        {
            if (constraints == null)
            {
                return null;
            }

            var abstractConstraints = new Abstract.Models.Vehicles.Constraints.CapacityConstraint[constraints.Length];
            for (var c = 0; c < constraints.Length; c++)
            {
                abstractConstraints[c] = constraints[c].ToAbstract();
            }
            return abstractConstraints;
        }

        /// <summary>
        /// Converts the given vehicle to it's abstract equivalent.
        /// </summary>
        /// <param name="vehicle">The vehicle.</param>
        /// <param name="weightMatrix">The weight matrix.</param>
        /// <returns></returns>
        public static Abstract.Models.Vehicles.Vehicle ToAbstract<T>(this Models.Vehicles.Vehicle vehicle, IWeightMatrixAlgorithm<T> weightMatrix)
        {
            return new Abstract.Models.Vehicles.Vehicle()
            {
                Metric = weightMatrix.Profile.Profile.Metric.ToModelMetric(),
                Arrival = weightMatrix.WeightIndexNullable(vehicle.Arrival),
                Departure = weightMatrix.WeightIndexNullable(vehicle.Departure),
                CapacityConstraints = vehicle.CapacityConstraints.ToAbstract()
            };
        }

        /// <summary>
        /// Converts an array of vehicles it's abstract equivalents.
        /// </summary>
        /// <param name="vehicles">The vehicles to convert.</param>
        /// <param name="weightMatrix">The weight matrix.</param>
        /// <returns></returns>
        public static Abstract.Models.Vehicles.Vehicle[] ToAbstract<T>(this Models.Vehicles.Vehicle[] vehicles, IWeightMatrixAlgorithm<T> weightMatrix)
        {
            if (vehicles == null)
            {
                return null;
            }

            var abstractVehicles = new Abstract.Models.Vehicles.Vehicle[vehicles.Length];
            for (var c = 0; c < vehicles.Length; c++)
            {
                abstractVehicles[c] = vehicles[c].ToAbstract(weightMatrix);
            }
            return abstractVehicles;
        }

        /// <summary>
        /// Converts the given vehicle pool to it's abstract equivalent.
        /// </summary>
        /// <param name="vehiclePool">The vehicle pool.</param>
        /// <param name="weightMatrix">The weight matrix.</param>
        /// <returns></returns>
        public static Abstract.Models.Vehicles.VehiclePool ToAbstract<T>(this Models.Vehicles.VehiclePool vehiclePool, IWeightMatrixAlgorithm<T> weightMatrix)
        {
            return new Abstract.Models.Vehicles.VehiclePool()
            {
                Vehicles = vehiclePool.Vehicles.ToAbstract(weightMatrix),
                Reusable = vehiclePool.Reusable
            };
        }

        /// <summary>
        /// Converts the given time window to it's abstract equivalent.
        /// </summary>
        /// <param name="timeWindow">The time window.</param>
        /// <returns></returns>
        public static Abstract.Models.TimeWindows.TimeWindow ToAbstract(this Models.TimeWindows.TimeWindow timeWindow)
        {
            return new Abstract.Models.TimeWindows.TimeWindow()
            {
                Min = timeWindow.Min,
                Max = timeWindow.Max
            };
        }

        /// <summary>
        /// Converts the array of time windows to it's abstract equivalents and removes invalid locations.
        /// </summary>
        /// <param name="timeWindows">The time windows to convert.</param>
        /// <param name="weightMatrix">The weight matrix.</param>
        /// <returns></returns>
        public static Abstract.Models.TimeWindows.TimeWindow[] ToAbstract<T>(this Models.TimeWindows.TimeWindow[] timeWindows, IWeightMatrixAlgorithm<T> weightMatrix)
        {
            if (timeWindows == null)
            {
                return null;
            }

            var abstractTimeWindows = new Abstract.Models.TimeWindows.TimeWindow[timeWindows.Length];
            for (var c = 0; c < timeWindows.Length; c++)
            {
                abstractTimeWindows[c] = timeWindows[c].ToAbstract();
            }

            abstractTimeWindows = weightMatrix.AdjustToMatrix(abstractTimeWindows);

            return abstractTimeWindows;
        }

        /// <summary>
        /// Converts the given visit cost to it's abstract equivalent.
        /// </summary>
        /// <param name="visitCost">The visit cost.</param>
        /// <param name="weightMatrix">The weight matrix.</param>
        /// <returns></returns>
        public static Abstract.Models.Costs.VisitCosts ToAbstract<T>(this Models.Costs.VisitCosts visitCost, IWeightMatrixAlgorithm<T> weightMatrix)
        {
            return new Abstract.Models.Costs.VisitCosts()
            {
                Name = visitCost.Name,
                Costs = weightMatrix.AdjustToMatrix(visitCost.Costs)
            };
        }

        /// <summary>
        /// Converts the array of vist costs to it's abstract equivalents.
        /// </summary>
        /// <param name="visitCosts">The visit costs to convert.</param>
        /// <param name="weightMatrix">The weight matrix.</param>
        /// <returns></returns>
        public static Abstract.Models.Costs.VisitCosts[] ToAbstract<T>(this Models.Costs.VisitCosts[] visitCosts, IWeightMatrixAlgorithm<T> weightMatrix)
        {
            if (visitCosts == null)
            {
                return null;
            }

            var abstractVisitCosts = new Abstract.Models.Costs.VisitCosts[visitCosts.Length];
            for (var c = 0; c < visitCosts.Length; c++)
            {
                abstractVisitCosts[c] = visitCosts[c].ToAbstract(weightMatrix);
            }

            return abstractVisitCosts;
        }

        /// <summary>
        /// Converts profile metrics into the equivalent model type.
        /// </summary>
        public static string ToModelMetric(this ProfileMetric profileMetric)
        {
            switch (profileMetric)
            {
                case ProfileMetric.DistanceInMeters:
                    return Abstract.Models.Metrics.Distance;
                case ProfileMetric.TimeInSeconds:
                    return Abstract.Models.Metrics.Time;
            }
            return "custom";
        }
    }
}