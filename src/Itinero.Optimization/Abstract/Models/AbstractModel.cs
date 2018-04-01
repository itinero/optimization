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

using System;
using System.Collections.Generic;
using Itinero.Optimization.Abstract.Models.Costs;
using Itinero.Optimization.Abstract.Models.TimeWindows;
using Itinero.Optimization.Abstract.Models.Vehicles;
using Itinero.Optimization.Models;

namespace Itinero.Optimization.Abstract.Models
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
        public VehiclePool VehiclePool { get; set; }

        /// <summary>
        /// Gets a visit cost for the given metric.
        /// </summary>
        /// <param name="metric">The metric to get costs for.</param>
        /// <param name="visitCosts">The visit costs.</param>
        public bool TryGetVisitCostsForMetric(string metric, out VisitCosts visitCosts)
        {
            if (this.VisitCosts == null)
            {
                visitCosts = null;
                return false;
            }

            for (var i = 0; i < this.VisitCosts.Length; i++)
            {
                if (this.VisitCosts[i].Name == metric)
                {
                    visitCosts = this.VisitCosts[i];
                    return true;
                }
            }
            visitCosts = null;
            return false;
        }

        /// <summary>
        /// Returns a travel cost for the given metric or throws an exception of not found.
        /// </summary>
        /// <param name="metric">The metric to get costs for.</param>
        /// <param name="travelCosts">The travel costs.</param>
        public bool TryGetTravelCostsForMetric(string metric, out TravelCostMatrix travelCosts)
        {
            for (var i = 0; i < this.TravelCosts.Length; i++)
            {
                if (this.TravelCosts[i].Name == metric)
                {
                    travelCosts = this.TravelCosts[i];
                    return true;
                }
            }
            travelCosts = null;
            return false;
        }

        /// <summary>
        /// Serializes this model to json.
        /// </summary>
        /// <remarks>The <see cref="IO.Json.JsonSerializer"/> needs to be setup properly.</remarks>
        /// <returns></returns>
        public string ToJson()
        {
            return IO.Json.JsonSerializer.ToJsonFunc(this);
        }

        /// <summary>
        /// Deserializes a model from json.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <returns></returns>
        public static AbstractModel FromJson(string json)
        {
            return IO.Json.JsonSerializer.FromJsonFunc(json, typeof(AbstractModel)) as AbstractModel;
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
            if (model.VehiclePool == null)
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


        /// <summary>
        /// Checks that the constraints are possible: e.g. that no visit requires hauling 1000kg if the max load each vehicle is less.
        /// 
        /// If such faulty visits are detected, this is returned via the out-parameters.
        /// 
        /// If a departure or arrival is included in the vehicle, this is taken into account as well
        /// 
        /// </summary>
        /// <returns>true if all constrains can be met.</returns>
        public static bool SanityCheck(this AbstractModel model, out string failReason, out List<int> faultyVisitids)
        {
            failReason = "";
            faultyVisitids = new List<int>();

            bool allgood = true;
            for (int visitID = 0; visitID < model.VisitCosts[0].Costs.Length; visitID++)
            {
                bool vehFound = false;
                string vehFailReasons = "";
                foreach (var vehicle in model.VehiclePool.Vehicles)
                {
                    string failR = "";
                    bool vehCanHandle = model.CanVehicleHandleVisit(vehicle, visitID, out failR);
                    if (vehCanHandle)
                    {
                        vehFound = true;
                        break;
                    }
                    vehFailReasons += "   " + failR + "\n";
                }

                if (!vehFound)
                {
                    faultyVisitids.Add(visitID);
                    failReason += "Visit " + visitID + " can not be visisted by any vehicle:\n" + vehFailReasons;
                    allgood = false;
                }
            }
            return allgood;

        }


        private static bool CanVehicleHandleVisit(this AbstractModel model, Vehicle vehicle, int visitID, out string failReason)
        {
            failReason = "";
            foreach (var visitCost in model.VisitCosts)
            {
                foreach (var vehConstr in vehicle.CapacityConstraints)
                {
                    if (vehConstr.Name == visitCost.Name)
                    {   // The visitcost is the same as the vehicleconstraint we are checking

                        // what can be visited by this vehicle
                        float maxCost = vehConstr.Capacity;

                        // cost of the visit itself
                        float totalCost = visitCost.Costs[visitID];

                        foreach (var travelCosts in model.TravelCosts)
                        {
                            if (travelCosts.Name == vehConstr.Name)
                            {
                                // As vehicleMetric is the same as the visit cost metric, we have to add the travelcosts as well
                                if (vehicle.Departure != null)
                                {
                                    totalCost += travelCosts.Costs[(int)vehicle.Departure][visitID];
                                }

                                if (vehicle.Departure != null)
                                {
                                    totalCost += travelCosts.Costs[visitID][(int)vehicle.Arrival];
                                }
                            }
                        }

                        if (totalCost > maxCost)
                        {
                            failReason = "Vehicle with metric " + vehConstr.Name + " and capacity "+ vehConstr.Capacity+" "+vehConstr.Name + " can not handle visit " + visitID + " as total cost is " + totalCost + " " + vehConstr.Name;
                            return false;
                        }
                    }

                }
            }
            return true;

        }
    }
}