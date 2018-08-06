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
using Itinero.Optimization.Models;
using Itinero.Optimization.Models.Vehicles;
using Itinero.Optimization.Models.Vehicles.Constraints;
using Itinero.Optimization.Models.Visits;
using Itinero.Optimization.Models.Visits.Costs;

namespace Itinero.Optimization.Tests.Functional.CVRP_ND
{
    public static class CVRPNDTests
    {
        /// <summary>
        /// Runs some functional tests related to the CVRP.
        /// </summary>
        public static void Run()
        {
            //Run1Wechelderzande();
            //Run1WechelderzandeCapacitated();
            Run2Spijkenisse();
        }

        public static void Run1Wechelderzande()
        {
            // WECHELDERZANDE - LILLE
            // build routerdb and save the result.
            var lille = Staging.RouterDbBuilder.Build("query3");
            var vehicle = lille.GetSupportedVehicle("car");
            var router = new Router(lille);

            // build problem1.
            var locations = Staging.StagingHelpers.GetLocations(
                Staging.StagingHelpers.GetFeatureCollection("CVRP_ND.data.problem1.geojson"));
            
            // build vehicle pool.
            var vehicles = VehiclePool.FromProfile(vehicle.Fastest(), null, null, 1000, true);
            
            // run
            Func<Action<IEnumerable<Result<Route>>>, IEnumerable<Result<Route>>> func = (intermediateRoutesFunc) =>
                router.Optimize(vehicles, locations, out _, intermediateRoutesFunc);
            func.RunWithIntermedidates("CVRP-ND-wechelderzande");
        }

        public static void Run1WechelderzandeCapacitated()
        {
            // WECHELDERZANDE - LILLE
            // build routerdb and save the result.
            var lille = Staging.RouterDbBuilder.Build("query3");
            var vehicle = lille.GetSupportedVehicle("car");
            var router = new Router(lille);

            // build problem1.
            var locations = Staging.StagingHelpers.GetLocations(
                Staging.StagingHelpers.GetFeatureCollection("CVRP_ND.data.problem1.geojson"));

            // build vehicle pool and capacity constraints.
            var vehicles = new VehiclePool()
            {
                Reusable = true,
                Vehicles = new[]
                {
                    new Vehicle()
                    {
                        Profile = vehicle.Fastest().FullName,
                        Metric =  vehicle.Fastest().Metric.ToModelMetric(),
                        Departure = null,
                        Arrival = null,
                        CapacityConstraints = new CapacityConstraint[]
                        {
                            new CapacityConstraint()
                            {
                                Metric = Itinero.Optimization.Models.Metrics.Time,
                                Capacity = 900
                            },
                            new CapacityConstraint()
                            {
                                Metric = Itinero.Optimization.Models.Metrics.Weight,
                                Capacity = 500
                            }
                        }
                    }
                }
            };

            // build visits.
            var visits = new Visit[locations.Length];
            for (var v = 0; v < locations.Length; v++)
            {
                visits[v] = new Visit()
                {
                    Latitude = locations[v].Latitude,
                    Longitude = locations[v].Longitude,
                    VisitCosts = new VisitCost[]
                    {
                        new VisitCost()
                        {
                            Metric = Itinero.Optimization.Models.Metrics.Weight,
                            Value = 100
                        }
                    }
                };
            }

            // run
            Func<Action<IEnumerable<Result<Route>>>, IEnumerable<Result<Route>>> func = (intermediateRoutesFunc) =>
                router.Optimize(vehicles, visits, out _, intermediateRoutesFunc);
            func.RunWithIntermedidates("CVRP-ND-wechelderzande-capacitated");
        }
        
        public static void Run2Spijkenisse()
        {
            // SPIJKENISSE
            // build routerdb and save the result.
            var spijkenisse = Staging.RouterDbBuilder.Build("query4");
            var vehicle = spijkenisse.GetSupportedVehicle("car");
            var router = new Router(spijkenisse);

            // build problem.
            const int max = 5400;
            var locations = Staging.StagingHelpers.GetLocations(
                Staging.StagingHelpers.GetFeatureCollection("CVRP_ND.data.problem2-spijkenisse.geojson"));  
            
            // build vehicle pool.
            var vehicles = VehiclePool.FromProfile(vehicle.Fastest(), null, null, max, true);
            
            // run
            Func<Action<IEnumerable<Result<Route>>>, IEnumerable<Result<Route>>> func = (intermediateRoutesFunc) =>
                router.Optimize(vehicles, locations, out _, intermediateRoutesFunc);
            func.RunWithIntermedidates("CVRP-ND-spijkenisse");
        }
    }
}