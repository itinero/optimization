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
using System.IO;
using Itinero.Optimization.Models.Costs;
using Itinero.Optimization.Models.Vehicles.Constraints;

namespace Itinero.Optimization.Test.Functional.VRP.NoDepot.Capacitated
{
    public static class NoDepotCVRPWithDepotConstraintTests
    {
        private const string FileNamePrefix = "no-depot-CVRP-with-depot-constraint-";
        private const string Name = "No Depot CVRP with Depot constraint.";

        public static void Run()
        {
            Run1Wechelderzande();
            Run1WechelderzandeCapacitated();
            Run2Spijkenisse();
            //Run2SpijkenisseCapacitated(); // still fails.
            Run2SpijkenisseVisitCosts();
            Run3DeHague();
            Run4Rotterdam();
            Run5Rotterdam();
        }

        private static void Run1Wechelderzande()
        {
            // WECHELDERZANDE - LILLE
            // build routerdb and save the result.
            var lille = Staging.RouterDbBuilder.Build("query3");
            var vehicle = lille.GetSupportedVehicle("car");
            var router = new Router(lille);

            // build problem1.
            var locations = Staging.StagingHelpers.GetLocations(
                Staging.StagingHelpers.GetFeatureCollection("data.NoDepotCVRP.problem1-depot.geojson"));

            // 
            var func = new Func<List<Route>>(() => router.CalculateNoDepotCVRP(vehicle.Fastest(), locations, 1000, 0));
            var routes = func.TestPerf(Name + " (Wechelderzande)");

            //#if DEBUG
            // write info about result.
            routes.WriteStats();
            routes.WriteGeoJson(FileNamePrefix + "lille-{0}.geojson");
            //#endif
        }

        private static void Run1WechelderzandeCapacitated()
        {
            // WECHELDERZANDE - LILLE
            // build routerdb and save the result.
            var lille = Staging.RouterDbBuilder.Build("query3");
            var vehicle = lille.GetSupportedVehicle("car");
            var router = new Router(lille);

            // build problem1.
            var locations = Staging.StagingHelpers.GetLocations(
                Staging.StagingHelpers.GetFeatureCollection("data.NoDepotCVRP.problem1-depot.geojson"));

            // build capacity constraints
            var capacityConstraint = new CapacityConstraint[]
            {
                new CapacityConstraint()
                {
                    Name = Itinero.Optimization.Models.Metrics.Time,
                    Capacity = 900
                },
                new CapacityConstraint()
                {
                    Name = Itinero.Optimization.Models.Metrics.Weight,
                    Capacity = 500
                }
            };

            // build visit costs.
            var visitCostWeight = new VisitCosts()
            {
                Name = Itinero.Optimization.Models.Metrics.Weight,
                Costs = new float[locations.Length]
            };
            for (var l = 0; l < locations.Length; l++)
            {
                visitCostWeight.Costs[l] = 100;
            }
            var visitCosts = new VisitCosts[]
            {
                visitCostWeight
            };

            // 
            var func = new Func<List<Route>>(() => router.CalculateNoDepotCVRP(vehicle.Fastest(), locations, capacityConstraint,
                visitCosts, 0));
            var routes = func.TestPerf(Name + " (Wechelderzande - Capacitated)");

            //#if DEBUG

            // write info about result.
            routes.WriteStats();
            routes.WriteGeoJson(FileNamePrefix + "lille-capacitated-{0}.geojson");
            //#endif
        }

        private static void Run2Spijkenisse()
        {
            // SPIJKENISSE
            // build routerdb and save the result.
            var spijkenisse = Staging.RouterDbBuilder.Build("query4");
            var vehicle = spijkenisse.GetSupportedVehicle("car");
            var router = new Router(spijkenisse);

            // build problem.
            var locations = Staging.StagingHelpers.GetLocations(
                Staging.StagingHelpers.GetFeatureCollection("data.NoDepotCVRP.problem2-spijkenisse-depot.geojson"));

            // 
            var max = 5400;
            var func = new Func<List<Route>>(() => router.CalculateNoDepotCVRP(vehicle.Fastest(), locations, max, 0));
            var routes = func.TestPerf(Name + " (Spijkenisse)");

            //#if DEBUG

            // write info about result.
            routes.WriteStats();
            const string filename = FileNamePrefix + "spijkenisse-{0}";
            routes.WriteGeoJson(filename + ".geojson");
            routes.WriteJson(filename + ".json");
            //#endif
        }

        private static void Run2SpijkenisseCapacitated()
        {
            // SPIJKENISSE
            // build routerdb and save the result.
            var spijkenisse = Staging.RouterDbBuilder.Build("query4");
            var vehicle = spijkenisse.GetSupportedVehicle("car");
            var router = new Router(spijkenisse);

            // build problem.
            var locations = Staging.StagingHelpers.GetLocations(
                Staging.StagingHelpers.GetFeatureCollection("data.NoDepotCVRP.problem2-spijkenisse-depot.geojson"));

            // build capacity constraints
            var capacityConstraint = new CapacityConstraint[]
            {
                new CapacityConstraint()
                {
                    Name = Itinero.Optimization.Models.Metrics.Time,
                    Capacity = 5400
                },
                new CapacityConstraint()
                {
                    Name = Itinero.Optimization.Models.Metrics.Weight,
                    Capacity = 3000
                }
            };

            // build visit costs.
            var visitCostWeight = new VisitCosts()
            {
                Name = Itinero.Optimization.Models.Metrics.Weight,
                Costs = new float[locations.Length]
            };
            for (var l = 0; l < locations.Length; l++)
            {
                visitCostWeight.Costs[l] = 100;
            }
            var visitCosts = new VisitCosts[]
            {
                visitCostWeight
            };

            // 
            var func = new Func<List<Route>>(() => router.CalculateNoDepotCVRP(vehicle.Fastest(), locations,
                capacityConstraint, visitCosts));
            var routes = func.TestPerf(Name + " (Spijkenisse - Capacitated)");

            //#if DEBUG

            // write info about result.
            routes.WriteStats();
            routes.WriteGeoJson(FileNamePrefix + "spijkenisse-capacitated-{0}.geojson");
            //#endif
        }

        private static void Run2SpijkenisseVisitCosts()
        {
            // SPIJKENISSE
            // build routerdb and save the result.
            var spijkenisse = Staging.RouterDbBuilder.Build("query4");
            var vehicle = spijkenisse.GetSupportedVehicle("car");
            var router = new Router(spijkenisse);

            // build problem.
            var locations = Staging.StagingHelpers.GetLocations(
                Staging.StagingHelpers.GetFeatureCollection("data.NoDepotCVRP.problem2-spijkenisse-depot.geojson"));

            // build capacity constraints
            var capacityConstraint = new CapacityConstraint[]
            {
                new CapacityConstraint()
                {
                    Name = Itinero.Optimization.Models.Metrics.Time,
                    Capacity = 3600 * 4
                }
            };

            // build visit costs.
            var visitCostWeight = new VisitCosts()
            {
                Name = Itinero.Optimization.Models.Metrics.Time,
                Costs = new float[locations.Length]
            };
            for (var l = 0; l < locations.Length; l++)
            {
                visitCostWeight.Costs[l] = 180;
            }
            var visitCosts = new VisitCosts[]
            {
                visitCostWeight
            };

            // 
            var func = new Func<List<Route>>(() => router.CalculateNoDepotCVRP(vehicle.Fastest(), locations,
                capacityConstraint, visitCosts, 0));
            var routes = func.TestPerf(Name + " (Spijkenisse - Capacitated & Visit Costs)");

            //#if DEBUG

            // write info about result.
            routes.WriteStats();
            var filename = FileNamePrefix + "spijkenisse-visit-costs-{0}";
            routes.WriteGeoJson(filename + ".geojson");
            routes.WriteJson(filename + ".json");
            //#endif
        }

        private static void Run3DeHague()
        {
            // DE-HAGUE
            // build routerdb and save the result.
            var dehague = Staging.RouterDbBuilder.Build("query5");
            var vehicle = dehague.GetSupportedVehicle("car");
            var router = new Router(dehague);

            // build problem.
            var locations = Staging.StagingHelpers.GetLocations(
                Staging.StagingHelpers.GetFeatureCollection("data.NoDepotCVRP.problem3-de-hague-depot.geojson"));

            // 
            var max = 3800;
            var func = new Func<List<Route>>(() => router.CalculateNoDepotCVRP(vehicle.Fastest(), locations, max, 0));
            var routes = func.TestPerf("No-Depot Capacitated VRP (De Haque)");

            //#if DEBUG

            // write info about result.
            routes.WriteStats();
            routes.WriteGeoJson(FileNamePrefix + "de-hague-{0}.geojson");
            //#endif
        }

        private static void Run4Rotterdam()
        {
            // ROTTERDAM
            // build routerdb and save the result.
            var rotterdam = Staging.RouterDbBuilder.Build("query6");
            var vehicle = rotterdam.GetSupportedVehicle("car");
            var router = new Router(rotterdam);

            // build problem.
            var locations = Staging.StagingHelpers.GetLocations(
                Staging.StagingHelpers.GetFeatureCollection("data.NoDepotCVRP.problem4-rotterdam-depot.geojson"));

            // 
            var max = 4500;
            var func = new Func<List<Route>>(() => router.CalculateNoDepotCVRP(vehicle.Fastest(), locations, max, 0));
            var routes = func.TestPerf(Name + " (Rotterdam4)");

            //#if DEBUG

            // write info about result.
            routes.WriteStats();
            routes.WriteGeoJson(FileNamePrefix + "rotterdam4-{0}.geojson");
            //#endif 
        }

        private static void Run5Rotterdam()
        {
            // ROTTERDAM
            // build routerdb and save the result.
            var rotterdam = Staging.RouterDbBuilder.Build("query7");
            var vehicle = rotterdam.GetSupportedVehicle("car");
            var router = new Router(rotterdam);

            // build problem.
            var locations = Staging.StagingHelpers.GetLocations(
                Staging.StagingHelpers.GetFeatureCollection("data.NoDepotCVRP.problem5-rotterdam-depot.geojson"));

            // 
            var max = 4500;
            var func = new Func<List<Route>>(() => router.CalculateNoDepotCVRP(vehicle.Fastest(), locations, max, 0));
            var routes = func.TestPerf(Name + " (Rotterdam5)");

            //#if DEBUG

            // write info about result.
            routes.WriteStats();
            routes.WriteGeoJson(FileNamePrefix + "rotterdam5-{0}.geojson");
            //#endif 
        }
    }
}