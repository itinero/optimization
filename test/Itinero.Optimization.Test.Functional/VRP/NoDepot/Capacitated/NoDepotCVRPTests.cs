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
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Itinero.Attributes;
using Itinero.Optimization.Models;
using Itinero.Optimization.Models.Costs;
using Itinero.Optimization.Models.Vehicles.Constraints;

namespace Itinero.Optimization.Test.Functional.VRP.NoDepot.Capacitated
{
    public static class NoDepotCVRPTests
    {
        private const string Name = "no-depot-";

        public static void Run()
        {
            Run1Wechelderzande();
            Run1WechelderzandeCapacitated();
            Run2Spijkenisse();
            Run2SpijkenisseCapacitated();
            Run2SpijkenisseVisitCosts();
            Run3DeHague();
            Run4Rotterdam();
            Run5Rotterdam();
            Run6Hoogeveen();
        }

        public static void Run1Wechelderzande()
        {
            // WECHELDERZANDE - LILLE
            // build routerdb and save the result.
            var lille = Staging.RouterDbBuilder.Build("query3");
            var vehicle = lille.GetSupportedVehicle("car");
            var router = new Router(lille);

            // build problem.
            var max = 1000;
            var locations = Staging.StagingHelpers.GetLocations(
                Staging.StagingHelpers.GetFeatureCollection("data.NoDepotCVRP.problem1.geojson"));
            
            // run things.
            Func<Action<Route[]>, List<Route>> func = (intermediateRoutesFunc) =>
                router.CalculateNoDepotCVRP(vehicle.Fastest(), locations, max, intermediateRoutesFunc);
            func.RunWithIntermedidates(NoDepotCVRPTests.Name + "lille");
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
            
            // run things.
            Func<Action<Route[]>, List<Route>> func = (intermediateRoutesFunc) =>
                router.CalculateNoDepotCVRP(vehicle.Fastest(), locations, capacityConstraint,
                    visitCosts, intermediateRoutesFunc);
            func.RunWithIntermedidates(NoDepotCVRPTests.Name + "lille-capacitated");
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
                Staging.StagingHelpers.GetFeatureCollection("data.NoDepotCVRP.problem2-spijkenisse.geojson"));        
            
            // run things.
            Func<Action<Route[]>, List<Route>> func = (intermediateRoutesFunc) =>
                router.CalculateNoDepotCVRP(vehicle.Fastest(), locations, max, intermediateRoutesFunc);
            func.RunWithIntermedidates(NoDepotCVRPTests.Name + "spijkenisse");
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

            // build capacity constraints
            var capacityConstraint = new CapacityConstraint[]
            {
                new CapacityConstraint()
                {
                    Name = Itinero.Optimization.Models.Metrics.Time,
                    Capacity = 3600
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
            
            // run things.
            Func<Action<Route[]>, List<Route>> func = (intermediateRoutesFunc) =>
                router.CalculateNoDepotCVRP(vehicle.Fastest(), locations, 
                    capacityConstraint, visitCosts, intermediateRoutesFunc);
            func.RunWithIntermedidates(NoDepotCVRPTests.Name + "spijkenisse-capacitated");
        }

        public static void Run2SpijkenisseVisitCosts()
        {
            // SPIJKENISSE
            // build routerdb and save the result.
            var spijkenisse = Staging.RouterDbBuilder.Build("query4");
            var vehicle = spijkenisse.GetSupportedVehicle("car");
            var router = new Router(spijkenisse);

            // build problem.
            var locations = Staging.StagingHelpers.GetLocations(
                Staging.StagingHelpers.GetFeatureCollection("data.NoDepotCVRP.problem2-spijkenisse.geojson"));

            // build capacity constraints
            var capacityConstraint = new CapacityConstraint[]
            {
                new CapacityConstraint()
                {
                    Name = Itinero.Optimization.Models.Metrics.Time,
                    Capacity = 3600 * 7
                },
                new CapacityConstraint()
                {
                    Name = Itinero.Optimization.Models.Metrics.Weight,
                    Capacity = 25000
                }
            };

            // build visit costs.
            var visitCostTime = new VisitCosts()
            {
                Name = Itinero.Optimization.Models.Metrics.Time,
                Costs = new float[locations.Length]
            };
            for (var l = 0; l < locations.Length; l++)
            {
                visitCostTime.Costs[l] = 180;
            }
            var visitCostWeight = new VisitCosts()
            {
                Name = Itinero.Optimization.Models.Metrics.Weight,
                Costs = new float[locations.Length]
            };
            for (var l = 0; l < locations.Length; l++)
            {
                visitCostWeight.Costs[l] = 400;
            }
            var visitCosts = new VisitCosts[]
            {
                visitCostTime,
                visitCostWeight
            };
            
            // run things.
            Func<Action<Route[]>, List<Route>> func = (intermediateRoutesFunc) =>
                router.CalculateNoDepotCVRP(vehicle.Fastest(), locations, 
                    capacityConstraint, visitCosts, intermediateRoutesFunc);
            func.RunWithIntermedidates(NoDepotCVRPTests.Name + "spijkenisse-visitcost");
        }

        public static void Run3DeHague()
        {
            // DE-HAGUE
            // build routerdb and save the result.
            var dehague = Staging.RouterDbBuilder.Build("query5");
            var vehicle = dehague.GetSupportedVehicle("car");
            var router = new Router(dehague);

            // build problem.
            var max = 3800;
            var locations = Staging.StagingHelpers.GetLocations(
                Staging.StagingHelpers.GetFeatureCollection("data.NoDepotCVRP.problem3-de-hague.geojson"));   
            
            // run things.
            Func<Action<Route[]>, List<Route>> func = (intermediateRoutesFunc) =>
                router.CalculateNoDepotCVRP(vehicle.Fastest(), locations, max, intermediateRoutesFunc);
            func.RunWithIntermedidates(NoDepotCVRPTests.Name + "de-hague");
        }

        public static void Run4Rotterdam()
        {
            // ROTTERDAM
            // build routerdb and save the result.
            var rotterdam = Staging.RouterDbBuilder.Build("query6");
            var vehicle = rotterdam.GetSupportedVehicle("car");
            var router = new Router(rotterdam);

            // build problem.
            var max = 4500;
            var locations = Staging.StagingHelpers.GetLocations(
                Staging.StagingHelpers.GetFeatureCollection("data.NoDepotCVRP.problem4-rotterdam.geojson"));        
            
            // run things.
            Func<Action<Route[]>, List<Route>> func = (intermediateRoutesFunc) =>
                router.CalculateNoDepotCVRP(vehicle.Fastest(), locations, max, intermediateRoutesFunc);
            func.RunWithIntermedidates(NoDepotCVRPTests.Name + "rotterdam4");
        }

        public static void Run5Rotterdam()
        {
            // ROTTERDAM
            // build routerdb and save the result.
            var rotterdam = Staging.RouterDbBuilder.Build("query7");
            var vehicle = rotterdam.GetSupportedVehicle("car");
            var router = new Router(rotterdam);

            // build problem.
            var max = 4500;
            var locations = Staging.StagingHelpers.GetLocations(
                Staging.StagingHelpers.GetFeatureCollection("data.NoDepotCVRP.problem5-rotterdam.geojson"));        
            
            // run things.
            Func<Action<Route[]>, List<Route>> func = (intermediateRoutesFunc) =>
                router.CalculateNoDepotCVRP(vehicle.Fastest(), locations, max, intermediateRoutesFunc);
            func.RunWithIntermedidates(NoDepotCVRPTests.Name + "rotterdam5");
        }

        public static void Run6Hoogeveen()
        {
            // HOOGEVEEN
            // build routerdb and save the result.
            var hoogeveen = Staging.RouterDbBuilder.Build("query8");
            var vehicle = hoogeveen.GetSupportedVehicle("car");
            var router = new Router(hoogeveen);

            // build problem.
            var max = 4500;
            var locations = Staging.StagingHelpers.GetLocations(
                Staging.StagingHelpers.GetFeatureCollection("data.NoDepotCVRP.problem6-hogeveen.geojson"));        
            
            // run things.
            Func<Action<Route[]>, List<Route>> func = (intermediateRoutesFunc) =>
                router.CalculateNoDepotCVRP(vehicle.Fastest(), locations, max, intermediateRoutesFunc);
            func.RunWithIntermedidates(NoDepotCVRPTests.Name + "hogeveen");
        }
    }
}