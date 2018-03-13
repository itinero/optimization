using System;
using Itinero.LocalGeo;
using Itinero.Optimization.Abstract.Tours;

namespace Itinero.Optimization.Test.Functional.TSP
{
    public static class TSPTests
    {
        /// <summary>
        /// Runs some functional tests related to the TSP.
        /// </summary>
        public static void Run()
        {
            // WECHELDERZANDE
            // build routerdb and save the result.
            var wechelderzande = Staging.RouterDbBuilder.Build("query1");
            var router = new Router(wechelderzande);

            // define test locations.
            var locations = new Coordinate[]
            {
                new Coordinate(51.270453873703080f, 4.8008108139038080f),
                new Coordinate(51.264197451065370f, 4.8017120361328125f),
                new Coordinate(51.267446600889850f, 4.7830009460449220f),
                new Coordinate(51.260733228426076f, 4.7796106338500980f),
                new Coordinate(51.256489871317920f, 4.7884941101074220f),
                new Coordinate(4.7884941101074220f, 51.256489871317920f), // non-resolvable location.
                new Coordinate(51.270964016530680f, 4.7894811630249020f),
                new Coordinate(51.26216325894976f, 4.779932498931885f),
                new Coordinate(51.26579184564325f, 4.777781367301941f),
                new Coordinate(4.779181480407715f, 51.26855085674035f), // non-resolvable location.
                new Coordinate(51.26855085674035f, 4.779181480407715f),
                new Coordinate(51.26906437701784f, 4.7879791259765625f),
                new Coordinate(51.259820134021695f, 4.7985148429870605f),
                new Coordinate(51.257040455587656f, 4.780147075653076f),
                new Coordinate(51.299771179035815f, 4.7829365730285645f), // non-routable location.
                new Coordinate(51.256248149311716f, 4.788386821746826f),
                new Coordinate(51.270054481615624f, 4.799646735191345f),
                new Coordinate(51.252984777835955f, 4.776681661605835f)
            };

            // calculate directed sequence.
            var func = new Func<Route>(() => router.CalculateDirected(Itinero.Osm.Vehicles.Vehicle.Car.Fastest(), locations, 60,
                new Tour(new int[] { 0, 1, 2, 14, 3, 5, 4 }, 4)));
            var route = func.TestPerf("Testing Directed Sequence 1");
            func = new Func<Route>(() => router.CalculateDirected(Itinero.Osm.Vehicles.Vehicle.Car.Fastest(), locations, 60,
                 new Tour(new int[] { 0, 1, 2, 14, 3, 4 }, 0)));
            route = func.TestPerf("Testing Directed Sequence 2");

            // calculate TSP.
            func = new Func<Route>(() => router.CalculateTSP(Itinero.Osm.Vehicles.Vehicle.Car.Fastest(), locations, 0, 0));
            route = func.TestPerf("Testing TSP (0->0)");
            func = new Func<Route>(() => router.CalculateTSP(Itinero.Osm.Vehicles.Vehicle.Car.Fastest(), locations, 0, locations.Length - 1));
            route = func.TestPerf("Testing TSP (0->last)");
            func = new Func<Route>(() => router.CalculateTSP(Itinero.Osm.Vehicles.Vehicle.Car.Fastest(), locations, 0, null));
            route = func.TestPerf("Testing TSP (0->...)");

            // calculate Directed TSP.
            func = new Func<Route>(() => router.CalculateTSPDirected(Itinero.Osm.Vehicles.Vehicle.Car.Fastest(), locations, 60, 0, 0));
            route = func.TestPerf("Testing Directed TSP (0->0)");
            func = new Func<Route>(() => router.CalculateTSPDirected(Itinero.Osm.Vehicles.Vehicle.Car.Fastest(), locations, 60, 0, locations.Length - 1));
            route = func.TestPerf("Testing Directed TSP (0->last)");
            func = new Func<Route>(() => router.CalculateTSPDirected(Itinero.Osm.Vehicles.Vehicle.Car.Fastest(), locations, 60, 0, null));
            route = func.TestPerf("Testing Directed TSP (0->...)");     

            // HENGELO
            // build routerdb and save the result.
            var hengelo = Staging.RouterDbBuilder.Build("query2");
            var vehicle = hengelo.GetSupportedVehicle("car");
            router = new Router(hengelo);

            locations = new Coordinate[]
            {
                new Coordinate(52.286143714914566f, 6.776547431945801f),
                new Coordinate(52.2790550797244f, 6.791224479675293f),
                new Coordinate(52.270363390765176f, 6.796717643737792f),
                new Coordinate(52.265294640450435f, 6.777276992797852f),
                new Coordinate(52.25807132666112f, 6.78380012512207f),
                new Coordinate(52.25851789290645f, 6.7974042892456055f),
                new Coordinate(52.26810484821601f, 6.8122100830078125f),
                new Coordinate(52.261433597272266f, 6.819033622741699f),
                new Coordinate(52.25318507282242f, 6.798820495605469f),
                new Coordinate(52.24934924915677f, 6.809506416320801f),
                new Coordinate(52.2905013499346f, 6.760196685791016f)
            };

            // calculate Directed TSP.
            func = new Func<Route>(() => router.CalculateTSPDirected(vehicle.Fastest(), locations, 60, 0, 0));
            route = func.TestPerf("Testing Directed TSP Hengelo (0->0)");
            func = new Func<Route>(() => router.CalculateTSPDirected(vehicle.Fastest(), locations, 60, 0, locations.Length - 1));
            route = func.TestPerf("Testing Directed TSP Hengelo (0->last)");
            func = new Func<Route>(() => router.CalculateTSPDirected(vehicle.Fastest(), locations, 60, 0, null));
            route = func.TestPerf("Testing Directed TSP Hengelo (0->...)");      
        }
    }
}