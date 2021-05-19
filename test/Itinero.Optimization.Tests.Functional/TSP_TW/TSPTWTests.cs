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
            // RunProblem1();
            // RunProblem2();
            RunProblem3();
        }

        private static void RunProblem1()
        {
            // WECHELDERZANDE
            // build routerdb and save the result.
            var wechelderzande = Staging.RouterDbBuilder.Build("query1");
            var router = new Router(wechelderzande);

            // get test visits from geojson.
            var visits = Staging.StagingHelpers.GetVisits(
                Staging.StagingHelpers.GetFeatureCollection("TSP_TW.data.problem1.geojson"));

            var func = new Func<Action<IEnumerable<Result<Route>>>, IEnumerable<Result<Route>>>(
                (intermediateRoutesFunc) =>
                    router.Optimize(VehiclePool.FromProfile(router.Db.GetSupportedProfile("car"), 0, 0),
                        visits, out _, intermediateRoutesFunc));
            func.RunWithIntermediates("TSP-TW-" + "problem1-closed");
            func = new Func<Action<IEnumerable<Result<Route>>>, IEnumerable<Result<Route>>>(
                (intermediateRoutesFunc) =>
                    router.Optimize(VehiclePool.FromProfile(router.Db.GetSupportedProfile("car"), 0, visits.Length - 1),
                        visits, out _, intermediateRoutesFunc));
            func.RunWithIntermediates("TSP-TW-" + "problem1-fixed");
            func = new Func<Action<IEnumerable<Result<Route>>>, IEnumerable<Result<Route>>>(
                (intermediateRoutesFunc) =>
                    router.Optimize(VehiclePool.FromProfile(router.Db.GetSupportedProfile("car"), 0, null),
                        visits, out _, intermediateRoutesFunc));
            func.RunWithIntermediates("TSP-TW-" + "problem1-open");
        }

        private static void RunProblem2()
        {
            var wechelderzande = Staging.RouterDbBuilder.Build("query1");
            var router = new Router(wechelderzande);

            // get test visits from geojson.
            var visits = Staging.StagingHelpers.GetVisits(
                Staging.StagingHelpers.GetFeatureCollection("TSP_TW.data.problem2.geojson"));

            var func = new Func<Action<IEnumerable<Result<Route>>>, IEnumerable<Result<Route>>>(
                (intermediateRoutesFunc) =>
                    router.Optimize(VehiclePool.FromProfile(router.Db.GetSupportedProfile("car"), 0, 0),
                        visits, out _, intermediateRoutesFunc));
            func.RunWithIntermediates("TSP-TW-" + "problem2-closed");
            func = new Func<Action<IEnumerable<Result<Route>>>, IEnumerable<Result<Route>>>(
                (intermediateRoutesFunc) =>
                    router.Optimize(VehiclePool.FromProfile(router.Db.GetSupportedProfile("car"), 0, visits.Length - 1),
                        visits, out _, intermediateRoutesFunc));
            func.RunWithIntermediates("TSP-TW-" + "problem2-fixed");
            func = new Func<Action<IEnumerable<Result<Route>>>, IEnumerable<Result<Route>>>(
                (intermediateRoutesFunc) =>
                    router.Optimize(VehiclePool.FromProfile(router.Db.GetSupportedProfile("car"), 0, null),
                        visits, out _, intermediateRoutesFunc));
            func.RunWithIntermediates("TSP-TW-" + "problem2-open");
        }
        
        private static void RunProblem3()
        {
            var wechelderzande = Staging.RouterDbBuilder.Build("query1");
            var router = new Router(wechelderzande);

            // get test visits from geojson.
            var visits = Staging.StagingHelpers.GetVisits(
                Staging.StagingHelpers.GetFeatureCollection("TSP_TW.data.problem3.geojson"));

            var func = new Func<Action<IEnumerable<Result<Route>>>, IEnumerable<Result<Route>>>(
                (intermediateRoutesFunc) =>
                    router.Optimize(VehiclePool.FromProfile(router.Db.GetSupportedProfile("car"), 0, 0),
                        visits, out _, intermediateRoutesFunc));
            func.RunWithIntermediates("TSP-TW-" + "problem3-closed");
            func = new Func<Action<IEnumerable<Result<Route>>>, IEnumerable<Result<Route>>>(
                (intermediateRoutesFunc) =>
                    router.Optimize(VehiclePool.FromProfile(router.Db.GetSupportedProfile("car"), 0, visits.Length - 1),
                        visits, out _, intermediateRoutesFunc));
            func.RunWithIntermediates("TSP-TW-" + "problem3-fixed");
            func = new Func<Action<IEnumerable<Result<Route>>>, IEnumerable<Result<Route>>>(
                (intermediateRoutesFunc) =>
                    router.Optimize(VehiclePool.FromProfile(router.Db.GetSupportedProfile("car"), 0, null),
                        visits, out _, intermediateRoutesFunc));
            func.RunWithIntermediates("TSP-TW-" + "problem3-open");
        }
    }
}