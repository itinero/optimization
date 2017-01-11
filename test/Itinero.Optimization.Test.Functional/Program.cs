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
            var locations = new Coordinate[]
            {
                new Coordinate(51.270453873703080f, 4.8008108139038080f),
                new Coordinate(51.264197451065370f, 4.8017120361328125f),
                new Coordinate(51.267446600889850f, 4.7830009460449220f),
                new Coordinate(51.260733228426076f, 4.7796106338500980f),
                new Coordinate(51.256489871317920f, 4.7884941101074220f),
                new Coordinate(51.270964016530680f, 4.7894811630249020f),
                new Coordinate(51.26216325894976f, 4.779932498931885f),
                new Coordinate(51.26579184564325f, 4.777781367301941f),
                new Coordinate(51.26855085674035f, 4.779181480407715f),
                new Coordinate(51.26906437701784f, 4.7879791259765625f),
                new Coordinate(51.259820134021695f, 4.7985148429870605f),
                new Coordinate(51.257040455587656f, 4.780147075653076f),
                new Coordinate(51.256248149311716f, 4.788386821746826f),
                new Coordinate(51.270054481615624f, 4.799646735191345f),
                new Coordinate(51.252984777835955f,  4.776681661605835f)
            };

            // calculate TSP.
            var route = router.CalculateTSP(Itinero.Osm.Vehicles.Vehicle.Car.Fastest(), locations, 0, 0);
            route = router.CalculateTSP(Itinero.Osm.Vehicles.Vehicle.Car.Fastest(), locations, 0, locations.Length - 1);
            route = router.CalculateTSP(Itinero.Osm.Vehicles.Vehicle.Car.Fastest(), locations, 0, null);

            // calculate Directed TSP.
            route = router.CalculateTSPDirected(Itinero.Osm.Vehicles.Vehicle.Car.Fastest(), locations, 60, 0, 0);
            // TODO: fix or add a solver capable of handling this.
            route = router.CalculateTSPDirected(Itinero.Osm.Vehicles.Vehicle.Car.Fastest(), locations, 60, 0, locations.Length - 1);
            route = router.CalculateTSPDirected(Itinero.Osm.Vehicles.Vehicle.Car.Fastest(), locations, 60, 0, null);

            // calculate STSP
            route = router.CalculateSTSP(Itinero.Osm.Vehicles.Vehicle.Car.Fastest(), locations, 600, 0, 0);
            route = router.CalculateSTSP(Itinero.Osm.Vehicles.Vehicle.Car.Fastest(), locations, 600, 0, locations.Length - 1);
            route = router.CalculateSTSP(Itinero.Osm.Vehicles.Vehicle.Car.Fastest(), locations, 600, 0, null);

            // calculate directed TSP.
            route = router.CalculateSTSPDirected(Itinero.Osm.Vehicles.Vehicle.Car.Fastest(), locations, 60, 600, 0, 0);
            route = router.CalculateSTSPDirected(Itinero.Osm.Vehicles.Vehicle.Car.Fastest(), locations, 60, 600, 0, locations.Length - 1);
            route = router.CalculateSTSPDirected(Itinero.Osm.Vehicles.Vehicle.Car.Fastest(), locations, 60, 600, 0, null);

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
            route = router.CalculateTSPTW(Itinero.Osm.Vehicles.Vehicle.Car.Fastest(), locations, windows, 0, 0);
            route = router.CalculateTSPTW(Itinero.Osm.Vehicles.Vehicle.Car.Fastest(), locations, windows, 0, locations.Length - 1);
            route = router.CalculateTSPTW(Itinero.Osm.Vehicles.Vehicle.Car.Fastest(), locations, windows, 0, null);

            calculate directed TSP - TW.
            route = router.CalculateTSPTWDirected(Itinero.Osm.Vehicles.Vehicle.Car.Fastest(), locations, windows, 60, 0, 0);
            // TODO: fix or add a solver capable of handling this.
            route = router.CalculateTSPTWDirected(Itinero.Osm.Vehicles.Vehicle.Car.Fastest(), locations, windows, 60, 0, locations.Length - 1);
            route = router.CalculateTSPTWDirected(Itinero.Osm.Vehicles.Vehicle.Car.Fastest(), locations, windows, 60, 0, null);

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