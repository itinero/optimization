/*
 *  Licensed to SharpSoftware under one or more contributor
 *  license agreements. See the NOTICE file distributed with this work for 
 *  additional information regarding copyright ownership.
 * 
 *  SharpSoftware licenses this file to you under the Apache License, 
 *  Version 2.0 (the "License"); you may not use this file except in 
 *  compliance with the License. You may obtain a copy of the License at
 * 
 *       http://www.apache.org/licenses/LICENSE-2.0
 * 
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */

using System.Collections.Generic;
using Itinero.Optimization.Models.TimeWindows;
using Itinero.Optimization.Models.Vehicles;
using Itinero.Optimization.Models.Visits;

namespace Itinero.Optimization.Models
{
    /// <summary>
    /// Contains extension methods for model validation.
    /// </summary>
    public static class ModelValidation
    {
        /// <summary>
        /// Validates the model, returns a message if invalid.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="message">The message if invalid.</param>
        /// <returns>True if valid, false otherwise.</returns>
        internal static bool IsValid(this Model model, out string message)
        {
            if (model.VehiclePool == null)
            {
                message = "No vehicle pool defined.";
                return false;
            }
            if (!model.VehiclePool.IsValid(out var vpMessage))
            {
                message = $"Invalid vehicle pool: {vpMessage}";
                return false;
            }
            
            if (model.Visits == null ||
                model.Visits.Length == 0)
            {
                message = "No visits defined.";
                return false;
            }
            if (!model.Visits.AreValid(out message))
            {
                message = $"Invalid visit: {message}";
                return false;
            }

            message = string.Empty;
            return true;
        }

        /// <summary>
        /// Validates that the given vehicle pool is valid.
        /// </summary>
        /// <param name="vehiclePool">The vehicle pool.</param>
        /// <param name="message">The message if invalid.</param>
        /// <returns>True if valid, false otherwise.</returns>
        private static bool IsValid(this VehiclePool vehiclePool, out string message)
        {
            if (vehiclePool.Vehicles == null ||
                vehiclePool.Vehicles.Length == 0)
            {
                message = "No vehicles defined in vehicle pool, at least one vehicle is required.";
                return false;
            }

            for (var v = 0; v < vehiclePool.Vehicles.Length; v++)
            {
                var vehicle = vehiclePool.Vehicles[v];
                if (vehicle == null)
                {
                    message = $"Vehicle at index {v} is null.";
                    return false;
                }
                if (!vehicle.IsValid(out message))
                {
                    message = $"Vehicle at index {v} is invalid: {message}";
                    return false;
                }
            }

            message = string.Empty;
            return true;
        }

        /// <summary>
        /// Validates that the given vehicle is valid.
        /// </summary>
        /// <param name="vehicle">The vehicle.</param>
        /// <param name="message">The message if invalid.</param>
        /// <returns>True if valid, false otherwise.</returns>
        private static bool IsValid(this Vehicle vehicle, out string message)
        {
            if (string.IsNullOrWhiteSpace(vehicle.Metric))
            {
                message = $"Vehicle doesn't have a metric.";
                return false;
            }
            if (string.IsNullOrWhiteSpace(vehicle.Profile))
            {
                message = $"Vehicle doesn't have a profile.";
                return false;
            }

            if (vehicle.CapacityConstraints != null)
            {
                for (var cc = 0; cc < vehicle.CapacityConstraints.Length; cc++)
                {
                    var capacityContraint = vehicle.CapacityConstraints[cc];
                    if (capacityContraint == null)
                    {
                        message = $"Vehicle has a capacity constraint at index {cc} that's null.";
                        return false;
                    }

                    if (string.IsNullOrWhiteSpace(capacityContraint.Metric))
                    {
                        message = $"Vehicle has a capacity constraint at index {cc} that doesn't have a proper metric set.";
                        return false;
                    }

                    if (capacityContraint.Capacity <= 0)
                    {
                        message = $"Vehicle has a capacity constraint at index {cc} that has a capacity <= 0.";
                        return false;
                    }
                }
            }

            message = string.Empty;
            return true;
        }
        
        /// <summary>
        /// Validates the visits and checks if they are valid.
        /// </summary>
        /// <param name="visits">The visits.</param>
        /// <param name="message">The message if invalid.</param>
        /// <returns>True if valid, false otherwise.</returns>
        private static bool AreValid(this Visit[] visits, out string message)
        {
            var visitCostMetrics = new HashSet<string>();
            for (var v = 0; v < visits.Length; v++)
            {
                var visit = visits[v];
                if (visit == null)
                {
                    message = $"Visit at index {v} is null.";
                    return false;
                }
                if (!visit.TimeWindow.IsValid(out var twMessage))
                {
                    message = $"Time window for visit at index {v} invalid: {twMessage}";
                    return false;
                }

                var visitCosts = visit.VisitCosts;
                if (visitCosts != null)
                {
                    visitCostMetrics.Clear();
                    for (var vc = 0; vc < visitCosts.Length; vc++)
                    {
                        var visitCost = visitCosts[vc];
                        if (visitCost == null)
                        {
                            message = $"Visit at index {v} has a visit cost at index {vc} that is null.";
                            return false;
                        }

                        if (visitCost.Value < 0)
                        {
                            message = $"Visit at index {v} has a visit cost at index {vc} that has a value < 0.";
                            return false;
                        }

                        if (visitCostMetrics.Contains(visitCost.Metric))
                        {
                            message = $"Visit at index {v} has a visit cost at index {vc} that has a duplicate metric {visitCost.Metric}.";
                            return false;
                        }

                        visitCostMetrics.Add(visitCost.Metric);
                    }
                }
            }

            message = string.Empty;
            return true;
        }
        
        /// <summary>
        /// Validates the time window and checks if it's valid.
        /// </summary>
        /// <param name="tw">The time window.</param>
        /// <param name="message">The message if invalid.</param>
        /// <returns>True if valid, false otherwise.</returns>
        private static bool IsValid(this TimeWindow tw, out string message)
        {
            if (tw == null)
            {
                message = string.Empty;
                return true;
            }
            if (tw.IsUnlimited)
            {
                message = string.Empty;
                return true;
            }

            if (tw.Min >= tw.Max)
            {
                message = $"Max has to be >= min: {tw}";
                return false;
            }
            message = string.Empty;
            return true;
        }
    }
}