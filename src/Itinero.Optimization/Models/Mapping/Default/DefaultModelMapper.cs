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
using System.Diagnostics;
using Itinero.Algorithms.Matrices;
using Itinero.Algorithms.Search;
using Itinero.LocalGeo;
using Itinero.Optimization.Models.Vehicles;
using Itinero.Optimization.Models.Visits;
using Itinero.Optimization.Models.Visits.Costs;
using Itinero.Profiles;
using Vehicle = Itinero.Optimization.Models.Vehicles.Vehicle;

namespace Itinero.Optimization.Models.Mapping.Default
{
    /// <summary>
    /// A default model mapper.
    /// </summary>
    internal static class DefaultModelMapper
    {
        /// <summary>
        /// Implements the default model mapper.
        /// </summary>
        /// <param name="router">The router to use.</param>
        /// <param name="model">The model to map.</param>
        /// <param name="mappings">The mappings.</param>
        /// <param name="message">The reason why if the mapping fails.</param>
        /// <returns>True if mapping succeeds.</returns>
        internal static bool TryMap(RouterBase router, Model model, out (MappedModel mappedModel, IModelMapping modelMapping) mappings,
            out string message)
        {
            Debug.Assert(model.IsValid(out var _)); // model is asumed to be valid.
            
            // Verify if this mapper can handle this model:
            // - check if there are any vehicles with a turn-cost.
            // - check if there is only one metric defined.
            // - check if there is only one profile defined.
            // - check if the profile is supported.
            // - check if metrics match.
            var metric = model.VehiclePool.Vehicles[0].Metric; // this exists because the model was validated.
            var profileName = model.VehiclePool.Vehicles[0].Profile;
            for (var v = 1; v < model.VehiclePool.Vehicles.Length; v++)
            {
                var vehicle = model.VehiclePool.Vehicles[v];
                if (vehicle.Metric != metric)
                {
                    message =
                        $"Two different vehicle metrics found: {metric} at index '0' and {vehicle.Metric} at '{v}'.";
                    mappings = (null, null);
                    return false;
                }
                if (vehicle.Profile != profileName)
                {
                    message =
                        $"Two different vehicle profiles found: {profileName} at index '0' and {vehicle.Profile} at '{v}'.";
                    mappings = (null, null);
                    return false;
                }

                if (vehicle.TurnPentalty != 0)
                {
                    message =
                        $"A vehicle was found with a turn penalty: {vehicle.TurnPentalty} at index '{v}'.";
                    mappings = (null, null);
                    return false;
                }
            }
            
            if (!router.Db.SupportProfile(profileName))
            {
                message =
                    $"The vehicle profile is not supported: '{profileName}'.";
                mappings = (null, null);
                return false;
            }
            var profile = router.Db.GetSupportedProfile(profileName);
            if (profile.Metric.ToModelMetric() != metric)
            {
                message =
                    $"The vehicle profile metric '{profile.Metric.ToModelMetric()}' doesn't match what was defined in the vehicle: '{metric}'.";
                mappings = (null, null);
                return false;
            }
            
            var locations = new Coordinate[model.Visits.Length];
            for (var i = 0; i < locations.Length; i++)
            {
                locations[i] = new Coordinate()
                {
                    Latitude = model.Visits[i].Latitude,
                    Longitude = model.Visits[i].Longitude
                };
            }
            
            // do mass resolving.
            var massResolvingAlgorithm =
                new MassResolvingAlgorithm(router, new IProfileInstance[] { profile }, locations, null, maxSearchDistance: 250f);
            massResolvingAlgorithm.Run();
            if (!massResolvingAlgorithm.HasSucceeded)
            {
                message =
                    $"Resolving failed: {massResolvingAlgorithm.ErrorMessage}";
                mappings = (null, null);
                return false;
            }

            var weightMatrixAlgorithm = new WeightMatrixAlgorithm(router, profile, massResolvingAlgorithm);
            weightMatrixAlgorithm.Run();
            if (!weightMatrixAlgorithm.HasSucceeded)
            {
                message =
                    $"Calculating weight matrix failed: {weightMatrixAlgorithm.ErrorMessage}";
                mappings = (null, null);
                return false;
            }
            
            // build mapping.
            var modelMapping = new DefaultModelMapping(router, massResolvingAlgorithm, weightMatrixAlgorithm);
            
            // try to adjust the vehicle pool to the mapping, if any of the defined departure or arrival points cannot be mapped then this will fail.
            if (!weightMatrixAlgorithm.TryToMap(model.VehiclePool, out var mappedVehiclePool, out message))
            {
                mappings = (null, null);
                return false;
            }
            
            // build mapped model.
            var mappedModel = new MappedModel()
            {
                Visits = weightMatrixAlgorithm.AdjustToMapping(model.Visits),
                VehiclePool = mappedVehiclePool,
                TravelCosts = new TravelCostMatrix[]
                {
                    new TravelCostMatrix()
                    {
                        Costs = weightMatrixAlgorithm.Weights,
                        Directed = false,
                        Metric = metric
                    }
                }
            };

            mappings = (mappedModel, modelMapping);
            message = string.Empty;
            return true;
        }
        
        private static bool TryToMap(this IWeightMatrixAlgorithm<float> algorithm, VehiclePool vehiclePool, out VehiclePool mappedVehiclePool,
            out string message)
        {
            if (algorithm.Errors.Count == 0)
            { // don't copy if no errors.
                mappedVehiclePool = vehiclePool;
            }

            mappedVehiclePool = new VehiclePool()
            {
                Reusable = vehiclePool.Reusable,
                Vehicles = vehiclePool.Vehicles
            };
            for (var v = 0; v < mappedVehiclePool.Vehicles.Length; v++)
            {
                var vehicle = mappedVehiclePool.Vehicles[v];
                if (!algorithm.AdjustToMapping(vehicle, out message))
                {
                    message = $"Vehicle at index {v} could not be mapped: {message}";
                    mappedVehiclePool = null;
                    return false;
                }
            }
        }

        private static bool AdjustToMapping(this IWeightMatrixAlgorithm<float> algorithm, Vehicle vehicle,
            out string message)
        {
            if (vehicle.Arrival.HasValue)
            {
                if (algorithm.Errors.TryGetValue(vehicle.Arrival.Value, out var error))
                {
                    message = $"Arrival location is in error: {error.Code} - {error.Message}.";
                    return false;
                }
                vehicle.Arrival = algorithm.WeightIndex(vehicle.Arrival.Value);
            }

            if (vehicle.Departure.HasValue)
            {
                if (algorithm.Errors.TryGetValue(vehicle.Departure.Value, out var error))
                {
                    message = $"Departure location is in error: {error.Code} - {error.Message}.";
                    return false;
                }
                vehicle.Departure = algorithm.WeightIndex(vehicle.Departure.Value);
            }
        }
        
        private static Visit[] AdjustToMapping(this IWeightMatrixAlgorithm<float> algorithm, Visit[] ar)
        {
            if (algorithm.Weights.Length == ar.Length)
            { // don't copy if no errors.
                return ar;
            }
            
            var newAr = new W[algorithm.Weights.Length];
            for (var i = 0; i < newAr.Length; i++)
            {
                newAr[i] = ar[algorithm.OriginalLocationIndex(i)];
            }
            return newAr;
        }
    }
}