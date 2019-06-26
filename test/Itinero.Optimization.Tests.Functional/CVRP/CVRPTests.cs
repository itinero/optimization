using System;
using System.Collections.Generic;
using System.IO;
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
            // Run1Wechelderzande();
            Run2Spijkenisse3600();
            Run2Spijkenisse5400();
            Run3DeHague();
            Run4Brussels();
        }

        public static void Run1Wechelderzande()
        {
            // WECHELDERZANDE - LILLE
            // build router db and save the result.
            var lille = Staging.RouterDbBuilder.Build("query3");
            var vehicle = lille.GetSupportedVehicle("car");
            var router = new Router(lille);

            // build problem.
            var locations = Staging.StagingHelpers.GetLocations(
                Staging.StagingHelpers.GetFeatureCollection("CVRP.data.problem1.geojson"));
            
            // build vehicle pool.
            var vehicles = VehiclePool.FromProfile(vehicle.Fastest(), 0, 0, 1000, 0, true);
            
            // run
            Func<Action<IEnumerable<Result<Route>>>, IEnumerable<Result<Route>>> func = (intermediateRoutesFunc) =>
                router.Optimize(vehicles, locations, out _, intermediateRoutesFunc);
            func.RunWithIntermediates("CVRP-" + "lille");
        }

        public static void Run2Spijkenisse3600()
        {
            // SPIJKENISSE
            // build router db and save the result.
            var spijkenisse = Staging.RouterDbBuilder.Build("query4");
            var vehicle = spijkenisse.GetSupportedVehicle("car");
            var router = new Router(spijkenisse);

            // build problem.
            const int max = 3600;
            var locations = Staging.StagingHelpers.GetLocations(
                Staging.StagingHelpers.GetFeatureCollection("CVRP.data.problem2-spijkenisse.geojson"));
            
            // build vehicle pool.
            var vehicles = VehiclePool.FromProfile(vehicle.Fastest(), 0, 0, max, 0, true);
            
            // run
            Func<Action<IEnumerable<Result<Route>>>, IEnumerable<Result<Route>>> func = (intermediateRoutesFunc) =>
                router.Optimize(vehicles, locations, out _, intermediateRoutesFunc);
            func.RunWithIntermediates("CVRP-spijkenisse-3600");
        }

        public static void Run2Spijkenisse5400()
        {
            // SPIJKENISSE
            // build router db and save the result.
            var spijkenisse = Staging.RouterDbBuilder.Build("query4");
            var vehicle = spijkenisse.GetSupportedVehicle("car");
            var router = new Router(spijkenisse);

            // build problem.
            const int max = 5400;
            var locations = Staging.StagingHelpers.GetLocations(
                Staging.StagingHelpers.GetFeatureCollection("CVRP.data.problem2-spijkenisse.geojson"));
            
            // build vehicle pool.
            var vehicles = VehiclePool.FromProfile(vehicle.Fastest(), 0, 0, max, 0, true);
            
            // run
            Func<Action<IEnumerable<Result<Route>>>, IEnumerable<Result<Route>>> func = (intermediateRoutesFunc) =>
                router.Optimize(vehicles, locations, out _, intermediateRoutesFunc);
            func.RunWithIntermediates("CVRP-spijkenisse-5400", 1);
        }

        public static void Run3DeHague()
        {
            // SPIJKENISSE
            // build router db and save the result.
            var spijkenisse = Staging.RouterDbBuilder.Build("query5");
            var vehicle = spijkenisse.GetSupportedVehicle("car");
            var router = new Router(spijkenisse);

            // build problem.
            const int max = 5400;
            var locations = Staging.StagingHelpers.GetLocations(
                Staging.StagingHelpers.GetFeatureCollection("CVRP.data.problem3-de-hague.geojson"));
            
            // build vehicle pool.
            var vehicles = VehiclePool.FromProfile(vehicle.Fastest(), 0, 0, max, 0, true);
            
            // run
            Func<Action<IEnumerable<Result<Route>>>, IEnumerable<Result<Route>>> func = (intermediateRoutesFunc) =>
                router.Optimize(vehicles, locations, out _, intermediateRoutesFunc);
            func.RunWithIntermediates("CVRP-de-hague");
        }

        public static void Run4Brussels()
        {
            // build router db and save the result.
            var routerDb = Staging.RouterDbBuilder.Build("query9");
            var vehicle = routerDb.GetSupportedVehicle("car");
            var router = new Router(routerDb);

            // build problem.
            const int max = 7200;
            var locations = Staging.StagingHelpers.GetLocations(
                Staging.StagingHelpers.GetFeatureCollection("CVRP.data.problem6-brussels.geojson"));
            
            // build vehicle pool.
            var vehicles = VehiclePool.FromProfile(vehicle.Fastest(), 0, 0, max, 0, true);
            
            // run
            Func<Action<IEnumerable<Result<Route>>>, IEnumerable<Result<Route>>> func = (intermediateRoutesFunc) =>
                router.Optimize(vehicles, locations, out _, intermediateRoutesFunc);
            func.RunWithIntermediates("CVRP-brussels");
        }
    }
}