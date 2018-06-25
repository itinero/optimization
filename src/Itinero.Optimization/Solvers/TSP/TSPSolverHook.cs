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
using Itinero.Logging;
using Itinero.Optimization.Models.Mapping;
using Itinero.Optimization.Solvers.TSP.EAX;

namespace Itinero.Optimization.Solvers.TSP
{
    /// <summary>
    /// Hooks the TSP solvers into the solver registry.
    /// </summary>
    internal static class TSPSolverHook
    {
        /// <summary>
        /// The default solver hook for the TSP.
        /// </summary>
        public static readonly SolverRegistry.SolverHook Default = new SolverRegistry.SolverHook()
        {
            Name = "TSP",
            TrySolve = TSPSolverHook.Solve
        };

        private static Result<IEnumerable<(int vehicle, IEnumerable<int> tour)>> Solve(MappedModel model, Action<IList<IEnumerable<int>>> intermediateResult)
        {
            var tsp = model.TryToTSP();
            if (tsp.IsError)
            {
                return tsp.ConvertError<IEnumerable<(int vehicle, IEnumerable<int> tour)>>();
            }
            
            // use a default solver to solve the TSP here.
            try
            {
                var solution = EAXSolver.Default.Search(tsp.Value);
                return new Result<IEnumerable<(int vehicle, IEnumerable<int> tour)>>(
                    (new (int vehicle, IEnumerable<int> tour) [] { (0, solution.Solution) }));
            }
            catch (Exception ex)
            {
                Logger.Log($"{typeof(TSPSolverHook)}.{nameof(Solve)}", TraceEventType.Critical, $"Unhandled exception: {ex.ToString()}.");
                return new Result<IEnumerable<(int vehicle, IEnumerable<int> tour)>>($"Unhandled exception: {ex.ToString()}.");
            }
        }

        /// <summary>
        /// Converts the given abstract model to a TSP.
        /// </summary>
        internal static Result<TSProblem> TryToTSP(this MappedModel model)
        {
            if (!model.IsTSP(out var reasonWhenFailed))
            {
                return new Result<TSProblem>($"Model is not a TSP: {reasonWhenFailed}");
            }

            var vehicle = model.VehiclePool.Vehicles[0];
            var metric = vehicle.Metric;
            if (!model.TryGetTravelCostsForMetric(metric, out var weights))
            {
                throw new Exception("Travel costs not found but model was declared valid.");
            }
            var first = 0;
            int? last = null;
            if (vehicle.Departure.HasValue)
            {
                first = vehicle.Departure.Value;
            }
            if (vehicle.Arrival.HasValue)
            {
                last = vehicle.Arrival.Value;
            }
            return new Result<TSProblem>(new TSProblem(first, last, weights.Costs));
        }

        private static bool IsTSP(this MappedModel model, out string reasonIfNot)
        {
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
            foreach (var visit in model.Visits)
            {
                if (visit.TimeWindow == null || visit.TimeWindow.IsUnlimited) continue;
                reasonIfNot = "Timewindows detected.";
                return false;
            }

            reasonIfNot = string.Empty;
            return true;
        }
    }
}