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
using System.Linq;
using System.IO;
using Itinero.Profiles;
using OsmSharp.Streams;

namespace Itinero.Optimization.Test.Functional.Staging
{
    /// <summary>
    /// Builds a routerdb.
    /// </summary>
    public static class RouterDbBuilder
    {
        /// <summary>
        /// Builds a routerdb.
        /// </summary>
        /// <returns></returns>
        public static RouterDb Build(string queryName)
        {
            return Build(queryName, Itinero.Osm.Vehicles.Vehicle.Car);
        }

        /// <summary>
        /// Builds a routerdb.
        /// </summary>
        /// <returns></returns>
        public static RouterDb Build(string queryName, Vehicle vehicle)
        {
            RouterDb routerDb = null;
            var routerDbFileName = queryName + ".routerdb";
            if (File.Exists(routerDbFileName))
            {
                try
                {
                    using (var stream = File.OpenRead(routerDbFileName))
                    {
                        routerDb = RouterDb.Deserialize(stream);
                    }
                }
                catch
                {
                    routerDb = null;
                }
            }

            if (routerDb == null)
            {
                // check if OSM pbf file is there.
                var fileName = Path.Combine("Staging", queryName + ".osm.pbf");
                if (!File.Exists(fileName))
                { // check if OSM file is there, otherwise attempt to download from overpass.
                    fileName = Path.Combine("Staging", queryName + ".osm");
                    if (!File.Exists(fileName))
                    {
                        // make sure source OSM data is there.
                        Download.ToFile(queryName);
                    } 
                }

                // build routerdb.
                Itinero.Logging.Logger.Log("RouterDbBuilder", Itinero.Logging.TraceEventType.Information, "No existing RouterDb file found, creating now.");
                using (var stream = File.OpenRead(fileName))
                {
                    OsmStreamSource osmStream = null;
                    if (fileName.EndsWith(".osm.pbf"))
                    {
                        osmStream = new OsmSharp.Streams.PBFOsmStreamSource(stream);
                    }
                    else
                    {
                        osmStream = new OsmSharp.Streams.XmlOsmStreamSource(stream);
                    }
                    var sortedData = osmStream.ToList();
                    sortedData.Sort((x, y) =>
                    {
                        if (x.Type == y.Type)
                        {
                            return x.Id.Value.CompareTo(y.Id.Value);
                        }
                        if (x.Type == OsmSharp.OsmGeoType.Node)
                        {
                            return -1;
                        }
                        else if (x.Type == OsmSharp.OsmGeoType.Way)
                        {
                            if (y.Type == OsmSharp.OsmGeoType.Node)
                            {
                                return 1;
                            }
                            return -1;
                        }
                        return 1;
                    });
                    
                    routerDb = new RouterDb();
                    routerDb.LoadOsmData(sortedData, vehicle);
                }
                
                Itinero.Logging.Logger.Log("RouterDbBuilder", Itinero.Logging.TraceEventType.Information, "RouterDb file created.");

                if (!routerDb.HasContractedFor(vehicle.Fastest()))
                {
                    Itinero.Logging.Logger.Log("RouterDbBuilder", Itinero.Logging.TraceEventType.Information, "No contracted graph found for the 'car' profile, building now...");
                    routerDb.AddContracted(vehicle.Fastest(), true);
                }

                using (var stream = File.Open(routerDbFileName, FileMode.Create))
                {
                    routerDb.Serialize(stream);
                }
            }
            return routerDb;
        }
    }
}