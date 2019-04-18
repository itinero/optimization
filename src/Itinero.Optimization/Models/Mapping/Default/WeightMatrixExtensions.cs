using Itinero.Algorithms.Matrices;
using Itinero.Optimization.Models.Vehicles;
using Itinero.Optimization.Models.Visits;

namespace Itinero.Optimization.Models.Mapping.Default
{
    /// <summary>
    /// Contains extension methods for weight matrix algorithms used in model mapping.
    /// </summary>
    public static class WeightMatrixExtensions
    {
        /// <summary>
        /// Adjusts the given visit array to the weight matrix by removing invalid entries.
        /// </summary>
        /// <param name="algorithm">The weight matrix algorithm.</param>
        /// <param name="original">The original visits.</param>
        /// <returns>The adjusted visits.</returns>
        public static Visit[] AdjustToMapping(this IWeightMatrixAlgorithm<float> algorithm, Visit[] original)
        {
            if (algorithm.Weights.Length == original.Length)
            { // don't copy if no errors.
                return original;
            }
            
            var newAr = new Visit[algorithm.Weights.Length];
            for (var i = 0; i < newAr.Length; i++)
            {
                newAr[i] = original[algorithm.OriginalLocationIndex(i)];
            }
            return newAr;
        }
        
        internal static bool TryToMap(this IWeightMatrixAlgorithm<float> algorithm, VehiclePool vehiclePool, out VehiclePool mappedVehiclePool,
            out string message)
        {
            if (algorithm.Errors.Count == 0 &&
                algorithm.MassResolver.Errors.Count == 0)
            { // don't copy if no errors.
                mappedVehiclePool = vehiclePool;
                message = string.Empty;
                return true;
            }

            mappedVehiclePool = new VehiclePool()
            {
                Reusable = vehiclePool.Reusable,
                Vehicles = vehiclePool.Vehicles
            };
            for (var v = 0; v < mappedVehiclePool.Vehicles.Length; v++)
            {
                var vehicle = mappedVehiclePool.Vehicles[v];
                if (!AdjustToMapping(algorithm, vehicle, out message))
                {
                    message = $"Vehicle at index {v} could not be mapped: {message}";
                    mappedVehiclePool = null;
                    return false;
                }
            }

            message = string.Empty;
            return true;
        }

        internal static bool AdjustToMapping(this IWeightMatrixAlgorithm<float> algorithm, Vehicle vehicle,
            out string message)
        {
            if (vehicle.Arrival.HasValue)
            {
                if (algorithm.MassResolver.Errors.TryGetValue(vehicle.Arrival.Value, out var locationError))
                {
                    message = $"Arrival location is in error: {locationError.Code} - {locationError.Message}.";
                    return false;
                }
                var resolvedIdx = algorithm.MassResolver.ResolvedIndexOf(vehicle.Arrival.Value);
                if (algorithm.Errors.TryGetValue(resolvedIdx, out var error))
                {
                    message = $"Arrival location is in error: {error.Code} - {error.Message}.";
                    return false;
                }
                vehicle.Arrival = algorithm.WeightIndex(vehicle.Arrival.Value);
            }

            if (vehicle.Departure.HasValue)
            {
                if (algorithm.MassResolver.Errors.TryGetValue(vehicle.Departure.Value, out var locationError))
                {
                    message = $"Departure location is in error: {locationError.Code} - {locationError.Message}.";
                    return false;
                }
                var resolvedIdx = algorithm.MassResolver.ResolvedIndexOf(vehicle.Departure.Value);
                if (algorithm.Errors.TryGetValue(resolvedIdx, out var error))
                {
                    message = $"Departure location is in error: {error.Code} - {error.Message}.";
                    return false;
                }
                vehicle.Departure = algorithm.WeightIndex(vehicle.Departure.Value);
            }

            message = string.Empty;
            return true;
        }
    }
}