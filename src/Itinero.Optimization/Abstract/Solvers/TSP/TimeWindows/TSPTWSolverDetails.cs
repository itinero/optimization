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
using Itinero.Optimization.Abstract.Models;
using Itinero.Optimization.Abstract.Solvers.TSP;
using Itinero.Optimization.Models.Mapping;
using Itinero.Optimization.Tours;

namespace Itinero.Optimization.Abstract.Solvers.TSP.TimeWindows
{
    /// <summary>
    /// Hooks the TSPTW solver up to the solver registry by defining solver details.
    /// </summary>
    public static class TSPTWSolverDetails
    {
        /// <summary>
        /// Gets the default solver details.
        /// </summary>
        /// <returns></returns>
        public static SolverDetails Default = new SolverDetails()
        {
            Name = "TSPTW",
            TrySolve = TrySolve
        };

        private static Result<IList<ITour>> TrySolve(MappedModel mappedModel)
        {
            var model = mappedModel.BuildAbstract();

            var result = model.TryToTSPTW();

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
        /// Converts the given abstract model to a TSP-TW.
        /// </summary>
        public static Result<TSPTWProblem> TryToTSPTW(this AbstractModel model)
        {
            string reasonIfFailed;
            if (!model.IsTSPTW(out reasonIfFailed))
            {
                return new Result<TSPTWProblem>("Model is not a TSP-TW: " + reasonIfFailed);
            }

            var vehicle = model.VehiclePool.Vehicles[0];
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
                return new Result<TSPTWProblem>(new TSPTWProblem(first, vehicle.Arrival.Value, weights.Costs, model.TimeWindows));
            }
            return new Result<TSPTWProblem>(new TSPTWProblem(first, weights.Costs, model.TimeWindows));
        }

        /// <summary>
        /// Returns true if the given model is a TSP-TW.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static bool IsTSPTW(this AbstractModel model)
        {
            string reasonIfNot;
            return model.IsTSPTW(out reasonIfNot);
        }

        /// <summary>
        /// Returns true if the given model is a TSP-TW.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="reasonIfNot">The reason if it's not considered a TSP-TW.</param>
        /// <returns></returns>
        public static bool IsTSPTW(this AbstractModel model, out string reasonIfNot)
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
            if (vehicle.CapacityConstraints != null &&
                vehicle.CapacityConstraints.Length > 0)
            {
                reasonIfNot = "At least one capacity constraint was found.";
                return false;
            }
            if (model.TimeWindows == null)
            {
                // TODO: check if timewindows are there but are all set to max.
                reasonIfNot = "No timewindows detected.";
                return false;
            }
            return true;
        }
    }
}