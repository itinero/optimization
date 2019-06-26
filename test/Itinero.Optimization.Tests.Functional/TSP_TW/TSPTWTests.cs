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

namespace Itinero.Optimization.Tests.Functional.TSP_TW
{
    public static class TSPTWTests
    {
        /// <summary>
        /// Runs some functional tests related to the TSP-TW.
        /// </summary>
        public static void Run()
        {
            // WECHELDERZANDE
            // build routerdb and save the result.
            var wechelderzande = Staging.RouterDbBuilder.Build("query1");
            var router = new Router(wechelderzande);

            // get test visits from geojson.
            var visits = Staging.StagingHelpers.GetVisits(
                Staging.StagingHelpers.GetFeatureCollection("TSP_TW.data.problem1.geojson"));
            
            // build model & run.
            // closed 0-> ... -> 0
            var model = new Model()
            {
                VehiclePool = VehiclePool.FromProfile(router.Db.GetSupportedProfile("car"), 0, 0),
                Visits = visits
            };
            Func<Action<IEnumerable<Result<Route>>>, IEnumerable<Result<Route>>> func = (intermediateRoutesFunc) =>
                router.Optimize(model, out _, intermediateRoutesFunc);
            func.RunWithIntermediates("TSP-TW-" + "wechel-closed");
            // closed fixed 0 -> ... -> last.
            model = new Model()
            {
                VehiclePool = VehiclePool.FromProfile(router.Db.GetSupportedProfile("car"), 0, visits.Length - 1),
                Visits = visits
            };
            func = (intermediateRoutesFunc) =>
                router.Optimize(model, out _, intermediateRoutesFunc);
            func.RunWithIntermediates("TSP-TW-" + "wechel-fixed");
            // open 0 -> ...
            model = new Model()
            {
                VehiclePool = VehiclePool.FromProfile(router.Db.GetSupportedProfile("car"), 0, null),
                Visits = visits
            };
            func = (intermediateRoutesFunc) =>
                router.Optimize(model, out _, intermediateRoutesFunc);
            func.RunWithIntermediates("TSP-TW-" + "wechel-open");    
        }
    }
}