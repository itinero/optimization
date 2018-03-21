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
using Itinero.Optimization.Models.Mapping;
using Itinero.Optimization.Abstract.Solvers.VRP.NoDepot.Capacitated;
using Itinero.Optimization.Abstract.Tours;
using Itinero.Optimization.Abstract.Tours.Typed;
using Itinero.Optimization.Abstract.Models.Costs;
using System;

namespace Itinero.Optimization.Abstract.Solvers.VRP.NoDepot.Capacitated
{
    /// <summary>
    /// Hooks the solver up to the solver registry by defining solver details.
    /// </summary>
    public static class NoDepotCVRPSolverDetails
    {
        /// <summary>
        /// Gets the default solver details.
        /// </summary>
        /// <returns></returns>
        public static SolverDetails Default = new SolverDetails()
        {
            Name = "NoDepotCVRP",
            TrySolve = TrySolve
        };

        private static Result<IList<ITour>> TrySolve(MappedModel mappedModel)
        {
            var model = mappedModel.BuildAbstract();

            // build the no-depot vrp.
            var result = model.TryToNoDepotCVRP();
            if (result.IsError)
            {
                return result.ConvertError<IList<ITour>>();
            }
            
            // call solver.
            var solution = result.Value.Solve((p, tour1, tour2) => 
            {
                return mappedModel.Overlaps(tour1, tour2);
            });

            var tours = new List<ITour>();
            for (var t = 0; t < solution.Count; t++)
            {
                tours.Add(solution.Tour(t));
            }

            return new Result<IList<ITour>>(tours);
        }

        /// <summary>
        /// Converts the given abstract model to a NoDepotCVRP problem.
        /// </summary>
        public static Result<NoDepotCVRProblem> TryToNoDepotCVRP(this AbstractModel model)
        {
            // check if the model is valid.
            string reasonWhenFailed;
            if (!model.IsNoDepotCVRP(out reasonWhenFailed))
            {
                return new Result<NoDepotCVRProblem>("Model is not a No-Depot VRP: " +
                    reasonWhenFailed);
            }

            // get the vehicle.
            var vehicle = model.VehiclePool.Vehicles[0];

            var metric = vehicle.Metric;
            if (!model.TryGetTravelCostsForMetric(metric, out TravelCostMatrix travelCosts))
            {
                throw new Exception("Travel costs not found but model was declared valid.");
            }

            // get the travel cost related constraint.
            Itinero.Optimization.Abstract.Models.Vehicles.Constraints.CapacityConstraint travelCostConstraint = null;
            for (var i = 0; i < vehicle.CapacityConstraints.Length; i++)
            {
                if (vehicle.CapacityConstraints[i].Name == metric)
                {
                    travelCostConstraint = vehicle.CapacityConstraints[i];
                    break;
                }
            }

            // get travel cost visits costs if any.
            if (!model.TryGetVisitCostsForMetric(metric, out VisitCosts travelCostVisitCosts))
            {
                travelCostVisitCosts = null;
            }

            // get the other constraints.
            var constraints = new CapacityConstraint[vehicle.CapacityConstraints.Length - 1];
            if (vehicle.CapacityConstraints.Length > 1)
            {
                var c = 0;
                foreach (var constraint in vehicle.CapacityConstraints)
                {
                    if (constraint.Name == metric)
                    {
                        continue;
                    }
                    if (!model.TryGetVisitCostsForMetric(constraint.Name, out VisitCosts visitCost))
                    {
                        throw new Exception(string.Format("Constraint found for metric {0} but no visits costs found.",
                            constraint.Name));
                    }
                    constraints[c] = new CapacityConstraint()
                    {
                        Name = constraint.Name,
                        Max = constraint.Capacity,
                        Values = visitCost.Costs
                    };
                    c++;
                }
            }
            
            var problem = new NoDepotCVRProblem()
            {
                Weights = travelCosts.Costs,
                Capacity = new Capacity()
                {
                    Max = travelCostConstraint.Capacity,
                    Constraints = constraints
                }
            };

            if (travelCostVisitCosts != null)
            {
                problem.VisitCosts = travelCostVisitCosts.Costs;
            }
            return new Result<NoDepotCVRProblem>(problem);
        }

        /// <summary>
        /// Returns true if the given model can be solved.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static bool IsNoDepotCVRP(this AbstractModel model)
        {
            string reasonIfNot;
            return model.IsNoDepotCVRP(out reasonIfNot);
        }

        /// <summary>
        /// Returns true if the given model can be solved.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="reasonIfNot">The reason if it's not considered solvable.</param>
        /// <returns></returns>
        public static bool IsNoDepotCVRP(this AbstractModel model, out string reasonIfNot)
        {
            if (!model.IsValid(out reasonIfNot))
            {
                reasonIfNot = "Model is invalid: " + reasonIfNot;
                return false;
            }

            if (!model.VehiclePool.Reusable ||
                model.VehiclePool.Vehicles.Length != 1)
            {
                reasonIfNot = "There has to be one reusable vehicle.";
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
                reasonIfNot = "At least one capacity constraint is required.";
                return false;
            }
            if (model.TimeWindows != null &&
                model.TimeWindows.Length > 0)
            {
                // TODO: check if timewindows are there but are all set to max.
                reasonIfNot = "Timewindows detected, not supported.";
                return false;
            }
            return true;
        }
    }
}