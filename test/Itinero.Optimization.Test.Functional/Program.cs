// Itinero.Optimization - Route optimization for .NET
// Copyright (C) 2017 Abelshausen Ben
// 
// This file is part of Itinero.
// 
// Itinero is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// Itinero is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Itinero. If not, see <http://www.gnu.org/licenses/>.

using Itinero.LocalGeo;
using Itinero.Optimization.Algorithms.Random;
using NetTopologySuite.Features;
using System;
using System.IO;

namespace Itinero.Optimization.Test.Functional
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // enable logging.
            Itinero.Logging.Logger.LogAction = (origin, level, message, parameters) =>
            {
                Console.WriteLine(string.Format("[{0}] {1} - {2}", origin, level, message));
            };

            // STEP 0: staging, download and build a routerdb to test with.
            // download belgium.
            Staging.Download.DownloadBelgiumAll();

            // build routerdb and save the result.
            var routerDb = Staging.RouterDbBuilder.BuildBelgium();
            var router = new Router(routerDb);

            // define test locations.
            // ANTWERPEN-NOORD
            var locations = new Coordinate[]
            {
                new Coordinate(51.218644911468550f, 4.433176517486572f),
                new Coordinate(51.217878824149580f, 4.433691501617432f),
                new Coordinate(51.219142189396116f, 4.432237744331360f),
                new Coordinate(51.219955294434854f, 4.430692791938782f),
                new Coordinate(51.220788544202456f, 4.428203701972961f),
                new Coordinate(51.219904895779710f, 4.428101778030395f),
                new Coordinate(51.219558823524130f, 4.427565336227417f),
                new Coordinate(51.218577711336620f, 4.428439736366271f),
                new Coordinate(51.218234989137960f, 4.429646730422974f),
                new Coordinate(51.219431145724300f, 4.434539079666138f),
                new Coordinate(51.218786031426326f, 4.426513910293579f),
                new Coordinate(51.221211883064340f, 4.429571628570557f)
            };
            //// WECHELDERZANDE
            //var locations = new Coordinate[]
            //{
            //    new Coordinate(51.270453873703080f, 4.8008108139038080f),
            //    new Coordinate(51.264197451065370f, 4.8017120361328125f),
            //    new Coordinate(51.267446600889850f, 4.7830009460449220f),
            //    new Coordinate(51.260733228426076f, 4.7796106338500980f),
            //    new Coordinate(51.256489871317920f, 4.7884941101074220f),
            //    new Coordinate(51.270964016530680f, 4.7894811630249020f),
            //    new Coordinate(51.26216325894976f, 4.779932498931885f),
            //    new Coordinate(51.26579184564325f, 4.777781367301941f),
            //    new Coordinate(51.26855085674035f, 4.779181480407715f),
            //    new Coordinate(51.26906437701784f, 4.7879791259765625f),
            //    new Coordinate(51.259820134021695f, 4.7985148429870605f),
            //    new Coordinate(51.257040455587656f, 4.780147075653076f),
            //    new Coordinate(51.256248149311716f, 4.788386821746826f),
            //    new Coordinate(51.270054481615624f, 4.799646735191345f),
            //    new Coordinate(51.252984777835955f, 4.776681661605835f)
            //};

            // calculate TSP.
            var func = new Func<Route>(() => router.CalculateTSP(Itinero.Osm.Vehicles.Vehicle.Car.Fastest(), locations, 0, 0));
            var route = func.TestPerf("Testing TSP (0->0)");
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

            // calculate STSP
            func = new Func<Route>(() => router.CalculateSTSP(Itinero.Osm.Vehicles.Vehicle.Car.Fastest(), locations, 600, 0, 0));
            route = func.TestPerf("Testing STSP (0->0)");
            func = new Func<Route>(() => router.CalculateSTSP(Itinero.Osm.Vehicles.Vehicle.Car.Fastest(), locations, 600, 0, locations.Length - 1));
            route = func.TestPerf("Testing STSP (0->last)");
            func = new Func<Route>(() => router.CalculateSTSP(Itinero.Osm.Vehicles.Vehicle.Car.Fastest(), locations, 600, 0, null));
            route = func.TestPerf("Testing STSP (0->...)");

            // calculate directed TSP.
            func = new Func<Route>(() => router.CalculateSTSPDirected(Itinero.Osm.Vehicles.Vehicle.Car.Fastest(), locations, 60, 600, 0, 0));
            route = func.TestPerf("Testing Directed STSP (0->0)");
            func = new Func<Route>(() => router.CalculateSTSPDirected(Itinero.Osm.Vehicles.Vehicle.Car.Fastest(), locations, 60, 600, 0, locations.Length - 1));
            route = func.TestPerf("Testing Directed STSP (0->last)");
            func = new Func<Route>(() => router.CalculateSTSPDirected(Itinero.Osm.Vehicles.Vehicle.Car.Fastest(), locations, 60, 600, 0, null));
            route = func.TestPerf("Testing Directed STSP (0->...)");

            // define some time windows, all max.
            var windows = new TimeWindows.TimeWindow[]
            {
                TimeWindows.TimeWindow.Default,
                TimeWindows.TimeWindow.Default,
                TimeWindows.TimeWindow.Default,
                TimeWindows.TimeWindow.Default,
                TimeWindows.TimeWindow.Default,
                TimeWindows.TimeWindow.Default,
                TimeWindows.TimeWindow.Default,
                TimeWindows.TimeWindow.Default,
                TimeWindows.TimeWindow.Default,
                TimeWindows.TimeWindow.Default,
                TimeWindows.TimeWindow.Default,
                TimeWindows.TimeWindow.Default,
                TimeWindows.TimeWindow.Default,
                TimeWindows.TimeWindow.Default,
                TimeWindows.TimeWindow.Default
            };

            // calculate TSP-TW.
            func = new Func<Route>(() => router.CalculateTSPTW(Itinero.Osm.Vehicles.Vehicle.Car.Fastest(), locations, windows, 0, 0));
            route = func.TestPerf("Testing TSP-TW (0->0)");
            func = new Func<Route>(() => router.CalculateTSPTW(Itinero.Osm.Vehicles.Vehicle.Car.Fastest(), locations, windows, 0, locations.Length - 1));
            route = func.TestPerf("Testing TSP-TW (0->last)");
            func = new Func<Route>(() => router.CalculateTSPTW(Itinero.Osm.Vehicles.Vehicle.Car.Fastest(), locations, windows, 0, null));
            route = func.TestPerf("Testing TSP-TW (0->...)");

            // calculate directed TSP - TW.
            func = new Func<Route>(() => router.CalculateTSPTWDirected(Itinero.Osm.Vehicles.Vehicle.Car.Fastest(), locations, windows, 60, 0, 0));
            route = func.TestPerf("Testing Directed TSP-TW (0->0)");
            func = new Func<Route>(() => router.CalculateTSPTWDirected(Itinero.Osm.Vehicles.Vehicle.Car.Fastest(), locations, windows, 60, 0, locations.Length - 1));
            route = func.TestPerf("Testing Directed TSP-TW (0->last)");
            func = new Func<Route>(() => router.CalculateTSPTWDirected(Itinero.Osm.Vehicles.Vehicle.Car.Fastest(), locations, windows, 60, 0, null));
            route = func.TestPerf("Testing Directed TSP-TW (0->...)");
        }

        private static string ToJson(FeatureCollection featureCollection)
        {
            var jsonSerializer = new NetTopologySuite.IO.GeoJsonSerializer();
            var jsonStream = new StringWriter();
            jsonSerializer.Serialize(jsonStream, featureCollection);
            var json = jsonStream.ToInvariantString();
            return json;
        }
    }
}