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

using Itinero.IO.Osm;
using System;
using System.IO;

namespace Itinero.Optimization.Test.Functional.Staging
{
    /// <summary>
    /// Builds a routerdb.
    /// </summary>
    public static class RouterDbBuilder
    {
        public static string BelgiumRouterDbLocation = "belgium.routerdb";

        /// <summary>
        /// Builds a routerdb.
        /// </summary>
        /// <returns></returns>
        public static RouterDb BuildBelgium()
        {
            RouterDb routerDb = null;
            if (!File.Exists(Download.BelgiumLocal))
            {
                throw new Exception("No location belgium OSM file found!");
            }

            if (File.Exists(RouterDbBuilder.BelgiumRouterDbLocation))
            {
                try
                {
                    using (var stream = File.OpenRead(RouterDbBuilder.BelgiumRouterDbLocation))
                    {
                        routerDb = RouterDb.Deserialize(stream);
                    }
                }
                catch
                {
                    Itinero.Logging.Logger.Log("RouterDbBuilder", Itinero.Logging.TraceEventType.Warning, "Invalid existing RouterDb file, could not load file.");
                    routerDb = null;
                }
            }

            if (routerDb == null)
            {
                Itinero.Logging.Logger.Log("RouterDbBuilder", Itinero.Logging.TraceEventType.Information, "No existing RouterDb file found, creating now.");
                using (var stream = File.OpenRead(Download.BelgiumLocal))
                {
                    routerDb = new RouterDb();
                    routerDb.LoadOsmData(stream, Itinero.Osm.Vehicles.Vehicle.Car);
                }

                using (var stream = File.Open(RouterDbBuilder.BelgiumRouterDbLocation, FileMode.Create))
                {
                    routerDb.Serialize(stream);
                }
                Itinero.Logging.Logger.Log("RouterDbBuilder", Itinero.Logging.TraceEventType.Information, "RouterDb file created.");
            }

            if (routerDb != null)
            {
                if (!routerDb.HasContractedFor(Itinero.Osm.Vehicles.Vehicle.Car.Fastest()))
                {
                    Itinero.Logging.Logger.Log("RouterDbBuilder", Itinero.Logging.TraceEventType.Information, "No contracted graph found for the 'car' profile, building now...");
                    routerDb.AddContracted(Itinero.Osm.Vehicles.Vehicle.Car.Fastest());

                    using (var stream = File.Open(RouterDbBuilder.BelgiumRouterDbLocation, FileMode.Create))
                    {
                        routerDb.Serialize(stream);
                    }
                }
            }
            return routerDb;
        }
    }
}
