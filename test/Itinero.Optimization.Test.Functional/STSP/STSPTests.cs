using System;
using Itinero.LocalGeo;

namespace Itinero.Optimization.Test.Functional.STSP
{
    public static class STSPTests
    {
        /// <summary>
        /// Runs some functional tests related to the STSP.
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

            // calculate STSP
            var func = new Func<Route>(() => router.CalculateSTSP(Itinero.Osm.Vehicles.Vehicle.Car.Fastest(), locations, 600, 0, 0));
            var route = func.TestPerf("Testing STSP (0->0)");
            func = new Func<Route>(() => router.CalculateSTSP(Itinero.Osm.Vehicles.Vehicle.Car.Fastest(), locations, 600, 0, locations.Length - 1));
            route = func.TestPerf("Testing STSP (0->last)");
            func = new Func<Route>(() => router.CalculateSTSP(Itinero.Osm.Vehicles.Vehicle.Car.Fastest(), locations, 600, 0, null));
            route = func.TestPerf("Testing STSP (0->...)");

            // calculate directed STSP.
            func = new Func<Route>(() => router.CalculateSTSPDirected(Itinero.Osm.Vehicles.Vehicle.Car.Fastest(), locations, 60, 600, 0, 0));
            route = func.TestPerf("Testing Directed STSP (0->0)");
            func = new Func<Route>(() => router.CalculateSTSPDirected(Itinero.Osm.Vehicles.Vehicle.Car.Fastest(), locations, 60, 600, 0, locations.Length - 1));
            route = func.TestPerf("Testing Directed STSP (0->last)");
            func = new Func<Route>(() => router.CalculateSTSPDirected(Itinero.Osm.Vehicles.Vehicle.Car.Fastest(), locations, 60, 600, 0, null));
            route = func.TestPerf("Testing Directed STSP (0->...)"); 
        }
    }
}