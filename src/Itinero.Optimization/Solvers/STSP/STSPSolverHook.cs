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
using Itinero.Optimization.Models.Visits.Costs;

namespace Itinero.Optimization.Solvers.STSP
{
    /// <summary>
    /// Hooks the STSP solvers into the solver registry.
    /// </summary>
    internal static class STSPSolverHook
    {
        /// <summary>
        /// The default solver hook for the TSP.
        /// </summary>
        public static readonly SolverRegistry.SolverHook Default = new SolverRegistry.SolverHook()
        {
            Name = "STSP",
            TrySolve = STSPSolverHook.Solve
        };

        private static Result<IEnumerable<(int vehicle, IEnumerable<int> tour)>> Solve(MappedModel model,
            Action<IEnumerable<(int vehicle, IEnumerable<int>)>> intermediateResult)
        {
            var stsp = model.TryToSTSP();
            if (stsp.IsError)
            {
                return stsp.ConvertError<IEnumerable<(int vehicle, IEnumerable<int> tour)>>();
            }

            // use a default solver to solve the STSP here.
            var solution = GASolver.Default.Search(stsp.Value);
            return new Result<IEnumerable<(int vehicle, IEnumerable<int> tour)>>(
                (new (int vehicle, IEnumerable<int> tour)[] {(0, solution.Solution)}));
        }

        /// <summary>
        /// Converts the given abstract model to a STSP.
        /// </summary>
        internal static Result<STSProblem> TryToSTSP(this MappedModel model)
        {
            if (!model.IsTSP(out var reasonWhenFailed))
            {
                return new Result<STSProblem>($"Model is not an STSP: {reasonWhenFailed}");
            }

            var vehicle = model.VehiclePool.Vehicles[0];
            var capacity = vehicle.CapacityConstraints[0].Capacity;
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
            return new Result<STSProblem>(new STSProblem(first, last, weights.Costs, capacity));
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
            if (vehicle.CapacityConstraints == null ||
                vehicle.CapacityConstraints.Length != 1)
            {
                reasonIfNot = "The STSP needs exactly one capacity constraint.";
                return false;
            }

            if (!model.TryGetTravelCostsForMetric(vehicle.CapacityConstraints[0].Metric, out _))
            {
                reasonIfNot = "The STSP needs exactly one capacity constraint that matches the metric used by the travel matrix and vehicle.";
                return false;
            }
            foreach (var visit in model.Visits)
            {
                if (visit.TimeWindow == null || visit.TimeWindow.IsEmpty) continue;
                reasonIfNot = "Timewindows detected.";
                return false;
            }

            reasonIfNot = string.Empty;
            return true;
        }
    }
}