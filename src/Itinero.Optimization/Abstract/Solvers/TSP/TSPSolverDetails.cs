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
using Itinero.Optimization.Abstract.Solvers.TSP;
using Itinero.Optimization.Models.Mapping;
using Itinero.Optimization.Abstract.Tours;

namespace Itinero.Optimization.Abstract.Solvers.TSP
{
    /// <summary>
    /// Hooks the TSP solver up to the solver registry by defining solver details.
    /// </summary>
    public static class TSPSolverDetails
    {
        /// <summary>
        /// Gets the default solver details.
        /// </summary>
        /// <returns></returns>
        public static SolverDetails Default = new SolverDetails()
        {
            Name = "TSP",
            TrySolve = TrySolve
        };

        private static Result<IList<ITour>> TrySolve(MappedModel mappedModel, Action<IList<ITour>> intermediateResult)
        {
            var result = mappedModel.TryToTSP();

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
        /// Returns true if the given model is a TSP.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static bool IsTSP(this AbstractModel model)
        {
            string reasonIfNot;
            return model.IsTSP(out reasonIfNot);
        }

        /// <summary>
        /// Returns true if the given model is a TSP.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="reasonIfNot">The reason if it's not considered a TSP.</param>
        /// <returns></returns>
        public static bool IsTSP(this AbstractModel model, out string reasonIfNot)
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
            if (model.TimeWindows != null &&
                model.TimeWindows.Length > 0)
            {
                // TODO: check if timewindows are there but are all set to max.
                reasonIfNot = "Timewindows detected.";
                return false;
            }
            return true;
        }

        /// <summary>
        /// Converts the given abstract model to a TSP.
        /// </summary>
        public static Result<ITSProblem> TryToTSP(this MappedModel mappedModel)
        {
            var model = mappedModel.BuildAbstract();

            string reasonWhenFailed;
            if (!model.IsTSP(out reasonWhenFailed))
            {
                return new Result<ITSProblem>("Model is not a TSP: " +
                    reasonWhenFailed);
            }

            var vehicle = model.VehiclePool.Vehicles[0];
            var metric = vehicle.Metric;
            if (!model.TryGetTravelCostsForMetric(metric, out Models.Costs.TravelCostMatrix weights))
            {
                throw new Exception("Travel costs not found but model was declared valid.");
            }
            int first = 0;
            
            var problem = new TSProblem(first, weights.Costs);
            if (vehicle.Departure.HasValue)
            {
                problem.First = vehicle.Departure.Value;
            }
            if (vehicle.Arrival.HasValue)
            {
                problem.Last = vehicle.Arrival.Value;
            }
            return new Result<ITSProblem>(problem);
        }
    }
}