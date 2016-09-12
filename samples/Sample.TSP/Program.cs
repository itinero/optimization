// Itinero.Logistics - Route optimization for .NET
// Copyright (C) 2016 Abelshausen Ben
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

using Itinero;
using Itinero.LocalGeo;
using Itinero.Logistics.Routing.TSP;
using Itinero.Osm.Vehicles;
using System.Collections.Generic;
using System.IO;

namespace Sample.TSP
{
    class Program
    {
        static void Main(string[] args)
        {
            // download test-data (if not there yet).
            Download.ToFile("http://files.itinero.tech/data/itinero/routerdbs/planet/europe/belgium.c.cf.routerdb", "belgium.c.cf.routerdb").Wait();

            // load test-data into a router db and create router.
            var routerDb = RouterDb.Deserialize(File.OpenRead("belgium.c.cf.routerdb"));
            var router = new Router(routerDb);

            // calculate TSP along the given locations.
            var locations = new List<Coordinate>(new Coordinate[]
            {
                new Coordinate(51.270453873703080f, 4.8008108139038080f),
                new Coordinate(51.264197451065370f, 4.8017120361328125f),
                new Coordinate(51.267446600889850f, 4.7830009460449220f),
                new Coordinate(51.260733228426076f, 4.7796106338500980f),
                new Coordinate(51.256489871317920f, 4.7884941101074220f),
                new Coordinate(51.270964016530680f, 4.7894811630249020f)
            });
            var route = router.CalculateTSP(Vehicle.Car.Fastest(), locations.ToArray());
        }
    }
}
