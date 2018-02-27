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
using Itinero.Optimization.Capacities;
using Itinero.Optimization.VRP.NoDepot.Capacitated;

namespace Itinero.Optimization.Test.Functional.VRP.NoDepot.Capacitated
{
    public static class NoDepotCVRPTests
    {
        public static void Run()
        {
            Run1Wechelderzande();
            Run1WechelderzandeCapacitated();
            Run2Spijkenisse();
            Run2SpijkenisseCapacitated();
            Run3DeHague();
            Run4Rotterdam();
            Run5Rotterdam();
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
                Staging.StagingHelpers.GetFeatureCollection("data.NoDepotCVRP.problem1.geojson"));

            // 
            var func = new Func<List<Route>>(() => router.CalculateNoDepotCVRP(vehicle.Fastest(), locations, 900));
            var routes = func.TestPerf("No-Depot Capacitated VRP (Wechelderzande)");

//#if DEBUG
            routes.WriteGeoJson("lille-{0}.geojson");
//#endif
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
                Staging.StagingHelpers.GetFeatureCollection("data.NoDepotCVRP.problem1.geojson"));

            // build capacity.
            var capacity = new Capacity()
            {
                Max = 900,
                Constraints = new CapacityConstraint[]
                {
                    new CapacityConstraint()
                    {
                        Name = "weight",
                        Max = 500,
                        Values = new float[locations.Length]
                    }
                }
            };
            for (var l = 0; l < locations.Length; l++)
            {
                capacity.Constraints[0].Values[l] = 100;
            }

            // 
            var func = new Func<List<Route>>(() => router.CalculateNoDepotCVRP(vehicle.Fastest(), locations, capacity));
            var routes = func.TestPerf("No-Depot Capacitated VRP (Wechelderzande - Capacitated)");

//#if DEBUG
            routes.WriteGeoJson("lille-capacitated-{0}.geojson");
//#endif
        }

        public static void Run2Spijkenisse()
        {
            // SPIJKENISSE
            // build routerdb and save the result.
            var spijkenisse = Staging.RouterDbBuilder.Build("query4");
            var vehicle = spijkenisse.GetSupportedVehicle("car");
            var router = new Router(spijkenisse);

            // build problem.
            var locations = Staging.StagingHelpers.GetLocations(
                Staging.StagingHelpers.GetFeatureCollection("data.NoDepotCVRP.problem2-spijkenisse.geojson"));        

            // 
            var max = 5400;
            var func = new Func<List<Route>>(() => router.CalculateNoDepotCVRP(vehicle.Fastest(), locations, max));
            var routes = func.TestPerf("No-Depot Capacitated VRP (Spijkenisse)");

//#if DEBUG
            routes.WriteGeoJson("spijkenisse-{0}.geojson");
//#endif
        }

        public static void Run2SpijkenisseCapacitated()
        {
            // SPIJKENISSE
            // build routerdb and save the result.
            var spijkenisse = Staging.RouterDbBuilder.Build("query4");
            var vehicle = spijkenisse.GetSupportedVehicle("car");
            var router = new Router(spijkenisse);

            // build problem.
            var locations = Staging.StagingHelpers.GetLocations(
                Staging.StagingHelpers.GetFeatureCollection("data.NoDepotCVRP.problem2-spijkenisse.geojson"));

            // build capacity.
            var max = 5400;
            var capacity = new Capacity()
            {
                Max = max,
                Constraints = new CapacityConstraint[]
                {
                    new CapacityConstraint()
                    {
                        Name = "weight",
                        Max = 3000,
                        Values = new float[locations.Length]
                    }
                }
            };
            for (var l = 0; l < locations.Length; l++)
            {
                capacity.Constraints[0].Values[l] = 100;
            }     

            // 
            var func = new Func<List<Route>>(() => router.CalculateNoDepotCVRP(vehicle.Fastest(), locations, capacity));
            var routes = func.TestPerf("No-Depot Capacitated VRP (Spijkenisse - Capacitated)");

//#if DEBUG
            routes.WriteGeoJson("spijkenisse-capacitated-{0}.geojson");
//#endif
        }

        public static void Run3DeHague()
        {
            // DE-HAGUE
            // build routerdb and save the result.
            var dehague = Staging.RouterDbBuilder.Build("query5");
            var vehicle = dehague.GetSupportedVehicle("car");
            var router = new Router(dehague);

            // build problem.
            var locations = Staging.StagingHelpers.GetLocations(
                Staging.StagingHelpers.GetFeatureCollection("data.NoDepotCVRP.problem3-de-hague.geojson"));        

            // 
            var max = 3800;
            var func = new Func<List<Route>>(() => router.CalculateNoDepotCVRP(vehicle.Fastest(), locations, max));
            var routes = func.TestPerf("No-Depot Capacitated VRP (De Haque)");

//#if DEBUG
            routes.WriteGeoJson("de-hague-{0}.geojson");
//#endif
        }

        public static void Run4Rotterdam()
        {
            // ROTTERDAM
            // build routerdb and save the result.
            var rotterdam = Staging.RouterDbBuilder.Build("query6");
            var vehicle = rotterdam.GetSupportedVehicle("car");
            var router = new Router(rotterdam);

            // build problem.
            var locations = Staging.StagingHelpers.GetLocations(
                Staging.StagingHelpers.GetFeatureCollection("data.NoDepotCVRP.problem4-rotterdam.geojson"));        

            // 
            var max = 4500;
            var func = new Func<List<Route>>(() => router.CalculateNoDepotCVRP(vehicle.Fastest(), locations, max));
            var routes = func.TestPerf("No-Depot Capacitated VRP (Rotterdam4)");

//#if DEBUG
            routes.WriteGeoJson("rotterdam4-{0}.geojson");
//#endif 
        }

        public static void Run5Rotterdam()
        {
            // ROTTERDAM
            // build routerdb and save the result.
            var rotterdam = Staging.RouterDbBuilder.Build("query7");
            var vehicle = rotterdam.GetSupportedVehicle("car");
            var router = new Router(rotterdam);

            // build problem.
            var locations = Staging.StagingHelpers.GetLocations(
                Staging.StagingHelpers.GetFeatureCollection("data.NoDepotCVRP.problem5-rotterdam.geojson"));        

            // 
            var max = 4500;
            var func = new Func<List<Route>>(() => router.CalculateNoDepotCVRP(vehicle.Fastest(), locations, max));
            var routes = func.TestPerf("No-Depot Capacitated VRP (Rotterdam5)");

//#if DEBUG
            routes.WriteGeoJson("rotterdam5-{0}.geojson");
//#endif 
        }
    }
}