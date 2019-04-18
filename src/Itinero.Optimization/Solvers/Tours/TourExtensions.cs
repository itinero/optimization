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

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Itinero.IO.Json;
using Itinero.LocalGeo;

namespace Itinero.Optimization.Solvers.Tours
{
    /// <summary>
    /// Contains tour extensions.
    /// </summary>
    public static class TourExtensions
    {
        /// <summary>
        /// Puts the elements of the enumerator (back) in a list.
        /// </summary>
        /// <returns></returns>
        public static List<T> ToList<T>(this IEnumerator<T> enumerator)
        {
            var list = new List<T>();
            while (enumerator.MoveNext())
            {
                list.Add(enumerator.Current);
            }

            return list;
        }

        /// <summary>
        /// Calculates the total weight.
        /// </summary>
        /// <param name="tour">The tour.</param>
        /// <param name="weightsFunc">The weight function.</param>
        /// <returns>The total weight.</returns>
        internal static float Weight(this Tour tour, Func<int, int, float> weightsFunc)
        {
            var weight = 0f;
            var previous = Tour.NOT_SET;
            var first = Tour.NOT_SET;
            foreach (var visit in tour)
            {
                if (previous == Tour.NOT_SET)
                {
                    first = visit;
                    previous = visit;
                    continue;
                }

                weight += weightsFunc(previous, visit);
                previous = visit;
            }

            var closed = tour.IsClosed();
            if (closed &&
                first != Tour.NOT_SET)
            {
                weight += weightsFunc(previous, first);
            }

            return weight;
        }

        /// <summary>
        /// Calculates the total weight.
        /// </summary>
        /// <param name="tour">The tour.</param>
        /// <param name="weightsFunc">The weight function.</param>
        /// <param name="visitCount">The number of visits found.</param>
        /// <returns>The total weight.</returns>
        internal static float Weight(this Tour tour, Func<int, int, float> weightsFunc, out int visitCount)
        {
            visitCount = 0;
            var weight = 0f;
            var previous = Tour.NOT_SET;
            var first = Tour.NOT_SET;
            foreach (var visit in tour)
            {
                visitCount++;
                if (previous == Tour.NOT_SET)
                {
                    first = visit;
                    previous = visit;
                    continue;
                }

                weight += weightsFunc(previous, visit);
                previous = visit;
            }

            var closed = tour.IsClosed();
            if (closed &&
                first != Tour.NOT_SET)
            {
                weight += weightsFunc(previous, first);
            }

            return weight;
        }

        /// <summary>
        /// Enumerates all visits starting at the given vist.
        /// </summary>
        public static IEnumerable<int> From(this Tour tour, int visit)
        {
            var started = false;
            foreach (var v in tour)
            {
                if (!started)
                {
                    if (v != visit)
                    {
                        continue;
                    }

                    started = true;
                }

                yield return v;
            }

            foreach (var v in tour)
            {
                if (v == visit)
                {
                    break;
                }

                yield return v;
            }
        }

        /// <summary>
        /// Returns a geojson linestring representing the given tour.
        /// </summary>
        /// <param name="tour">The tour.</param>
        /// <param name="locationFunc">The location function.</param>
        /// <param name="description">The description for the tour if any.</param>
        /// <returns>A geojson feature collection with one linestring.</returns>
        public static string ToGeoJson(this Tour tour, Func<int, Coordinate> locationFunc, string description = null)
        {
            var stringWriter = new StringWriter();
            var jsonWriter = new JsonWriter(stringWriter);
            jsonWriter.WriteOpen();
            jsonWriter.WriteProperty("type", "FeatureCollection", true, false);
            jsonWriter.WritePropertyName("features", false);
            jsonWriter.WriteArrayOpen();

            // write linestring.
            if (tour.Count > 1)
            {
                jsonWriter.WriteOpen();
                jsonWriter.WriteProperty("type", "Feature", true, false);
                jsonWriter.WriteProperty("name", "ShapeMeta", true, false);
                jsonWriter.WritePropertyName("geometry", false);

                jsonWriter.WriteOpen();
                jsonWriter.WriteProperty("type", "LineString", true, false);
                jsonWriter.WritePropertyName("coordinates", false);
                jsonWriter.WriteArrayOpen();

                foreach (var visit in tour)
                {
                    var coordinate = locationFunc(visit);

                    jsonWriter.WriteArrayOpen();
                    jsonWriter.WriteArrayValue(coordinate.Longitude.ToInvariantString());
                    jsonWriter.WriteArrayValue(coordinate.Latitude.ToInvariantString());
                    if (coordinate.Elevation.HasValue)
                    {
                        jsonWriter.WriteArrayValue(coordinate.Elevation.Value.ToInvariantString());
                    }

                    jsonWriter.WriteArrayClose();
                }

                jsonWriter.WriteArrayClose();
                jsonWriter.WriteClose();

                jsonWriter.WritePropertyName("properties");
                jsonWriter.WriteOpen();

                if (description != null)
                {
                    jsonWriter.WriteProperty("tour", description, true, true);
                }

                jsonWriter.WriteClose();

                jsonWriter.WriteClose();
            }

            // write points per visit.
            var i = 0;
            foreach (var visit in tour)
            {
                var location = locationFunc(visit);

                jsonWriter.WriteOpen();
                jsonWriter.WriteProperty("type", "Feature", true, false);
                jsonWriter.WritePropertyName("geometry", false);

                jsonWriter.WriteOpen();
                jsonWriter.WriteProperty("type", "Point", true, false);
                jsonWriter.WritePropertyName("coordinates", false);
                jsonWriter.WriteArrayOpen();
                jsonWriter.WriteArrayValue(location.Longitude.ToInvariantString());
                jsonWriter.WriteArrayValue(location.Latitude.ToInvariantString());
                if (location.Elevation.HasValue)
                {
                    jsonWriter.WriteArrayValue(location.Elevation.Value.ToInvariantString());
                }

                jsonWriter.WriteArrayClose();
                jsonWriter.WriteClose();

                jsonWriter.WritePropertyName("properties");
                jsonWriter.WriteOpen();

                jsonWriter.WriteProperty("visit", visit.ToString(), false, false);
                jsonWriter.WriteProperty("index", i.ToString(), false, false);
                if (description != null)
                {
                    jsonWriter.WriteProperty("tour", description, true, true);
                }

                jsonWriter.WriteClose();

                jsonWriter.WriteClose();
                i++;
            }

            // write feature array close and feature collection close.
            jsonWriter.WriteArrayClose();
            jsonWriter.WriteClose();

            return stringWriter.ToInvariantString();
        }
    }
}