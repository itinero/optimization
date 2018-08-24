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

using Itinero.Optimization.Models.Visits;
using Itinero.Optimization.Models.Visits.Costs;

namespace Itinero.Optimization.Models.Mapping
{
    /// <summary>
    /// Contains extensions methods related to the models.
    /// </summary>
    public static class MappedModelExtensions
    {
        /// <summary>
        /// Gets the travel cost for the given metric.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="metric">The metric.</param>
        /// <param name="travelCosts">The travel costs.</param>
        /// <returns>True if travel costs are found for the given metric.</returns>
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

        /// <summary>
        /// Gets the visit cost for the given metric.
        /// </summary>
        /// <param name="visit">The visit.</param>
        /// <param name="metric">The metric.</param>
        /// <param name="cost">The visit cost.</param>
        /// <returns>True if the visit cost is found for the given metric.</returns>
        public static bool TryGetVisitCostForMetric(this Visit visit, string metric, out float cost)
        {
            if (visit.VisitCosts == null)
            {
                cost = 0;
                return false;
            }
            
            foreach (var visitCost in visit.VisitCosts)
            {
                if (visitCost.Metric != metric) continue;
                cost = visitCost.Value;
                return true;
            }

            cost = 0;
            return false;
        }
    }
}