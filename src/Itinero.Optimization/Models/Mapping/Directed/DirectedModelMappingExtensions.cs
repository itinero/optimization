using System.Linq;
using Itinero.Algorithms.Matrices;
using Itinero.Optimization.Models.Vehicles;
using Itinero.Optimization.Models.Visits;

namespace Itinero.Optimization.Models.Mapping.Directed
{
    /// <summary>
    /// Extensions methods for the directed model mapping.
    /// </summary>
    public static class DirectedModelMappingExtensions
    {
        /// <summary>
        /// Try to map the the vehicle pool according to what's found in the weight matrix.
        /// </summary>
        /// <param name="algorithm">The weight matrix.</param>
        /// <param name="vehiclePool">The vehicle pool.</param>
        /// <param name="mappedVehiclePool">The mapped vehicle pool.</param>
        /// <param name="message">The message, if any.</param>
        /// <returns>True if the mapping can be done, false otherwise.</returns>
        internal static bool TryToMap(this IDirectedWeightMatrixAlgorithm<float> algorithm, VehiclePool vehiclePool, out VehiclePool mappedVehiclePool,
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
                if (algorithm.AdjustToMapping(vehicle, out message)) continue;
                
                message = $"Vehicle at index {v} could not be mapped: {message}";
                mappedVehiclePool = null;
                return false;
            }

            message = string.Empty;
            return true;
        }
        
        

        internal static bool AdjustToMapping(this IDirectedWeightMatrixAlgorithm<float> algorithm, Vehicle vehicle,
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
        
        internal static Visit[] AdjustToMapping(this IDirectedWeightMatrixAlgorithm<float> algorithm, Visit[] ar)
        {
            if (algorithm.Weights.Length / 2 == ar.Length)
            { // don't copy if no errors.
                return ar;
            }
            
            var newAr = new Visit[algorithm.Weights.Length / 2];
            for (var i = 0; i < newAr.Length; i++)
            {
                newAr[i] = ar[algorithm.OriginalLocationIndex(i)];
            }
            return newAr;
        }

        internal static void SetOrderAndIndex(this Route localRoute, int visit1, int order1,
            int visit2, int order2)
        {
            // TODO: add visit costs based on the original model!
            if (localRoute.Stops == null || localRoute.Stops.Length != 2) return;
            
            var visit1Stop = localRoute.Stops[0];
            var visit2Stop = localRoute.Stops[1];

            // add visit1 costs & order to the stop meta data.
            visit1Stop.Attributes.AddOrReplace("order", order1.ToInvariantString());
            visit1Stop.Attributes.AddOrReplace("visit", visit1.ToInvariantString());
//                var visit1Costs = mappedModel.Visits[visit1].VisitCosts;
//                if (visit1Costs != null)
//                {
//                    foreach (var visitCost in visit1Costs)
//                    {
//                        visit1Stop.Attributes.AddOrReplace("cost_" + visitCost.Metric.ToLowerInvariant(),
//                            visitCost.Value.ToInvariantString());
//                    }
//
//                    var visit1Cost = visit1Costs.FirstOrDefault(x => x.Metric == Metrics.Time)?.Value;
//                    if (visit1Cost.HasValue)
//                    {
//                        if (localRoute.ShapeMeta != null &&
//                            localRoute.ShapeMeta.Length > 0)
//                        {
//                            for (var sm = 0; sm < localRoute.ShapeMeta.Length; sm++)
//                            {
//                                localRoute.ShapeMeta[sm].Time += visit1Cost.Value;
//                            }
//                        }
//
//                        localRoute.TotalTime += visit1Cost.Value;
//                    }
//                }

            // add visit2 costs & order to the stop meta data.
            visit2Stop.Attributes.AddOrReplace("order", order2.ToInvariantString());
            visit2Stop.Attributes.AddOrReplace("visit", visit2.ToInvariantString());
//                var visit2Costs = mappedModel.Visits[visit2].VisitCosts;
//                if (visit2Costs != null)
//                {
//                    foreach (var visitCost in visit2Costs)
//                    {
//                        visit2Stop.Attributes.AddOrReplace("cost_" + visitCost.Metric.ToLowerInvariant(), visitCost.Value.ToInvariantString());
//                    }
//
//                    // if there is a cost with travel time also add the travel time.
//                    var visit2Cost = visit2Costs.FirstOrDefault(x => x.Metric == Metrics.Time)?.Value;
//                    if (visit2Cost.HasValue)
//                    { // this is the second stop, add the travel time at the end.
//                        if (localRoute.ShapeMeta != null &&
//                            localRoute.ShapeMeta.Length > 0)
//                        {
//                            localRoute.ShapeMeta[localRoute.ShapeMeta.Length - 1].Time += visit2Cost.Value;
//                        }
//                        localRoute.TotalTime += visit2Cost.Value;
//                    }
//                }
        }
    }
}