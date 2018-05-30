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
using Itinero.Optimization.Abstract.Models;
using Itinero.Optimization.Abstract.Models.Costs;
using Itinero.Optimization.Models.Mapping;
using Itinero.Optimization.Abstract.Tours;

namespace Itinero.Optimization.Abstract.Solvers.STSP
{
    /// <summary>
    /// Hooks the STSP solver up to the solver registry by defining solver details.
    /// </summary>
    public static class STSPSolverDetails
    {
        /// <summary>
        /// Gets the default solver details.
        /// </summary>
        /// <returns></returns>
        public static SolverDetails Default = new SolverDetails()
        {
            Name = "STSP",
            TrySolve = TrySolve
        };

        private static Result<IList<ITour>> TrySolve(MappedModel mappedModel, Action<IList<ITour>> intermediateResult)
        {
            var model = mappedModel.BuildAbstract();
            
            var result = model.TryToSTSP();

            if (result.IsError)
            {
                return result.ConvertError<IList<ITour>>();
            }
            
            var solution = result.Value.Solve();

            return new Result<IList<ITour>>(new List<ITour>(
                new ITour[]
                {
                    solution
                }));
        }

        /// <summary>
        /// Converts the given abstract model to a STSP.
        /// </summary>
        public static Result<STSProblem> TryToSTSP(this AbstractModel model)
        {
            string reasonWhenFailed;
            if (!model.IsSTSP(out reasonWhenFailed))
            {
                return new Result<STSProblem>("Model is not an STSP: " + reasonWhenFailed);
            }

            var vehicle = model.VehiclePool.Vehicles[0];
            var capacity = vehicle.CapacityConstraints[0].Capacity;
            var metric = vehicle.Metric;
            if (!model.TryGetTravelCostsForMetric(metric, out Models.Costs.TravelCostMatrix weights))
            {
                throw new System.Exception("Travel costs not found but model was declared valid.");
            }
            int first = 0;
            
            if (vehicle.Departure.HasValue)
            {
                first = vehicle.Departure.Value;
            }
            if (vehicle.Arrival.HasValue)
            {
                return new Result<STSProblem>(new STSProblem(first, vehicle.Arrival.Value, weights.Costs, capacity));
            }
            return new Result<STSProblem>(new STSProblem(first, weights.Costs, capacity));
        }

        /// <summary>
        /// Returns true if the given model is a STSP.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static bool IsSTSP(this AbstractModel model)
        {
            string reasonIfNot;
            return model.IsSTSP(out reasonIfNot);
        }

        /// <summary>
        /// Returns true if the given model is a STSP.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="reasonIfNot">The reason if it's not considered a STSP.</param>
        /// <returns></returns>
        public static bool IsSTSP(this AbstractModel model, out string reasonIfNot)
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
            if (vehicle.TurnPentalty != 0)
            {
                reasonIfNot = "Turning penalty, this is a directed problem.";
                return false;
            }
            if (vehicle.CapacityConstraints == null ||
                vehicle.CapacityConstraints.Length != 1)
            {
                reasonIfNot = "The STSP needs exactly one capacity constraint.";
                return false;
            }
            TravelCostMatrix costs;
            if (!model.TryGetTravelCostsForMetric(vehicle.CapacityConstraints[0].Name, out costs))
            {
                reasonIfNot = "The STSP needs exactly one capacity constraint that matches the metric used by the travel matrix and vehicle.";
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
    }
}