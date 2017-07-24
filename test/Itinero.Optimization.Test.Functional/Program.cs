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

using Itinero.LocalGeo;
using Itinero.Optimization.Tours;
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
            // WECHELDERZANDE
            var locations = new Coordinate[]
            {
                new Coordinate(51.270453873703080f, 4.8008108139038080f),
                new Coordinate(51.264197451065370f, 4.8017120361328125f),
                new Coordinate(51.267446600889850f, 4.7830009460449220f),
                new Coordinate(51.260733228426076f, 4.7796106338500980f),
                new Coordinate(51.256489871317920f, 4.7884941101074220f),
                new Coordinate(4.7884941101074220f, 51.256489871317920f), // add another invalid location.
                new Coordinate(51.270964016530680f, 4.7894811630249020f),
                new Coordinate(51.26216325894976f, 4.779932498931885f),
                new Coordinate(51.26579184564325f, 4.777781367301941f),
                new Coordinate(4.779181480407715f, 51.26855085674035f), // add another invalid location.
                new Coordinate(51.26855085674035f, 4.779181480407715f),
                new Coordinate(51.26906437701784f, 4.7879791259765625f),
                new Coordinate(51.259820134021695f, 4.7985148429870605f),
                new Coordinate(51.257040455587656f, 4.780147075653076f),
                new Coordinate(51.256248149311716f, 4.788386821746826f),
                new Coordinate(51.270054481615624f, 4.799646735191345f),
                new Coordinate(51.252984777835955f, 4.776681661605835f)
            };

            var json = router.Db.GetGeoJsonAround(51.264197451065370f, 4.8017120361328125f, 1000);

            // calculate directed sequence.
            var func = new Func<Route>(() => router.CalculateDirected(Itinero.Osm.Vehicles.Vehicle.Car.Fastest(), locations, 60,
                new Tour(new int[] { 0, 1, 2, 3, 5, 4 }, 4)));
            var route = func.TestPerf("Testing Directed Sequence 1");
            func = new Func<Route>(() => router.CalculateDirected(Itinero.Osm.Vehicles.Vehicle.Car.Fastest(), locations, 60,
                 new Tour(new int[] { 0, 1, 2, 3, 4 }, 0)));
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
    }
}