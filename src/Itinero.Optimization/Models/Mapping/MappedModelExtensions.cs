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

using Itinero.Optimization.Models.Visits.Costs;

namespace Itinero.Optimization.Models.Mapping
{
    /// <summary>
    /// Contains extensions methods related to the models.
    /// </summary>
    internal static class MappedModelExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="metric"></param>
        /// <param name="travelCosts"></param>
        /// <returns></returns>
        public static bool TryGetTravelCostsForMetric(this MappedModel model, string metric, out TravelCostMatrix travelCosts)
        {
            foreach (var t in model.TravelCosts)
            {
                if (t.Metric != metric) continue;
                travelCosts = t;
                return true;
            }
            travelCosts = null;
            return false;
        }
    }
}