using System;
using System.Collections.Generic;
using Itinero.Optimization.Models;
using Itinero.Optimization.Models.Vehicles;

namespace Itinero.Optimization.Tests.Functional.TSP_TW_D
{
    public static class TSPTWDTests
    {
        /// <summary>
        /// Runs some functional tests related to the TSP-TW.
        /// </summary>
        public static void Run()
        {
            //RunProblem1();
            RunProblem2();
        }

        private static void RunProblem1()
        {
            // WECHELDERZANDE
            // build routerdb and save the result.
            var wechelderzande = Staging.RouterDbBuilder.Build("query1");
            var router = new Router(wechelderzande);

            // get test visits from geojson.
            var visits = Staging.StagingHelpers.GetVisits(
                Staging.StagingHelpers.GetFeatureCollection("TSP_TW_D.data.problem1.geojson"));

            var func = new Func<Action<IEnumerable<Result<Route>>>, IEnumerable<Result<Route>>>(
                (intermediateRoutesFunc) =>
                    router.Optimize(VehiclePool.FromProfile(router.Db.GetSupportedProfile("car"), 0, 0, turnPenalty: 60),
                        visits, out _, intermediateRoutesFunc));
            func.RunWithIntermediates("TSP-TW-D-" + "problem1-closed");
            func = new Func<Action<IEnumerable<Result<Route>>>, IEnumerable<Result<Route>>>(
                (intermediateRoutesFunc) =>
                    router.Optimize(VehiclePool.FromProfile(router.Db.GetSupportedProfile("car"), 0, visits.Length - 1, turnPenalty: 60),
                        visits, out _, intermediateRoutesFunc));
            func.RunWithIntermediates("TSP-TW-D-" + "problem1-fixed");
            func = new Func<Action<IEnumerable<Result<Route>>>, IEnumerable<Result<Route>>>(
                (intermediateRoutesFunc) =>
                    router.Optimize(VehiclePool.FromProfile(router.Db.GetSupportedProfile("car"), 0, null, turnPenalty: 60),
                        visits, out _, intermediateRoutesFunc));
            func.RunWithIntermediates("TSP-TW-D-" + "problem1-open");
        }

        private static void RunProblem2()
        {
            // WECHELDERZANDE
            // build routerdb and save the result.
            var wechelderzande = Staging.RouterDbBuilder.Build("query1");
            var router = new Router(wechelderzande);

            // get test visits from geojson.
            var visits = Staging.StagingHelpers.GetVisits(
                Staging.StagingHelpers.GetFeatureCollection("TSP_TW_D.data.problem2.geojson"));

            var func = new Func<Action<IEnumerable<Result<Route>>>, IEnumerable<Result<Route>>>(
                (intermediateRoutesFunc) =>
                    router.Optimize(VehiclePool.FromProfile(router.Db.GetSupportedProfile("car"), 0, 0, turnPenalty: 60),
                        visits, out _, intermediateRoutesFunc));
            func.RunWithIntermediates("TSP-TW-D-" + "problem2-closed");
            func = new Func<Action<IEnumerable<Result<Route>>>, IEnumerable<Result<Route>>>(
                (intermediateRoutesFunc) =>
                    router.Optimize(VehiclePool.FromProfile(router.Db.GetSupportedProfile("car"), 0, visits.Length - 1, turnPenalty: 60),
                        visits, out _, intermediateRoutesFunc));
            func.RunWithIntermediates("TSP-TW-D-" + "problem2-fixed");
            func = new Func<Action<IEnumerable<Result<Route>>>, IEnumerable<Result<Route>>>(
                (intermediateRoutesFunc) =>
                    router.Optimize(VehiclePool.FromProfile(router.Db.GetSupportedProfile("car"), 0, null, turnPenalty: 60),
                        visits, out _, intermediateRoutesFunc));
            func.RunWithIntermediates("TSP-TW-D-" + "problem2-open");
        }
    }
}