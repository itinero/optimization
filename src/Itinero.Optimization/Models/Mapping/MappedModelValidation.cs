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

namespace Itinero.Optimization.Models.Mapping
{
    /// <summary>
    /// Contains validation code for mapped models. This verifies contraints that should apply to *any* model.
    /// </summary>
    internal static class MappedModelValidation
    {
        /// <summary>
        /// Returns true if the model is valid or a reason why if not.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="reasonWhy">The reason the model is not valid, empty otherwise.</param>
        /// <returns>True if the model is valid, false otherwise.</returns>
        public static bool IsValid(this MappedModel model, out string reasonWhy)
        {
            // REMARK: validation here counts on the fact that the model that this mapped model is based on was already validated.

            if (model.TravelCosts == null)
            {
                reasonWhy = $"No travel costs found.";
                return false;
            }
            foreach (var vehicle in model.VehiclePool.Vehicles)
            {
                if (!model.TryGetTravelCostsForMetric(vehicle.Metric, out var _))
                {
                    reasonWhy = $"A vehicle with metric '{vehicle.Metric}' was found but no travel costs exist for that metric.";
                    return false;
                }
            }
            
            reasonWhy = string.Empty;
            return true;
        }
    }
}