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
