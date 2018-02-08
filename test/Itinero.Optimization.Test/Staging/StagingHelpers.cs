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
 
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Itinero.Optimization.Tours;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;

namespace Itinero.Optimization.Test.Staging
{
    /// <summary>
    /// Some helper methods to setup test-problems.
    /// </summary>
    public static class StagingHelpers
    {
        public static string EmbeddedResourceRoot = "Itinero.Optimization.Test.";

        /// <summary>
        /// Gets a feature collection for the given embedded resource.
        /// </summary>
        public static FeatureCollection GetFeatureCollection(this string embeddedResourcePath)
        {
            using (var stream = typeof(NotSoRandomGenerator).Assembly.GetManifestResourceStream(EmbeddedResourceRoot + embeddedResourcePath))
            using (var streamReader = new StreamReader(stream))
            {
                var json = streamReader.ReadToEnd();
                return json.ToFeatures();
            }
        }

        /// <summary>
        /// Builds a linestring represents the route from the given tour and locations.
        /// </summary>
        public static LineString BuildRoute(this ITour tour, IList<Itinero.LocalGeo.Coordinate> locations)
        {
            var coordinates = new List<GeoAPI.Geometries.Coordinate>();
            var first = locations[tour.First];
            coordinates.Add(new GeoAPI.Geometries.Coordinate(first.Longitude, first.Latitude));
            foreach(var pair in tour.Pairs())
            {
                var to = locations[pair.To];
                coordinates.Add(new GeoAPI.Geometries.Coordinate(to.Longitude, to.Latitude));
            }
            
            return new LineString(coordinates.ToArray());
        }

        /// <summary>
        /// Builds the the routes from the given tours and locations.
        /// </summary>
        public static FeatureCollection BuildRoutes(this IMultiTour tours, IList<Itinero.LocalGeo.Coordinate> locations)
        {
            var features = new FeatureCollection();

            for (var t = 0; t < tours.Count; t++)
            {
                var lineString = tours.Tour(t).BuildRoute(locations);
                var attributes = new AttributesTable();
                attributes.Add("tour_id", t);
                features.Add(new Feature(lineString, attributes));
            }

            return features;
        }

        /// <summary>
        /// Builds a test problem and default solution (as-the-crow-flies distances) from the given lines.
        /// </summary>
        /// <param name="features">The linestring(s).</param>
        /// <param name="locations">The locations.</param>
        /// <returns>The weights (as-the-crow-flies distances) between the points in the linestring(s).</returns>
        public static float[][] BuildMatrix(this FeatureCollection features, out List<Itinero.LocalGeo.Coordinate> locations)
        {
            List<ITour> tours;
            IAttributesTable attributes;
            return features.BuildMatrix(out tours, out locations, out attributes);
        }

        /// <summary>
        /// Builds a test problem and default solution (as-the-crow-flies distances) from the given lines.
        /// </summary>
        /// <param name="features">The linestring(s).</param>
        /// <param name="tours">The predefined tour(s).</param>
        /// <param name="locations">The locations.</param>
        /// <returns>The weights (as-the-crow-flies distances) between the points in the linestring(s).</returns>
        public static float[][] BuildMatrix(this FeatureCollection features, out List<ITour> tours, 
            out List<Itinero.LocalGeo.Coordinate> locations, out IAttributesTable attributes)
        {
            tours = new List<ITour>();
            locations = new List<Itinero.LocalGeo.Coordinate>();
            attributes = null;

            foreach (var feature in features.Features)
            {
                var multiLineString = feature.Geometry as MultiLineString;
                attributes = feature.Attributes;
                if (multiLineString != null)
                {
                    foreach (var geometry in multiLineString.Geometries)
                    {
                        var lineString = geometry as LineString;
                        var rawTour = new List<int>();
                        foreach (var coord in lineString.Coordinates)
                        {
                            var locationId = locations.AddAndCheckDuplicates(coord);
                            rawTour.Add(locationId);
                        }
                        var last = rawTour[rawTour.Count - 1];
                        if (rawTour[0] == last)
                        {
                            rawTour.RemoveAt(rawTour.Count - 1);
                        }
                        tours.Add(new Tour(rawTour, last));
                    }
                }
            }

            var weights = new float[locations.Count][];
            for (var x = 0; x < locations.Count; x++)
            {
                weights[x] = new float[locations.Count];
                for (var y = 0; y < locations.Count; y++)
                {
                    weights[x][y] = Itinero.LocalGeo.Coordinate.DistanceEstimateInMeter(
                        locations[x],
                        locations[y]
                    );
                }
            }
            return weights;
        }

        /// <summary>
        /// Adds the given location but not if there is already another location nearby. Returns the new or existing ID.
        /// </summary>
        public static int AddAndCheckDuplicates(this List<Itinero.LocalGeo.Coordinate> locations, GeoAPI.Geometries.Coordinate location, int meters = 10)
        {
            return locations.AddAndCheckDuplicates(new Itinero.LocalGeo.Coordinate((float)location.Y, (float)location.X), meters);
        }

        /// <summary>
        /// Adds the given location but not if there is already another location nearby. Returns the new or existing ID.
        /// </summary>
        public static int AddAndCheckDuplicates(this List<Itinero.LocalGeo.Coordinate> locations, Itinero.LocalGeo.Coordinate location, int meters = 10)
        {
            for (var i = 0; i  < locations.Count; i++)
            {
                if (Itinero.LocalGeo.Coordinate.DistanceEstimateInMeter(locations[i], location) < meters)
                {
                    return i;
                }
            }
            locations.Add(location);
            return locations.Count - 1;
        }

        /// <summary>
        /// Returns true if the two given tour's bounding boxes overlap.
        /// </summary>
        public static bool ToursOverlap(this List<Itinero.LocalGeo.Coordinate> locations, ITour tour1, ITour tour2)
        {
            var box1 = locations.BoundingBox(tour1);
            var box2 = locations.BoundingBox(tour2);

            return box1.Overlaps(box2);
        }

        /// <summary>
        /// Calculates the boundingbox around the given tour.
        /// </summary>
        /// <param name="algorithm">The weight matrix algorithm.</param>
        /// <param name="tour">The tour.</param>
        /// <returns></returns>
        public static LocalGeo.Box BoundingBox(this List<Itinero.LocalGeo.Coordinate> locations, ITour tour)
        {
            LocalGeo.Box? box = null;
            foreach (var visit in tour)
            {
                var visitLocation = locations[visit];
                if (box == null)
                {
                    box = new LocalGeo.Box(visitLocation, visitLocation);
                }
                else
                {
                    box = box.Value.ExpandWith(visitLocation.Latitude, visitLocation.Longitude);
                }
            }
            return box.Value;
        }
    }
}