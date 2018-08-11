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
using Itinero.LocalGeo;
using Itinero.Logging;
using Itinero.Optimization.Models.Mapping;
using Itinero.Optimization.Solvers.CVRP_ND.GA;
using Itinero.Optimization.Solvers.CVRP_ND.SCI;
using Itinero.Optimization.Solvers.CVRP_ND.TourSeeded;
using Itinero.Optimization.Solvers.Shared.Seeds;
using Itinero.Optimization.Strategies;
using Itinero.Optimization.Strategies.GA;

namespace Itinero.Optimization.Solvers.CVRP_ND
{

    /// <summary>
    /// Hooks the CVRP no-depot solvers into the solver registry.
    /// </summary>
    public static class CVRPNDSolverHook
    {
        /// <summary>
        /// The default solver hook for the TSP.
        /// </summary>
        internal static readonly SolverRegistry.SolverHook Default = new SolverRegistry.SolverHook()
        {
            Name = "CVRP-ND",
            TrySolve = CVRPNDSolverHook.Solve
        };

        private static Result<IEnumerable<(int vehicle, IEnumerable<int> tour)>> Solve(MappedModel model, 
            Action<IEnumerable<(int vehicle, IEnumerable<int>)>> intermediateResult)
        {
            var cvrp = model.TryToCVRPND();
            if (cvrp.IsError)
            {
                return cvrp.ConvertError<IEnumerable<(int vehicle, IEnumerable<int> tour)>>();
            }
            
            // use a default solver to solve the TSP here.
            try
            {
                // var solver = new SeededTourStrategy();

                var solver = new GASolver(settings: new GASettings()
                {
                    PopulationSize = 1000,
                    ElitismPercentage = 0.1f,
                    CrossOverPercentage = 10,
                    MutationPercentage = 1,
                    StagnationCount = 10
                }, generator: new SeededTourStrategy());

                var candidate = solver.Search(cvrp.Value);
                var solution = candidate.Solution;

                var vehiclesAndTours = new List<(int vehicle, IEnumerable<int> tour)>();
                for (var t = 0; t < solution.Count; t++)
                {
                    vehiclesAndTours.Add((0, solution.Tour(t)));
                }
                
                return new Result<IEnumerable<(int vehicle, IEnumerable<int> tour)>>(vehiclesAndTours);
            }
            catch (Exception ex)
            {
                Logger.Log($"{typeof(CVRPNDSolverHook)}.{nameof(Solve)}", TraceEventType.Critical, $"Unhandled exception: {ex.ToString()}.");
                return new Result<IEnumerable<(int vehicle, IEnumerable<int> tour)>>($"Unhandled exception: {ex.ToString()}.");
            }
        }

        /// <summary>
        /// Converts the given abstract model to a CVRP-ND.
        /// </summary>
        public static Result<CVRPNDProblem> TryToCVRPND(this MappedModel model)
        {
            if (!model.IsCVRPND(out var reasonWhenFailed))
            {
                return new Result<CVRPNDProblem>($"Model is not a CVRP-ND: {reasonWhenFailed}");
            }

            // get travel weights.
            var vehicle = model.VehiclePool.Vehicles[0];
            var metric = vehicle.Metric;
            if (!model.TryGetTravelCostsForMetric(metric, out var weights))
            {
                throw new Exception("Travel costs not found but model was declared valid.");
            }
            
            // get the constraints.
            var maxWeight = float.MaxValue;
            var visitCostConstraints = new List<(string metric, float max, float[] costs)>();
            foreach (var capacityConstraint in vehicle.CapacityConstraints)
            {
                if (capacityConstraint.Metric == metric)
                {
                    maxWeight = capacityConstraint.Capacity;
                }
                else
                {
                    visitCostConstraints.Add((capacityConstraint.Metric, capacityConstraint.Capacity, null));
                }
            }
            
            // collect visit weights if any and visit locations.
            float[] visitWeights = null;
            var visitLocations = new Coordinate[model.Visits.Length];
            for (var v = 0; v < model.Visits.Length; v++)
            {
                var visit = model.Visits[v];
                visitLocations[v] = new Coordinate()
                {
                    Latitude = visit.Latitude,
                    Longitude = visit.Longitude
                };
                if (visit.TryGetVisitCostForMetric(metric, out var visitCost))
                {
                    if (visitWeights == null) visitWeights = new float[model.Visits.Length];
                    visitWeights[v] = visitCost;
                }

                for (var vcc = 0; vcc < visitCostConstraints.Count; vcc++)
                {
                    var visitCostConstraint = visitCostConstraints[vcc];
                    if (visitCostConstraint.metric == metric) continue;
                    if (!visit.TryGetVisitCostForMetric(visitCostConstraint.metric, out visitCost)) continue;
                    if (visitCostConstraint.costs == null)
                    {
                        visitCostConstraint.costs = new float[model.Visits.Length];
                    }

                    visitCostConstraint.costs[v] = visitCost;
                    visitCostConstraints[vcc] = visitCostConstraint;
                }
            }
            
            return new Result<CVRPNDProblem>(new CVRPNDProblem(weights.Costs, visitWeights, maxWeight, visitCostConstraints, 
                visitLocations: visitLocations));
        }

        private static bool IsCVRPND(this MappedModel model, out string reasonIfNot)
        {
            if (!model.VehiclePool.Reusable ||
                model.VehiclePool.Vehicles.Length > 1)
            {
                reasonIfNot = "Vehicle not reusable or more than one vehicle defined.";
                return false;
            }
            var vehicle = model.VehiclePool.Vehicles[0];
            if (vehicle.TurnPentalty != 0)
            {
                reasonIfNot = "Turning penalty, this is a directed problem.";
                return false;
            }
            if (vehicle.CapacityConstraints == null ||
                vehicle.CapacityConstraints.Length == 0)
            {
                reasonIfNot = "At least one capacity constraint required.";
                return false;
            }

            if (vehicle.Departure != null ||
                vehicle.Arrival != null)
            {
                reasonIfNot = "Vehicle has fixed arrival or departure location.";
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