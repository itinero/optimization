using System;
using System.Collections.Generic;
using System.Linq;
using Itinero.Optimization.Models.Vehicles;

namespace Itinero.Optimization.Tests.Functional.CVRP
{
    public static class CVRPTests
    {
        /// <summary>
        /// Runs some functional tests related to the CVRP.
        /// </summary>
        public static void Run()
        {
            Run1Wechelderzande();
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
                Staging.StagingHelpers.GetFeatureCollection("CVRP.data.problem1.geojson"));
            
            // build vehicle pool.
            var vehicles = VehiclePool.FromProfile(vehicle.Fastest(), 0, 0, 1000, 0, true);
            
            // run
            Func<Action<IEnumerable<Result<Route>>>, IEnumerable<Result<Route>>> func = (intermediateRoutesFunc) =>
                router.Optimize(vehicles, locations, out _, intermediateRoutesFunc);
            func.RunWithIntermedidates("CVRP-" + "lille");
        }

        public static void Run2Spijkenisse()
        {
            // SPIJKENISSE
            // build routerdb and save the result.
            var spijkenisse = Staging.RouterDbBuilder.Build("query4");
            var vehicle = spijkenisse.GetSupportedVehicle("car");
            var router = new Router(spijkenisse);

            // build problem2.
            const int max = 5400;
            var locations = Staging.StagingHelpers.GetLocations(
                Staging.StagingHelpers.GetFeatureCollection("CVRP.data.problem2-spijkenisse.geojson"));
            
            // build vehicle pool.
            var vehicles = VehiclePool.FromProfile(vehicle.Fastest(), 0, 0, max, 0, true);
            
            // run
            Func<Action<IEnumerable<Result<Route>>>, IEnumerable<Result<Route>>> func = (intermediateRoutesFunc) =>
                router.Optimize(vehicles, locations, out _, intermediateRoutesFunc);
            func.RunWithIntermedidates("CVRP-spijkenisse");
        }
    }
}